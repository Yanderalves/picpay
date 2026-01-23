namespace Picpay.Exceptions;

public class UserNotAuthorized : Exception
{
    public UserNotAuthorized(string message) : base(message)
    {
        
    }
}