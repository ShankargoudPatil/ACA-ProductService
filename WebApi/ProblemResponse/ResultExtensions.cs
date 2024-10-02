using Application.Shared.Response;
using Application.Shared.Responses;
namespace WebApi.ProblemResponse;
public static class ResultExtensions
{
    public static IResult ToProblemDetails(this Result result)
    {
        if (result.IsSuccess)
        {
            throw new InvalidOperationException();
        }
        return Results.Problem(
            statusCode: GetStatusCode(result.Error.ErrorTypeEnum),
            title: GetTitle(result.Error.ErrorTypeEnum),
            type: GetType(result.Error.ErrorTypeEnum),
            extensions: new Dictionary<string, object?>
            {
                {"error",new[]{result.Error } }
            });
    }

    static int GetStatusCode(ErrorTypes errorType) =>
        errorType switch
        {
            ErrorTypes.NotFound => StatusCodes.Status404NotFound,
            ErrorTypes.BadRequest => StatusCodes.Status400BadRequest,
            ErrorTypes.Conflict => StatusCodes.Status409Conflict,
            ErrorTypes.Validation => StatusCodes.Status400BadRequest,
            ErrorTypes.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

    static string GetTitle(ErrorTypes errorType) =>
         errorType switch
         {
             ErrorTypes.NotFound => "Not found",
             ErrorTypes.BadRequest => "Bad Request",
             ErrorTypes.Conflict => "Conflict",
             ErrorTypes.Validation => "The request is invalid",
             ErrorTypes.Forbidden => "The request is forbidden",
             _ => "An error occurred"
         };


    static string GetType(ErrorTypes errorType) =>
       errorType switch
       {
           ErrorTypes.NotFound => "https://tools.ietf.org/html/rfc7231#section-6.5.4",
           ErrorTypes.BadRequest => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
           ErrorTypes.Conflict => "https://tools.ietf.org/html/rfc7231#section-6.5.8",
           ErrorTypes.Validation => "https://tools.ietf.org/html/rfc7231#section-6.5.1",
           ErrorTypes.Forbidden => "https://tools.ietf.org/html/rfc7231#section-6.5.3",
           _ => "https://tools.ietf.org/html/rfc7231#section-6.6.1"
       };
}
