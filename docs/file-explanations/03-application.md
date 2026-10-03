# `src/TutoringCentre.Application`

Part of the [file index](INDEX.md). References: Domain; packages FluentValidation (+DI extensions), `Microsoft.Extensions.DependencyInjection.Abstractions`, `Microsoft.Extensions.Logging.Abstractions`. Contains **no** EF, ASP.NET or Npgsql (enforced by `DependencyRuleTests`). Concepts: overview §6.1–6.5, 6.7, 6.11.

## `DependencyInjection.cs`
`AddApplication(this IServiceCollection)`:
1. `AddScoped<CurrentActorContext>()`
2. `AddScoped<ICurrentActor>(p => p.GetRequiredService<CurrentActorContext>())` — alias to the same instance.
3. `AddCqrsHandlers(typeof(AssemblyMarker).Assembly)` — scan.
4. `AddScoped<Dispatcher>()`.
Called from `Program.cs` and `PostgresFixture.CreateServiceProvider`. `Dispatcher` is registered as a concrete class (no interface); the Api and tests depend on the concrete type.

## `Common/Cqrs/`
| File | Contents / notes |
| --- | --- |
| `ICommand.cs`, `IQuery.cs` | Empty marker interfaces carrying the response type as a generic parameter, so `SendAsync<TCommand,TResponse>` can constrain `TCommand : ICommand<TResponse>`. |
| `ICommandHandler.cs`, `IQueryHandler.cs` | `Task<Result<TResponse>> HandleAsync(TRequest, CancellationToken)`; `in` on the request type parameter (contravariant). |
| `Unit.cs` | `readonly record struct Unit` with `static readonly Unit Value` — the "no data" response. **Unused so far.** |
| `HandlerRegistration.cs` | `internal static` extension `AddCqrsHandlers`. `assembly.GetTypes().Where(type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false })`; for each implemented interface passing `IsHandlerInterface` (closed generic of `ICommandHandler<,>`/`IQueryHandler<,>`) → `services.AddScoped(interface, implementation)`. Then `AddValidatorsFromAssembly(assembly, includeInternalTypes: true)` (needed because validators are `internal`). A class implementing two handler interfaces would be registered for both. |
| `Dispatcher.cs` | See overview §6.3. Notable members: constructor null-guards; `private const string CommandKind/QueryKind` for log fields; `SendAsync` / `QueryAsync`; `ValidateAsync` (runs *all* validators sequentially, concatenates failures, groups by camelCased path, `Distinct` messages per field); `ToCamelCase`; `LogOutcome` (suppressions CA1848/CA1873 with written justification). Stopwatch uses `Stopwatch.GetTimestamp/GetElapsedTime` (no allocation). |

**Behaviours worth remembering:** validation failure returns *before* the handler is even resolved; the handler is resolved *before* the transaction starts; exceptions in `CommitAsync` are also caught by the `catch` → rollback no-op (the `UnitOfWork` already nulled the transaction) → rethrow; the `Result` returned to callers is the handler's own object, unchanged.

## `Common/Ports/`
- `IClock`: `UtcNow`; `ToLocal(DateTimeOffset, tzId)` → `DateTime` (Kind Unspecified); `FromLocal(DateTime, tzId)` → `DateTimeOffset` with zero offset. The only way product code may obtain "now" (comment). Implemented by `SystemClock`; `ToLocal/FromLocal` have no product callers yet.
- `IUnitOfWork`: four methods; `[SuppressMessage CA1716]` for the `readOnly` parameter name.

## `Common/Security/`
`Actor.cs` (abstract record + `SystemActor` + `AnonymousActor`), `ICurrentActor.cs` (read-only), `CurrentActorContext.cs` (private `_isSet` flag; `Set` null-guard + once-only). See overview §6.11 and §7.

## `Centres/`
- `ICentreRepository.cs` — `Task<bool> ExistsBySlugAsync(string, CancellationToken)`, `void Add(Centre)`.
- `Commands/CreateCentre/`
  - `CreateCentreCommand.cs` — `sealed record (Name, Slug, TimeZoneId, DefaultLocale) : ICommand<CreateCentreResult>`; comment: "Carries exactly the four fields a caller may provide" (no actor, no id — those come from context/domain).
  - `CreateCentreValidator.cs` — `AbstractValidator<CreateCentreCommand>`: `Name` NotEmpty+MaximumLength(120); `Slug` NotEmpty+Max(60); `TimeZoneId` NotEmpty+Max(64) (`TimeZoneIdMaxLength` const **duplicated** in `CentreConfiguration` — two constants of 64 that must be kept in sync by hand); `DefaultLocale.IsInEnum()`.
  - `CreateCentreHandler.cs` — `internal sealed class … (ICurrentActor currentActor, ICentreRepository centres)` (primary constructor): steps 1 authorize → 2 uniqueness → 3 `Centre.Create` → 4 `centres.Add`; returns `CreateCentreResult(Id, Slug)`.
  - `CreateCentreResult.cs` — `(Guid CentreId, string Slug)`.

## `Platform/`
- `ISystemInfoReadService.cs` — `GetSchemaStatusAsync` + `record SchemaStatus(string? LatestAppliedMigration, int PendingMigrationCount)`.
- `SystemInfoDto.cs` — `(string ApplicationVersion, string? LatestMigration, bool DatabaseUpToDate)`; comment lists what is deliberately excluded.
- `Queries/GetSystemInfo/GetSystemInfoQuery.cs` — `sealed record GetSystemInfoQuery : IQuery<SystemInfoDto>` (no members).
- `Queries/GetSystemInfo/GetSystemInfoHandler.cs` — maps status to DTO; `DatabaseUpToDate = PendingMigrationCount == 0`; `ReadApplicationVersion()` reads `AssemblyInformationalVersionAttribute` via reflection, returns `"unknown"` if missing, and **strips everything from `+`** (SourceLink build metadata such as a commit SHA). No authorization (documented as intentional).

## `AssemblyMarker.cs`
Used by `AddApplication` (`typeof(AssemblyMarker).Assembly` is the scanned assembly) and by architecture tests. Because it is `internal`, `InternalsVisibleTo` for `TutoringCentre.Architecture.Tests` and `TutoringCentre.Application.Tests` appears in the `.csproj` (the Application.Tests need access to `internal` handlers/validators).

## Tests that cover this layer
`Application.Tests` (22): `CreateCentreHandlerTests` (4), `CreateCentreValidatorTests` (3), `DispatcherTests` (9), `GetSystemInfoHandlerTests` (3), `CurrentActorContextTests` (3); fakes `FakeCentreRepository`, `FakeUnitOfWork`; `TestRequests.cs` defines test-only command/query/handlers/validators driven by `HandlerBehaviour.Mode` (`Succeed`/`Fail`/`Throw`). Integration coverage in `Infrastructure.Tests`.
