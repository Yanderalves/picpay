using System.Net;
using Microsoft.AspNetCore.Mvc;
using Picpay.Context;
using Picpay.DTO;
using Picpay.Models;
using Picpay.Exceptions;
using Picpay.Service;
using PicpaySimplificado.DTO;
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
                var token = await userService.LoginAsync(userLoginDto);
                
                return Results.Ok(new ApiResponse<object>(true, "Login successfully", Data: new {token}));

            });
        
        users.MapGet("{id}", async (Guid id, [FromServices] IUserService userService) =>
        {
            var user = await userService.GetUserByIdAsync(id);
            
            return Results.Ok(new ApiResponse<object>(Success: true, Data: user is not null?  new {user} : null));
                
        });

        users.MapGet("",
            async ([FromServices] IUserService userService) =>
            {
                var users = await userService.GetAllUsersAsync();
                return Results.Ok(new ApiResponse<object>(Success: true, Data: new { users = users}));
            });

        users.MapPost("", async (UserRegisterDTO userRegisterDto, [FromServices] IUserService userService) =>
        {
            var user = await userService.CreateUserAsync(userRegisterDto);
            
            return Results.Created($"users/{user.Id}", new ApiResponse<UserResponseDTO>(Success: true, Data: user, StatusCode: (int)HttpStatusCode.Created));
        });

        users.MapGet("/statement/{id}", async (Guid id, [FromServices] IUserService userService) =>
        {
            var statement =  await userService.GetStatementByUserId(id);
            return Results.Ok(new ApiResponse<object>(Success: true, Data: new {statement}));
           
        });
        
        var transfer = app.MapGroup("transfer").RequireAuthorization();

        transfer.MapPost("",
            async ([FromServices] DatabaseContext context, [FromServices] TransferService transferService, HttpContext httpContext,
                TransferDTO transferDto) =>
            {
                var userIdFromToken = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdFromToken is null || userIdFromToken != transferDto.Payer.ToString())
                    return Results.Forbid();
                    
                await transferService.ExecuteTransferAsync(transferDto);
                return Results.NoContent();
                
            });
    }
}