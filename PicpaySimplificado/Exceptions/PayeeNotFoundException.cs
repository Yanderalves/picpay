namespace Picpay.Exceptions;

public class PayeeNotFoundException  : Exception
{
    public PayeeNotFoundException(string message) :  base(message)
    {
        
    }
}