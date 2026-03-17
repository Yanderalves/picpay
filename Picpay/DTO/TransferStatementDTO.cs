namespace Picpay.DTO;

public abstract record TransferBaseDTO(decimal Value, DateTimeOffset Date);

public record TransferMadeDTO(string To, decimal Value, DateTimeOffset Date) 
    : TransferBaseDTO(Value, Date);

public record TransferReceivedDTO(string From, decimal Value, DateTimeOffset Date) 
    : TransferBaseDTO(Value, Date);
    
public record TransferStatementResponseDTO(decimal currentBalance, List<TransferMadeDTO> transferMade, List<TransferReceivedDTO> transferReceived);