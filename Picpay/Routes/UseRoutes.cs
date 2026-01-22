using Microsoft.AspNetCore.Mvc;
using Picpay.Context;
using Picpay.DTO;
using Picpay.Models;
using Picpay.Exceptions;
using Picpay.Service;
using PicpaySimplificado.Repository;

namespace Picpay.Routes;

public static class Routes
{
    public static void UseRoutes(this WebApplication app)
    {
        var users = app.MapGroup("users");

        users.MapPost("/login",
            async ([FromServices] IUserService userService, UserLoginDTO userLoginDto) =>
            {
                try
                {
                    var token = await userService.LoginAsync(userLoginDto);
                    return token == string.Empty ? Results.Unauthorized() : Results.Ok(token);
                }
                catch (UserNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Results.Problem();
                }

            });
        
        users.MapGet("{id}", async (Guid id, [FromServices] IUserService userService) =>
        {
            try
            {
                var user = await userService.GetUserByIdAsync(id);
                return Results.Ok(user);
            }
            catch (UserNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return Results.Problem();
            }
        });

        users.MapGet("",
            async ([FromServices] IUserService userService) =>
            {
                try
                {
                    var users = await userService.GetAllUsersAsync();
                    return Results.Ok(users);
                }
                catch (UserNotFoundException ex)
                {
                    return Results.NotFound(ex.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return Results.Problem();
                }
            });

        users.MapPost("", async (UserRegisterDTO userRegisterDto, [FromServices] IUserService userService) =>
        {
            try
            {
                await userService.CreateUserAsync(userRegisterDto);
                return Results.Created();
            }
            catch (UserAlreadyExistsException ex)
            {
                return Results.Conflict(ex.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        });

        users.MapGet("/statement/{id}", async (Guid id, [FromServices] IUserService userService) =>
        {
            try
            {
                var statement =  await userService.GetStatementByUserId(id);
                return Results.Ok(statement);
            }
            catch (UserNotFoundException ex)
            {
                return Results.NotFound(ex.Message);
            }
            catch (Exception e)
            {
                return Results.Problem();
            }
        });
        
        var transfer = app.MapGroup("transfer").RequireAuthorization();

        transfer.MapPost("",
            async ([FromServices] DatabaseContext context, [FromServices] TransferService transferService, HttpContext httpContext,
                TransferDTO transferDto) =>
            {
                try
                {
                    var userIdFromToken = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                    if (userIdFromToken is null || userIdFromToken != transferDto.Payer.ToString())
                        return Results.Forbid();
                        
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