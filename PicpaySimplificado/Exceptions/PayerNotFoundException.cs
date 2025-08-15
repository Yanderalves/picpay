namespace Picpay.Exceptions;

public class PayerNotFoundException : Exception
{
    public PayerNotFoundException(string message) : base(message) 
    {
        
    }
}