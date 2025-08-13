using PicpaySimplificado.Enums;

namespace PicpaySimplificado.DTO;

public record UserRegisterDTO(string name, string identifier, string email, string password, UserType type);