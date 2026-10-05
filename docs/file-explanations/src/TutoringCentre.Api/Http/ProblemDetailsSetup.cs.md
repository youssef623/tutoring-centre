# src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs

## Purpose

Registers Problem Details generation for framework-produced errors and wires the global exception handler.

## Where It Fits

Api/Http, `public static`. Called at `Program.cs:46`.

## Walkthrough

`AddApiProblemDetails(this IServiceCollection)`: `services.AddProblemDetails(options => options.CustomizeProblemDetails = context => ProblemResult.Enrich(context.ProblemDetails, context.HttpContext))` - every framework-generated problem (empty 404/405 via `UseStatusCodePages`, exception-handler fallbacks) gets `traceId` and `correlationId`; `services.AddExceptionHandler<GlobalExceptionHandler>()`.

## Concepts Used

### Problem Details (RFC 9457) and centralised error handling

#### What it means

Problem Details is a standard JSON error shape (`title`, `status`, `detail`, extensions) served as `application/problem+json`. Centralising error writing in one place keeps every error response uniform and prevents leaking internals.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling](../../../../PROJECT_OVERVIEW2.md#612-http-problem-details-and-error-handling).)

#### Where it appears in this file

Enrichment hook.

#### How it works here

`CustomizeProblemDetails`.

#### Why it matters here

Framework errors look like application errors.

### Dependency Injection (DI) and the container

#### What it means

A class that needs a collaborator can create it itself (`new X()`), which hard-wires the choice, or it can *receive* it, usually as a constructor parameter. That second approach is dependency injection. A DI *container* stores *registrations* ("when asked for type A, build type B with lifetime L") and performs *resolution*: it picks a constructor, resolves each parameter recursively, builds the object and caches it according to the lifetime. This is inversion of control: the class no longer decides what it depends on.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning](../../../../PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

Extension-method registration.

#### How it works here

Method.

#### Why it matters here

Same `AddXxx` pattern as the other layers.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

None.

## Related Files

- [`src/TutoringCentre.Api/Http/ProblemResult.cs`](ProblemResult.cs.md)
- [`src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs`](GlobalExceptionHandler.cs.md)
- [`src/TutoringCentre.Api/Program.cs`](../Program.cs.md)
