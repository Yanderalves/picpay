namespace PicpaySimplificado.DTO;

public abstract record TransferBaseDTO(decimal Value, DateTime Date);

public record TransferMadeDTO(string To, decimal Value, DateTime Date) 
    : TransferBaseDTO(Value, Date);

public record TransferReceivedDTO(string From, decimal Value, DateTime Date) 
    : TransferBaseDTO(Value, Date);
    
public record TransferStatementResponseDTO(decimal currentBalance, List<TransferMadeDTO> transferMade, List<TransferReceivedDTO> transferReceived);