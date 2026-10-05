using Microsoft.AspNetCore.Mvc;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Api.Http;

/// <summary>
/// The only place domain outcomes become HTTP responses. Endpoints never write their own switch.
/// 400 = your input is malformed; 401 = we don't know who you are; 403 = we know, and you may not;
/// 404 = doesn't exist (for you); 409 = conflicts with current state; 422 = well-formed but breaks a business rule.
/// </summary>
public static class ResultHttpExtensions
{
    public static IResult ToHttpResult<T>(this Result<T> result, Func<T, IResult> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);

        return result.IsSuccess ? onSuccess(result.Value) : result.Error!.ToProblemResult();
    }

    public static IResult ToHttpResult(this Result result) =>
        result.IsSuccess ? Results.NoContent() : result.Error!.ToProblemResult();

    public static IResult ToProblemResult(this Error error)
    {
        ArgumentNullException.ThrowIfNull(error);

        var (status, title) = error.Kind switch
        {
            ErrorKind.Validation => (StatusCodes.Status400BadRequest, "One or more validation errors occurred."),
            ErrorKind.NotFound => (StatusCodes.Status404NotFound, "The requested resource was not found."),
            ErrorKind.Conflict => (StatusCodes.Status409Conflict, "The request conflicts with the current state."),
            ErrorKind.Rule => (StatusCodes.Status422UnprocessableEntity, "The request breaks a business rule."),
            ErrorKind.Forbidden => (StatusCodes.Status403Forbidden, "You are not allowed to perform this action."),
            ErrorKind.Unauthenticated => (StatusCodes.Status401Unauthorized, "Authentication is required."),
            _ => throw new ArgumentOutOfRangeException(nameof(error), error.Kind, "Unmapped error kind."),
        };

        // Messages on Error are authored, safe strings — never exception text.
        ProblemDetails problem = error.Kind == ErrorKind.Validation && error.Fields is not null
            ? new ValidationProblemDetails(new Dictionary<string, string[]>(error.Fields)) { Status = status, Title = title, Detail = error.Message }
            : new ProblemDetails { Status = status, Title = title, Detail = error.Message };

        problem.Extensions["code"] = error.Code;
        return new ProblemResult(problem);
    }
}
