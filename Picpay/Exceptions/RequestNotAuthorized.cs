namespace Picpay.Exceptions;

public class RequestNotAuthorized : Exception
{
    public RequestNotAuthorized(string message) : base(message)
    {
        
    }
}