# src/TutoringCentre.Api/Http/HttpContextKeys.cs

## Purpose

Holds the string key under which the correlation ID is stored in `HttpContext.Items`.

## Where It Fits

Api/Http, `internal static`. Used by `CorrelationIdMiddleware` (write) and `ProblemResult.Enrich` (read).

## Walkthrough

`public const string CorrelationId = "CorrelationId";` A shared constant avoids two files disagreeing on a magic string. (`ResultHttpExtensionsTests` writes the literal `"CorrelationId"`, so the test depends on the value.)

## Concepts Used

This file introduces no concept that needs a tutorial beyond what its walkthrough already explains.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

String-keyed `Items` dictionary is untyped; a typo elsewhere would silently yield null.

## Related Files

- [`src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs`](CorrelationIdMiddleware.cs.md)
- [`src/TutoringCentre.Api/Http/ProblemResult.cs`](ProblemResult.cs.md)
