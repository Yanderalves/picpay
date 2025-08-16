using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Picpay.Context;
using Picpay.DTO;
using Picpay.Models;
using Picpay.Exceptions;
using PicpaySimplificado.Service;

namespace Picpay.Routes;

public static class Routes
{
    public static void UseRoutes(this WebApplication app)
    {
        var users = app.MapGroup("users");
        
        users.MapGet("{id}", async (Guid id, [FromServices] DatabaseContext context) =>
        {   
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if(user is null)
                return Results.NotFound();
            
            var userResponseDto = new UserResponseDTO(user.Id, user.Email, user.Name, user.Type, user.Balance);
            
            return Results.Ok(userResponseDto);
        });

        users.MapGet("",
            async ([FromServices] DatabaseContext context) =>
            {
                var users = await context.Users.ToListAsync();
                if (users is null)
                    return Results.NotFound();

                var usersDto = users.Select(item =>
                    new UserResponseDTO
                        (item.Id, item.Email, item.Name, item.Type, item.Balance)).ToList();
                
                return Results.Ok(usersDto);
            });

        users.MapPost("", async (UserRegisterDTO userRegisterDto, [FromServices] DatabaseContext context) =>
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == userRegisterDto.email || x.Identifier == userRegisterDto.identifier);
            if(user is not null)
                return Results.Conflict("User already exists ");

            User newUser = new User(userRegisterDto.name, userRegisterDto.email, userRegisterDto.password, userRegisterDto.type,  userRegisterDto.identifier);
            await context.Users.AddAsync(newUser);
            await context.SaveChangesAsync();
            return Results.Created($"/users/{newUser.Id}", user);
        });

        users.MapGet("/statement/{id}", async (Guid id, [FromServices] DatabaseContext context) =>
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user is null)
            {
                return Results.NotFound();
            }

            var transfers = await context.Transfers
                .Where(t => t.PayeeId == id || t.PayerId == id)
                .ToListAsync();

            var transfersMade = new List<object>();
            var transfersReceived = new List<object>();
    
            var allUserIds = transfers.Select(t => t.PayerId).Union(transfers.Select(t => t.PayeeId)).ToHashSet();
    
            var allUsers = await context.Users
                .Where(u => allUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id, u => u.Name);

            foreach (var transfer in transfers)
            {
                if (transfer.PayeeId == id)
                {
                    transfersReceived.Add(new
                    {
                        from = allUsers.GetValueOrDefault(transfer.PayerId, "User Not Found"),
                        value = transfer.Value,
                        date = transfer.CreatedAt
                    });
                }
                else if (transfer.PayerId == id)
                {
                    transfersMade.Add(new
                    {
                        to = allUsers.GetValueOrDefault(transfer.PayeeId, "User Not Found"),
                        value = transfer.Value,
                        date = transfer.CreatedAt
                    });
                }
            }
    
            return Results.Ok(new
            {
                currentBalance = user.Balance,
                transfersMade = transfersMade,
                transfersReceived = transfersReceived
            });
        });
        
        var transfer = app.MapGroup("transfer");

        transfer.MapPost("",
            async ([FromServices] DatabaseContext context, [FromServices] TransferService transferService,
                TransferDTO transferDto) =>
            {
                try
                {
                    await transferService.ExecuteTransferAsync(transferDto);
                    return Results.NoContent();
                }
                catch (PayerNotFoundException ex)
                {
                    return Results.UnprocessableEntity(ex.Message);
                }
                catch (PayeeNotFoundException ex)
                {
                    return Results.UnprocessableEntity(ex.Message);
                }
                catch (InvalidUserTypeException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (InsufficientFundsException ex)
                {
                    return Results.BadRequest(ex.Message);
                }
                catch (RequestNotAuthorized)
                {
                    return Results.Unauthorized();
                }
                catch (Exception)
                {
                    return Results.Problem(
                        detail: "An unexpected server error occurred. Please try again later.",
                        statusCode: StatusCodes.Status500InternalServerError);
                }
            });
    }
}