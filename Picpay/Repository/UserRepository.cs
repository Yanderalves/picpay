using Microsoft.EntityFrameworkCore;
using Picpay.Context;
using Picpay.Models;

namespace PicpaySimplificado.Repository;

public class UserRepository(DatabaseContext context) : IUserRepository
{
    public async Task CreateUserAsync(User user)
    {
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Email == email);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Id== id);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByEmailOrIdentifier(string? email, string? identifier)
    {
        return await context.Users.FirstOrDefaultAsync(x => x.Email == email || x.Identifier == identifier);
    }

    public async Task<Dictionary<Guid, string>> GetNamesMapByIdsAsync(Guid id, HashSet<Guid> allUserIds)
    {
        var allUsers = await context.Users
            .Where(u => allUserIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Name);
        
        return allUsers;
    }
}