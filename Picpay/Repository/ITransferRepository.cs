using Picpay.Models;

namespace Picpay.Repository;

public interface ITransferRepository
{
    public Task<List<Transfer>> GetTransferByPayerAndPayeeAsync(Guid id);
}