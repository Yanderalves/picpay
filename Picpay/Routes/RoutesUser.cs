using System.Net;
using Microsoft.AspNetCore.Mvc;
using Picpay.DTO;
using Picpay.Enums;
using Picpay.Models;
using Picpay.Service;

namespace Picpay.Routes;

public static class RoutesUser
{
    public static void UserRoutes(this WebApplication app)
    {
        var users = app.MapGroup("users");

        users.MapPost("/login",
            async ([FromServices] IUserService userService, UserLoginDTO userLoginDto) =>
            {
                var token = await userService.LoginAsync(userLoginDto);
                
                return Results.Ok(new ApiResponse<object>(true, Message: "Login successfully", Data: new {token, }));

            });
        
        users.MapGet("{id}", async (Guid id, [FromServices] IUserService userService) =>
        {
            var user = await userService.GetUserByIdAsync(id);
            
            return Results.Ok(new ApiResponse<object>(Success: true, Data: user is not null?  new {user} : null));
                
        });

        users.MapGet("",
            async ([FromServices] IUserService userService, [FromQuery] UserType? type, 
                [FromQuery] int? page, [FromQuery]int? pageSize) =>
            {
                List<UserResponseDTO> allUsersAsync;
                var totalItems = 0;
                
                if (type.HasValue)
                    allUsersAsync = await userService.GetUsersByType(type.Value);  
                else
                    (allUsersAsync, totalItems)  = await userService.GetAllUsersAsync(page ?? 1,  pageSize ?? 20);

                return Results.Ok(new ApiResponse<object>(Success: true, Data: new { users = allUsersAsync }, 
                    Pagination: new PagedResultDTO(
                        Page: page ?? 1,
                        PageSize: pageSize ?? 20,
                        TotalItems: totalItems,
                        TotalPages: (int)Math.Ceiling((double)totalItems / (pageSize ?? 20))
                )));
            });

        users.MapPost("", async (UserRegisterDTO userRegisterDto, [FromServices] IUserService userService) =>
        {
            var user = await userService.CreateUserAsync(userRegisterDto);
            
            return Results.Created($"users/{user.Id}", new ApiResponse<UserResponseDTO>(Success: true, StatusCode: (int)HttpStatusCode.Created, Data: user));
        });

        users.MapGet("/statement/{id}", async (Guid id, [FromServices] IUserService userService) =>
        {
            var statement =  await userService.GetStatementByUserId(id);
            return Results.Ok(new ApiResponse<object>(Success: true, Data: new {statement}));
        });

        users.MapPost("deposit",async ([FromServices]  IUserService userService,
            [FromBody] BalanceDTO balanceDto, HttpContext httpContext) =>
        {
            var userIdFromToken = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (userIdFromToken is null || userIdFromToken != balanceDto.UserId.ToString())
                return Results.Json(new ApiResponse<object>(Success: false, Message: "Invalid Token", StatusCode: 403), statusCode: 403);
            
            await userService.AddBalanceAsync(balanceDto);
            return Results.Ok(new ApiResponse<object>(Success: true, Message: "Balance added successfully."));
        }).RequireAuthorization();
    }
    
}