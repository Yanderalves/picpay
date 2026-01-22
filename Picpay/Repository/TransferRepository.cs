using Microsoft.EntityFrameworkCore;
using Picpay.Context;

namespace PicpaySimplificado.Repository;

public class TransferRepository : ITransferRepository
{
    private readonly DatabaseContext _databaseContext;
    
    public TransferRepository(DatabaseContext databaseContext)
    {
        this._databaseContext = databaseContext;
    }
    public async Task<List<Transfer>> GetTransferByPayerAndPayeeAsync(Guid id)
    {
       return await _databaseContext.Transfers.Where(t => t.PayeeId == id || t.PayerId == id).ToListAsync();
    }
}