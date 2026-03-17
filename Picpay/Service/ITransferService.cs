using Picpay.DTO;
using Picpay.Models;

namespace Picpay.Service;

public interface ITransferService
{
    public Task ExecuteTransferAsync(TransferDTO transferDto);
    public Task<(User payer, User payee)> ValidateTransferAndGetUserAsync(TransferDTO transferDto);
}