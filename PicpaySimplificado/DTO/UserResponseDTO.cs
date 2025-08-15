using Picpay.Enums;

namespace Picpay.DTO;

public record UserResponseDTO(Guid Id, string Email, string Name, UserType Type, decimal Balance);