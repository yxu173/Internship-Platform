using SharedKernel;

namespace Web.Api.Infrastructure;

public static class CustomResults
{
    public static IResult Problem(Result result)
    {
        if (result.Error is ValidationError validationError)
        {
            var errorResponse = new
            {
                type = "https://tools.ietf.org/html/rfc7231#section-6.5.4",
                title = result.Error.Code,
                status = StatusCodes.Status400BadRequest,
                errors = validationError.ErrorsByProperty
                    .ToDictionary(
                        kvp => ToCamelCase(kvp.Key), 
                        kvp => kvp.Value.Select(e => e.Description).ToArray()
                    ),
                detail = result.Error.Description
            };

            return Results.BadRequest(errorResponse);
        }

        var statusCode = result.Error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Problem => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status400BadRequest
        };

        return Results.Problem(
            statusCode: statusCode,
            title: result.Error.Code,
            detail: result.Error.Description,
            type: "https://tools.ietf.org/html/rfc7231#section-6.5.4"
        );
    }

    public static IResult Problem<T>(Result<T> result)
    {
        return Problem(result as Result);
    }

    private static string ToCamelCase(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;
            
        return char.ToLowerInvariant(value[0]) + value[1..];
    }
}