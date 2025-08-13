using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PicpaySimplificado.Api;
using PicpaySimplificado.Context;
using PicpaySimplificado.DTO;
using PicpaySimplificado.Enums;
using PicpaySimplificado.Models;
using PicpaySimplificado.Service;

namespace PicpaySimplificado.Routes;

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
            return Results.Ok(user);
        });

        users.MapPost("", async (UserRegisterDTO userRegisterDto, [FromServices] DatabaseContext context) =>
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == userRegisterDto.email || x.Identifier == userRegisterDto.identifier);
            if(user is not null)
                return Results.Conflict("User already exists ");

            User newUser = new User(userRegisterDto.name, userRegisterDto.email, userRegisterDto.password, userRegisterDto.type,  userRegisterDto.identifier);
            context.Users.Add(newUser);
            context.SaveChanges();
            return Results.Created($"/users/{newUser.Id}", user);
        });
        
        var transfer = app.MapGroup("transfer");

        transfer.MapPost("", async ([FromServices]  DatabaseContext context, [FromServices] TransferService transferService, TransferDTO transferDto) =>
        {
            var result = await transferService.ExecuteTransferAsync(transferDto);
            return result;

        });

    }
}