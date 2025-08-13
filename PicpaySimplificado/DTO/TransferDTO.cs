namespace PicpaySimplificado.DTO;

public record TransferDTO(decimal value, Guid payer, Guid payee);