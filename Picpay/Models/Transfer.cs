public class Transfer
{
    public Guid Id { get; init; }
    public decimal Value { get; set; }
    public Guid PayerId { get; set; }
    public Guid PayeeId { get; set; }
    public DateTime CreatedAt { get; set; }

    public Transfer(decimal value, Guid payerId, Guid payeeId)
    {
        Id = Guid.NewGuid();
        Value = value;
        PayerId = payerId;
        PayeeId = payeeId;
        CreatedAt = DateTime.Now;
    }
}