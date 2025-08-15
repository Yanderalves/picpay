using Picpay.Enums;

namespace Picpay.DTO;

public record UserRegisterDTO(string name, string identifier, string email, string password, UserType type);