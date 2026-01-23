using Picpay.DTO;
using Picpay.Enums;
using Picpay.Models;
using PicpaySimplificado.DTO;

namespace Picpay.Service;

public interface IUserService
{
    public Task<UserResponseDTO> CreateUserAsync(UserRegisterDTO userRegisterDto);
    public Task<User?> GetUserByEmailAsync(string email);
    public Task<UserResponseDTO?> GetUserByIdAsync(Guid id);
    public Task<List<UserResponseDTO>> GetAllUsersAsync();
    public Task<User?> GetUserByEmailOrIdentifier(string? email,  string? identifier);
    public Task<string> LoginAsync(UserLoginDTO userLoginDto);
    public Task<TransferStatementResponseDTO> GetStatementByUserId(Guid userId);
    public Task<List<UserResponseDTO>> GetUsersByType(UserType type);
}