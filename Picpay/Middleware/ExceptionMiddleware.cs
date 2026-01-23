using System.Net;
using Picpay.DTO;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Picpay.Middleware;

public class ExceptionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            await HandleExceptionAsync(context, e);
        }
    }

    public static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var statusCode = exception switch
        {
            Picpay.Exceptions.UserNotFoundException => (int)HttpStatusCode.NotFound,
            Picpay.Exceptions.UserAlreadyExistsException => (int)HttpStatusCode.Conflict,
            Picpay.Exceptions.InsufficientFundsException => (int)HttpStatusCode.BadRequest,
            Picpay.Exceptions.UserNotAuthorized => (int)HttpStatusCode.Unauthorized,
            _ => (int)HttpStatusCode.InternalServerError
        };

        context.Response.StatusCode = statusCode;

        var response = new ApiResponse<object>( Success: false, Message: exception.Message, StatusCode: statusCode);
        
        return context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}