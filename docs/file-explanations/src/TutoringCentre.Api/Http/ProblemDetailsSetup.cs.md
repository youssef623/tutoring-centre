# `src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs`

## Purpose

Source summary (intent, not independent execution proof): Registers Problem Details for framework-generated errors (empty 404/405, exception handler fallbacks).

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 17 lines and 626 bytes. Source hash: `2c7a79057afad88dd8bb0cd50fa5fe986b8f1f44c83dbe837f8a25c29213fa34`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 4](../../../../../src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs#L4): `public static class ProblemDetailsSetup`
- [Line 6](../../../../../src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs#L6): `public static IServiceCollection AddApiProblemDetails(this IServiceCollection services)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Api.Http;

/// <summary>Registers Problem Details for framework-generated errors (empty 404/405, exception handler fallbacks).</summary>
public static class ProblemDetailsSetup
{
    public static IServiceCollection AddApiProblemDetails(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddProblemDetails(options =>
            options.CustomizeProblemDetails = context => ProblemResult.Enrich(context.ProblemDetails, context.HttpContext));

        services.AddExceptionHandler<GlobalExceptionHandler>();

        return services;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
