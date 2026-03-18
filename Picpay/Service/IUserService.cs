using Picpay.DTO;
using Picpay.Enums;
using Picpay.Models;

namespace Picpay.Service;

public interface IUserService
{
    public Task<UserResponseDTO> CreateUserAsync(UserRegisterDTO userRegisterDto);
    public Task<UserResponseDTO?> GetUserByIdAsync(Guid id);
    public Task<(List<UserResponseDTO>, int)> GetAllUsersAsync(int page, int pageSize);
    public Task<string> LoginAsync(UserLoginDTO userLoginDto);
    public Task<TransferStatementResponseDTO> GetStatementByUserId(Guid userId);
    public Task<List<UserResponseDTO>> GetUsersByType(UserType type);
    public Task AddBalanceAsync(BalanceDTO balanceDto);
}