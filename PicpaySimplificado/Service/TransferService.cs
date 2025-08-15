using Microsoft.EntityFrameworkCore;
using Picpay.Api;
using Picpay.Context;
using Picpay.DTO;
using Picpay.Enums;
using Picpay.Exceptions;
using Picpay.Models;

namespace PicpaySimplificado.Service;

public class TransferService(DatabaseContext context, AuthorizationClient client)
{
    private DatabaseContext Context { get; } = context;
    private async Task<(User payer, User payee)> ValidateTransferAndgetUserAsync(TransferDTO transferDto)
    {
        var payee =  await Context.Users.FirstOrDefaultAsync(x => x.Id == transferDto.Payee);
        var payer =  await Context.Users.FirstOrDefaultAsync(x => x.Id == transferDto.Payer);
                    
        if(payer is null)
            throw new PayerNotFoundException("Payer not found");
        if(payee is null)
            throw new PayeeNotFoundException("Payee not found");
                    
        if(payer.Type == UserType.Merchant)
            throw new InvalidUserTypeException("Invalid user type");
                    
        if(payer.Balance -  transferDto.Value < 0)
            throw new InsufficientFundsException("Not enough balance");
        
        return (payer, payee);
    }
    public async Task ExecuteTransferAsync(TransferDTO  transferDto)
    {
        await using var transaction = await Context.Database.BeginTransactionAsync();
        try
        {
            var (payer, payee) = await ValidateTransferAndgetUserAsync(transferDto);

            if (payer is null && payee is null)
                throw new InvalidOperationException("Payer and payee not found");
            
            var response = await client.AuthorizeTransferAsync();

            if (response.Data.Authorization == false)
                throw new RequestNotAuthorized("Request not authorized");

            var transfer = new Transfer(transferDto.Value, transferDto.Payer, transferDto.Payee);
                
            payer?.Debit(transferDto.Value);
            payee.Credit(transferDto.Value);
                
            await Context.Transfers.AddAsync(transfer);
            await Context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}