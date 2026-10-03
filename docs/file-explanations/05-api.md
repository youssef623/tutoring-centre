# `src/TutoringCentre.Api`

Part of the [file index](INDEX.md). `Microsoft.NET.Sdk.Web` project (brings the ASP.NET Core shared framework). References Application + Infrastructure (Infrastructure only for `AddInfrastructure`). Packages: `Serilog.AspNetCore`; `Microsoft.EntityFrameworkCore.Design` with `PrivateAssets=all` (design-time only, not flowed to dependents). `UserSecretsId a0748b74-…` enables `dotnet user-secrets`. `InternalsVisibleTo TutoringCentre.Architecture.Tests`. Concepts: overview §5, §6.12, §6.13, §8.

## `Program.cs`
Annotated in overview §5. Key language features: **top-level statements** (no `Main`); **list pattern** `args is ["seed"]`; `return await …` / `return 0` give the process exit code; `public partial class Program;` at the end exposes the generated class. Parts:
- Serilog block (`ReadFrom.Configuration`, `ReadFrom.Services`, `Enrich.FromLogContext`, `Destructure.With<SensitiveDataDestructuringPolicy>`, `WriteTo.Console(new RenderedCompactJsonFormatter())`, `preserveStaticLogger: true`).
- Services: `AddHealthChecks`, `AddApplication().AddInfrastructure(configuration)`, `AddApiProblemDetails`, JSON options, `RouteHandlerOptions.ThrowOnBadRequest`.
- `seed` branch; dev auto-migrate; four middleware; two health endpoints; `/api` group; fallback.
- Request-logging level function: exception or `>= 500` → `Error`; `/health*` → `Verbose`; else `Information`.

## `Cli/SeedCommand.cs`
`public static class SeedCommand` with `RunAsync(IServiceProvider)` → exit code. A private record `SeedCentre` and a static array of two demo centres. For each: `await using var scope = services.CreateAsyncScope()` → `GetRequiredService<CurrentActorContext>().Set(new SystemActor(null))` → `GetRequiredService<Dispatcher>()` → `SendAsync<CreateCentreCommand, CreateCentreResult>`. Outcome handling: success → log "created"; `centre.slug_taken` → log "already exists" (idempotent); otherwise log error and set exit code 1. Uses `CancellationToken.None` and `ILoggerFactory` (not `ILogger<T>`, because it's a static class). Two analyzer suppressions explained in code.

## `Endpoints/PlatformEndpoints.cs`
Extension on `RouteGroupBuilder`: `api.MapGet("/system/info", GetSystemInfoAsync).WithName("GetSystemInfo").Produces<SystemInfoDto>().ProducesProblem(500).AllowAnonymous()`. The handler is a static method taking `(Dispatcher, CancellationToken)`; body is two lines. Route becomes `GET /api/system/info` because `Program.cs` maps this onto the `/api` group. Response JSON uses ASP.NET's web defaults (camelCase): `applicationVersion`, `latestMigration`, `databaseUpToDate` — matching `frontend/README.md`'s table. **No frontend code calls this yet.**

## `Http/`
| File | Details |
| --- | --- |
| `CorrelationIdMiddleware.cs` | Convention-based middleware (`InvokeAsync(HttpContext)` + constructor takes `RequestDelegate next`). `HeaderName` is public const `X-Correlation-Id`. `IsValid`: length 1–64 and every char `char.IsAsciiLetterOrDigit` or `.`/`_`/`-`. Generated id: `Guid.NewGuid().ToString("N")` (32 hex chars). |
| `HttpContextKeys.cs` | `internal static class` with const `CorrelationId = "CorrelationId"`. (A test sets `context.Items["CorrelationId"]` with the literal string — coupling by value.) |
| `ProblemResult.cs` | `internal sealed class ProblemResult(ProblemDetails problem) : IResult`. `ExecuteAsync`: enrich, set status (default 500 if null), `WriteAsJsonAsync(problem, problem.GetType(), options: null, contentType: "application/problem+json", ct)` — passing `problem.GetType()` makes derived types (`ValidationProblemDetails`) serialise their extra members. `Enrich` is `internal static` and also used as the `CustomizeProblemDetails` callback. |
| `ResultHttpExtensions.cs` | `ToHttpResult<T>(this Result<T>, Func<T,IResult> onSuccess)`; `ToHttpResult(this Result)` → 204/problem; `ToProblemResult(this Error)` with the kind→status/title switch (throws for unmapped kinds) and `ValidationProblemDetails` when `Kind == Validation && Fields != null`. `Detail` = `error.Message`; extension `code`. |
| `GlobalExceptionHandler.cs` | `internal sealed partial class … : IExceptionHandler`; two `[LoggerMessage]` source-generated methods (compile-time log delegates: no boxing/parsing at runtime). Returns `true` (handled) after writing. |
| `ProblemDetailsSetup.cs` | `AddApiProblemDetails`: `AddProblemDetails(CustomizeProblemDetails = ctx => ProblemResult.Enrich(ctx.ProblemDetails, ctx.HttpContext))` and `AddExceptionHandler<GlobalExceptionHandler>()`. |

## `Logging/SensitiveDataDestructuringPolicy.cs`
`public sealed class … : IDestructuringPolicy`. `TryDestructure`: returns `false` (let Serilog handle) for `IEnumerable` values (strings, dictionaries, lists), and for objects with no sensitive property. Otherwise reflects public readable instance properties (skipping indexers), replaces sensitive ones with `ScalarValue("***")`, and others via `propertyValueFactory.CreatePropertyValue(value, destructureObjects: true)`; returns a `StructureValue` named after the type. Sensitive = name starts with `Phone` or contains `Password|Token|Secret|ConnectionString` (case-insensitive). Reflection per call — acceptable for a safety net, not a hot path. Not covered: **fields**, nested sensitive properties inside types the factory destructures with other policies (it passes `destructureObjects: true`, so nested objects go through policies again), and a property named e.g. `Pin` or `Email`.

## Configuration files
- `appsettings.json`: `Logging` (legacy Microsoft logging section; Serilog ignores it) and `Serilog.MinimumLevel` (Default Information; override `Microsoft.AspNetCore` Warning); `AllowedHosts: "*"`; `ConnectionStrings.Postgres: ""`.
- `appsettings.Development.json`: `Logging` section only, same values — **no effect on Serilog** (which reads the `Serilog` section), so it is effectively redundant today.
- `Properties/launchSettings.json`: profiles `http` (`http://localhost:5080`, matches the Vite proxy target and README) and `https` (`https://localhost:7197;http://localhost:5245`); both `launchBrowser: true`, Development environment.

## `AssemblyMarker.cs`
Test handle.

## Tests
`Api.Tests` (40 methods): see `06-backend-tests.md`.
