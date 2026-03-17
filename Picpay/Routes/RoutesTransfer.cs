using Microsoft.AspNetCore.Mvc;
using Picpay.Context;
using Picpay.DTO;
using Picpay.Service;

namespace Picpay.Routes;

public static class RoutesTransfer
{
    public static void TransferRoutes(this WebApplication app)
    {
        var transfer = app.MapGroup("transfer").RequireAuthorization();

        transfer.MapPost("",
            async ([FromServices] DatabaseContext context, [FromServices] ITransferService transferService, 
                HttpContext httpContext, TransferDTO transferDto) =>
            {
                var userIdFromToken = httpContext.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userIdFromToken is null || userIdFromToken != transferDto.Payer.ToString())
                    return Results.Forbid();
                    
                await transferService.ExecuteTransferAsync(transferDto);
                return Results.NoContent();
            });
    }
}