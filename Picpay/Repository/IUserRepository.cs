using Picpay.Models;

namespace PicpaySimplificado.Repository;

public interface IUserRepository
{
    public Task CreateUserAsync(User user);
    public Task<User?> GetUserByEmailAsync(string email);
    public Task<User?> GetUserByIdAsync(Guid id);
    public Task<List<User>> GetAllUsersAsync();
    public Task<User?> GetUserByEmailOrIdentifier(string? email,  string? identifier);
    public Task<Dictionary<Guid, string>> GetNamesMapByIdsAsync(Guid id, HashSet<Guid> allUserIds);
    
}