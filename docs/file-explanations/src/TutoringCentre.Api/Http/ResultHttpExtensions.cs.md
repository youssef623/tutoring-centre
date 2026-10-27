# `src/TutoringCentre.Api/Http/ResultHttpExtensions.cs`

## Purpose

The shared transport mapper converts result kinds to status and Problem Details while preserving field dictionaries for any kind, including conflicts. Creation helpers support both Location-bearing and body-only 201 contracts. Feature endpoints choose the success shape; they do not duplicate failure switches.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 79 lines and 3,838 bytes. Source hash: `16fe3323b98a0770e9fbd81edb7f5c3f141781afd96137315392aacf79157173`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.AspNetCore.Mvc;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs#L11): `public static class ResultHttpExtensions`
- [Line 13](../../../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs#L13): `public static IResult ToHttpResult<T>(this Result<T> result, Func<T, IResult> onSuccess)`
- [Line 25](../../../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs#L25): `public static IResult ToCreatedHttpResult<T>(this Result<T> result, Func<T, (string Location, object Body)> onSuccess)`
- [Line 42](../../../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs#L42): `public static IResult ToCreatedHttpResult<T>(this Result<T> result, Func<T, object> onSuccess)`
- [Line 51](../../../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs#L51): `public static IResult ToHttpResult(this Result result) =>`
- [Line 54](../../../../../src/TutoringCentre.Api/Http/ResultHttpExtensions.cs#L54): `public static IResult ToProblemResult(this Error error)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/api/apiFetch.ts:36](../../../../../frontend/src/api/apiFetch.ts#L36)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
