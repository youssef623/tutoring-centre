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

    /// <summary>
    /// A successful creation maps to 201 with a Location header, built only from server-generated values (the
    /// result's own value) — never from caller-supplied input. Failures share the exact same mapping as
    /// <see cref="ToHttpResult{T}"/>, not a second switch.
    /// </summary>
    public static IResult ToCreatedHttpResult<T>(this Result<T> result, Func<T, (string Location, object Body)> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);

        if (result.IsFailure)
        {
            return result.Error!.ToProblemResult();
        }

        var (location, body) = onSuccess(result.Value);
        return Results.Created(location, body);
    }

    /// <summary>
    /// A 201 variant for a created resource with no single-resource read endpoint to point a Location header
    /// at (Day 30: staff creation). Same failure mapping as <see cref="ToHttpResult{T}"/>.
    /// </summary>
    public static IResult ToCreatedHttpResult<T>(this Result<T> result, Func<T, object> onSuccess)
    {
        ArgumentNullException.ThrowIfNull(onSuccess);

        return result.IsSuccess
            ? Results.Json(onSuccess(result.Value), statusCode: StatusCodes.Status201Created)
            : result.Error!.ToProblemResult();
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

        // Messages on Error are authored, safe strings — never exception text. Any error carrying field-level
        // detail gets the "errors" object, not just Validation: a 409 duplicate-name conflict (Day 26) names its
        // field exactly like a 400 shape failure does.
        ProblemDetails problem = error.Fields is not null
            ? new ValidationProblemDetails(new Dictionary<string, string[]>(error.Fields)) { Status = status, Title = title, Detail = error.Message }
            : new ProblemDetails { Status = status, Title = title, Detail = error.Message };

        problem.Extensions["code"] = error.Code;
        return new ProblemResult(problem);
    }
}
