namespace Picpay.DTO;

public record TransferDTO(decimal Value, Guid Payer, Guid Payee);