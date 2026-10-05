# `src/TutoringCentre.Api`

Folder map generated from the per-file explanations. Part of the [file index](INDEX.md); the teaching overview is [`../PROJECT_OVERVIEW2.md`](../PROJECT_OVERVIEW2.md). Each row links to the full explanation of that file (purpose, where it fits, walkthrough, concepts, flow, configuration, gotchas, related files). **28 files.**

## `src/TutoringCentre.Api`

| File | Purpose | Explanation |
| --- | --- | --- |
| `AssemblyMarker.cs` | Assembly handle for architecture tests. | [Explanation](./src/TutoringCentre.Api/AssemblyMarker.cs.md) |
| `Program.cs` | Application entry point and composition root: configures logging, services (including authentication, antiforgery, rate limiting and OpenAPI), runs the `seed` CLI mode or builds the HTTP pipeline, and starts the web host. | [Explanation](./src/TutoringCentre.Api/Program.cs.md) |
| `TutoringCentre.Api.csproj` | Project file for the web host: the composition root and HTTP edge, including build-time OpenAPI document generation. | [Explanation](./src/TutoringCentre.Api/TutoringCentre.Api.csproj.md) |
| `appsettings.Development.json` | Development-only overrides of configuration. | [Explanation](./src/TutoringCentre.Api/appsettings.Development.json.md) |
| `appsettings.json` | Base application configuration: logging levels, allowed hosts and the (empty) connection-string key. | [Explanation](./src/TutoringCentre.Api/appsettings.json.md) |

## `src/TutoringCentre.Api/Auth`

| File | Purpose | Explanation |
| --- | --- | --- |
| `ActorMiddleware.cs` | The single translation point from HTTP identity (session cookie claims) to Application identity (`StaffActor`). | [Explanation](./src/TutoringCentre.Api/Auth/ActorMiddleware.cs.md) |
| `AntiforgeryEndpointFilter.cs` | Endpoint filter applied once to the whole `/api` group that requires a valid antiforgery token on every unsafe request, login included. | [Explanation](./src/TutoringCentre.Api/Auth/AntiforgeryEndpointFilter.cs.md) |
| `AntiforgerySetup.cs` | Configures the ASP.NET Core antiforgery system: header name and the hardened `__Host-tcm.xsrf` cookie. | [Explanation](./src/TutoringCentre.Api/Auth/AntiforgerySetup.cs.md) |
| `AntiforgeryTokenResponse.cs` | Response record for `GET /api/auth/antiforgery`: just the request token string. | [Explanation](./src/TutoringCentre.Api/Auth/AntiforgeryTokenResponse.cs.md) |
| `AuthEndpoints.cs` | Maps the five session endpoints (antiforgery, login, logout, me, select centre) as thin transport around Application calls. | [Explanation](./src/TutoringCentre.Api/Auth/AuthEndpoints.cs.md) |
| `AuthenticationSetup.cs` | Registers cookie authentication, the fail-closed authorization fallback policy, the session-validation cache/options and Data Protection for the Api. | [Explanation](./src/TutoringCentre.Api/Auth/AuthenticationSetup.cs.md) |
| `LoginRateLimiting.cs` | Caps login attempts per client IP with a fixed-window limiter and answers rejections with Problem Details `auth.rate_limited` (429). | [Explanation](./src/TutoringCentre.Api/Auth/LoginRateLimiting.cs.md) |
| `LoginRequest.cs` | The login request model and its FluentValidation validator, kept with the endpoint because login is not a CQRS command. | [Explanation](./src/TutoringCentre.Api/Auth/LoginRequest.cs.md) |
| `RequestValidation.cs` | Converts a FluentValidation result into the same `validation.failed` Error the dispatcher produces, for requests that bypass the dispatcher. | [Explanation](./src/TutoringCentre.Api/Auth/RequestValidation.cs.md) |
| `SelectCentreRequest.cs` | The request body of `POST /api/session/centre`: only the centre id the user asks to switch into. | [Explanation](./src/TutoringCentre.Api/Auth/SelectCentreRequest.cs.md) |
| `SessionClaims.cs` | Defines the four session claim names and the factory that builds the principal stored in the encrypted cookie. | [Explanation](./src/TutoringCentre.Api/Auth/SessionClaims.cs.md) |
| `SessionRevalidationHandler.cs` | Re-checks a validated cookie's claims against the database so revoked access or a changed security stamp ends the session; valid results are cached briefly, invalid ones never. | [Explanation](./src/TutoringCentre.Api/Auth/SessionRevalidationHandler.cs.md) |
| `SessionValidationOptions.cs` | Typed options for the revalidation cache duration. | [Explanation](./src/TutoringCentre.Api/Auth/SessionValidationOptions.cs.md) |

## `src/TutoringCentre.Api/Cli`

| File | Purpose | Explanation |
| --- | --- | --- |
| `SeedCommand.cs` | The `seed` command: creates two demo centres through the dispatcher as the system actor, then seeds the development staff accounts and memberships. | [Explanation](./src/TutoringCentre.Api/Cli/SeedCommand.cs.md) |

## `src/TutoringCentre.Api/Endpoints`

| File | Purpose | Explanation |
| --- | --- | --- |
| `PlatformEndpoints.cs` | Maps `GET /api/system/info`, the only non-authentication product endpoint, as bind -> dispatch -> map with no logic. | [Explanation](./src/TutoringCentre.Api/Endpoints/PlatformEndpoints.cs.md) |

## `src/TutoringCentre.Api/Http`

| File | Purpose | Explanation |
| --- | --- | --- |
| `CorrelationIdMiddleware.cs` | Gives every request a validated correlation ID, exposes it in the response header and in every log line. | [Explanation](./src/TutoringCentre.Api/Http/CorrelationIdMiddleware.cs.md) |
| `GlobalExceptionHandler.cs` | The single place unexpected exceptions are logged and converted into Problem Details responses without leaking internals. | [Explanation](./src/TutoringCentre.Api/Http/GlobalExceptionHandler.cs.md) |
| `HttpContextKeys.cs` | Holds the string key under which the correlation ID is stored in `HttpContext.Items`. | [Explanation](./src/TutoringCentre.Api/Http/HttpContextKeys.cs.md) |
| `ProblemDetailsSetup.cs` | Registers Problem Details generation for framework-produced errors and wires the global exception handler. | [Explanation](./src/TutoringCentre.Api/Http/ProblemDetailsSetup.cs.md) |
| `ProblemResult.cs` | The single writer of error bodies: serialises a `ProblemDetails` as `application/problem+json` with `traceId` and `correlationId`. | [Explanation](./src/TutoringCentre.Api/Http/ProblemResult.cs.md) |
| `ResultHttpExtensions.cs` | The only place domain `Result`/`Error` values become HTTP responses. | [Explanation](./src/TutoringCentre.Api/Http/ResultHttpExtensions.cs.md) |

## `src/TutoringCentre.Api/Logging`

| File | Purpose | Explanation |
| --- | --- | --- |
| `SensitiveDataDestructuringPolicy.cs` | Serilog safety net that masks properties named like Password/Token/Secret/ConnectionString/Phone* when an object is logged with `{@Obj}`. | [Explanation](./src/TutoringCentre.Api/Logging/SensitiveDataDestructuringPolicy.cs.md) |

## `src/TutoringCentre.Api/Properties`

| File | Purpose | Explanation |
| --- | --- | --- |
| `launchSettings.json` | Local-development launch profiles for `dotnet run` and IDEs; the `https` profile is the one the frontend, the E2E suite and the README use. | [Explanation](./src/TutoringCentre.Api/Properties/launchSettings.json.md) |
