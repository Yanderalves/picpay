using System.Net;
using Microsoft.EntityFrameworkCore;
using PicpaySimplificado.Api;
using PicpaySimplificado.Context;
using PicpaySimplificado.DTO;
using PicpaySimplificado.Enums;

namespace PicpaySimplificado.Service;

public class TransferService
{
    public DatabaseContext _context { get; set; }
    private readonly AuthorizationClient _client;
    public TransferService(DatabaseContext context, AuthorizationClient client)
    {
        _context = context;
        _client = client;
    }
    public async Task<IResult> ExecuteTransferAsync(TransferDTO  transferDto)
    {
        using (var transaction = _context.Database.BeginTransaction())
        {
            try
            {
                var payee =  await _context.Users.FirstOrDefaultAsync(x => x.Id == transferDto.payee);
                var payer =  await _context.Users.FirstOrDefaultAsync(x => x.Id == transferDto.payer);
                    
                if(payer is null)
                    return Results.BadRequest("Payer not found");
                if(payee is null)
                    return Results.BadRequest("Payee not found");
                    
                if(payer.Type == UserType.Merchant)
                    return Results.BadRequest("Type is invalid");
                    
                if(payer.Balance -  transferDto.value < 0)
                    return Results.BadRequest("Not enough balance");

                AuthorizationResponse response = await _client.AuthorizeTransferAsync();

                if (response.Data.Authorization == false)
                    return Results.Unauthorized();

                Transfer transfer = new Transfer(transferDto.value, transferDto.payer, transferDto.payee);
                    
                payer.Debit(transferDto.value);
                payee.Credit(transferDto.value);
                    
                await _context.Transfers.AddAsync(transfer);
                await _context.SaveChangesAsync();
                transaction.Commit();
                    
                return Results.Ok();
            }
            catch (Exception e)
            {
                transaction.Rollback();
                return Results.StatusCode(500);
                
            }
        }
    }
}