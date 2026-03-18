using Picpay.DTO;
using Picpay.Enums;
using Picpay.Exceptions;
using Picpay.Models;
using Picpay.Repository;

namespace Picpay.Service;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly TokenService _tokenService;
    private readonly ITransferRepository _transferRepository;
    
    public UserService(IUserRepository userRepository,  TokenService tokenService, ITransferRepository  transferRepository)
    {
        this._transferRepository = transferRepository;
        this._userRepository = userRepository;
        this._tokenService = tokenService;
    }
    
    public async Task<UserResponseDTO> CreateUserAsync(UserRegisterDTO userRegisterDto)
    {
        var user = await _userRepository.GetUserByEmailOrIdentifier(userRegisterDto.email,  userRegisterDto.identifier);
        if(user is not null)
            throw new UserAlreadyExistsException("User already exists");

        User newUser = new User(userRegisterDto.name, userRegisterDto.email, userRegisterDto.password,
            userRegisterDto.type,  userRegisterDto.identifier);
        
        await _userRepository.CreateUserAsync(newUser);
        
        var userDto = new UserResponseDTO(newUser.Id, newUser.Email, newUser.Name, newUser.Type, newUser.Balance);
        return userDto;
    }
    

    public async Task<UserResponseDTO?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetUserByIdAsync(id);
        
        if(user is null)
            throw new UserNotFoundException("User not found");
            
        var userResponseDto = new UserResponseDTO(user.Id, user.Email, user.Name, user.Type, user.Balance);

        return userResponseDto;
    }

    public async Task<(List<UserResponseDTO>, int)> GetAllUsersAsync(int page, int pageSize)
    {
        var (users, total) = await _userRepository.GetAllUsersAsync(page, pageSize);
        if (users is null)
            throw new UserNotFoundException("No users were found");

        var usersDto = users.Select(item =>
            new UserResponseDTO
                (item.Id, item.Email, item.Name, item.Type, item.Balance)).ToList();

        return (usersDto, total);
    }

    public async Task<string> LoginAsync(UserLoginDTO userLoginDto)
    {
        var user = await _userRepository.GetUserByEmailAsync(userLoginDto.email);
        if (user is null)
            throw new UserNotFoundException("User not found");

        if (!BCrypt.Net.BCrypt.Verify(userLoginDto.password, user.Password))
            throw new UserNotAuthorized("User not authorized");
        
        var token = _tokenService.GenerateToken(user);
        
        return token;
    }

    public async Task<TransferStatementResponseDTO> GetStatementByUserId(Guid userId)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user is null)
            throw new  UserNotFoundException("User not found");
        
        var transfers = await _transferRepository.GetTransferByPayerAndPayeeAsync(userId);
        
        var transfersMade = new List<TransferMadeDTO>();
        var transfersReceived = new List<TransferReceivedDTO>();
        
        var allUserIds = transfers.Select(t => t.PayerId).Union(transfers.Select(t => t.PayeeId)).ToHashSet();

        var allUsers = await _userRepository.GetNamesMapByIdsAsync(userId, allUserIds);
        
        foreach (var transfer in transfers)
        {
            if (transfer.PayeeId == userId)
            {
                transfersReceived.Add(new TransferReceivedDTO(
                    From: allUsers.GetValueOrDefault(transfer.PayerId, "User Not Found"),
                    Value: transfer.Value,
                    Date: transfer.CreatedAt)
                );
            }
            else if (transfer.PayerId == userId)
            {
                transfersMade.Add(new TransferMadeDTO(
                    To: allUsers.GetValueOrDefault(transfer.PayeeId, "User Not Found"),
                    Value: transfer.Value,
                    Date: transfer.CreatedAt)
                );
            }
        }

        return new TransferStatementResponseDTO(
            currentBalance: user.Balance,
            transferMade: transfersMade,
            transferReceived: transfersReceived
        );

    }

    public async Task<List<UserResponseDTO>> GetUsersByType(UserType type)
    {
        var users = await _userRepository.GetUsersByType(type);
        
        var usersDto = users.Select(item =>
            new UserResponseDTO
                (item.Id, item.Email, item.Name, item.Type, item.Balance)).ToList();
        
        return usersDto;
    }

    public async Task AddBalanceAsync(BalanceDTO balanceDto)
    { 
        var user =  await _userRepository.GetUserByIdAsync(balanceDto.UserId);
        if (user is null)
            throw new UserNotFoundException("User not found");
        
        user.Credit(balanceDto.Balance);
        await _userRepository.SaveChangesAsync();
    }
}