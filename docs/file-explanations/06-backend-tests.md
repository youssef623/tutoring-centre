# Backend test projects (`tests/`)

Part of the [file index](INDEX.md). Five xUnit projects, all `IsPackable=false`, each adding `<Using Include="Xunit" />` (global using) and `coverlet.collector` (private asset). **I could not execute these in my sandbox (no .NET SDK); descriptions come from reading the code.** Concepts: overview §6.15–6.16, §10.

| Project | References | Docker? |
| --- | --- | --- |
| `Domain.Tests` | Domain | no |
| `Application.Tests` | Application (+ `Microsoft.Extensions.DependencyInjection`) | no |
| `Architecture.Tests` | all four `src` projects, NetArchTest | no |
| `Infrastructure.Tests` | Infrastructure (+ Configuration, DI, Logging, TimeProvider.Testing, Respawn, Testcontainers) | yes |
| `Api.Tests` (`Sdk.Web`) | Api (+ Mvc.Testing, Respawn, Testcontainers) | yes, except `HealthEndpointTests`, `ResultHttpExtensionsTests`, `SensitiveDataDestructuringPolicyTests` |

## Domain.Tests
- `Centres/CentreTests.cs` — 7 facts + 1 theory (4 cases). Helper `AssertValidationFailure(result, code)` also asserts `ErrorKind.Validation`.
- `Common/EntityTests.cs` — private nested `TestEntity : Entity` to test the abstract base.
- `Common/ErrorTests.cs`, `Common/ResultTests.cs` — see overview §6.4. `Failure(null!)` deliberately passes null past nullable annotations.

## Application.Tests
- `Cqrs/DispatcherTests.cs` — builds a real `ServiceCollection` per test (`CreateSut(HandlerMode)`) registering the `FakeUnitOfWork` singleton, a `HandlerBehaviour` singleton and test handlers/validators, and constructs `Dispatcher` with `NullLogger`. Asserts the exact `Calls` list (e.g. `["Begin(rw)","Handle","Save","Commit"]`). The nested `Fixture` is `IDisposable` to dispose the provider. `SendAsync_SeveralFieldFailures…` checks camelCase keys `name`,`slug` and message grouping (two validators on `Name`).
- `Cqrs/TestRequests.cs` — test-only types; `TestCommandHandler` switches on `HandlerMode` (`throw`/`Fail`/`Succeed`) using a switch expression where the `throw` is an expression.
- `Fakes/FakeUnitOfWork.cs` — records calls in order; `ThrowOnSave` simulates case C5.
- `Fakes/FakeCentreRepository.cs` — `Existing` (pre-seeded rows) + `Added`; `ExistsBySlugAsync` searches both.
- `Centres/CreateCentreHandlerTests.cs` — anonymous actor ⇒ Forbidden (with an existing duplicate slug in the repo, proving authorization runs *before* the uniqueness check); duplicate ⇒ Conflict; invalid slug ⇒ domain error `centre.slug_invalid`; success ⇒ one `Added`, ids match.
- `Centres/CreateCentreValidatorTests.cs`, `Security/CurrentActorContextTests.cs`, `Platform/GetSystemInfoHandlerTests.cs` (private fake read service).

## Architecture.Tests
`DependencyRuleTests`, `ProjectReferenceTests`, `ProjectFiles` — overview §6.15. The project refs to all four `src` projects exist so the assemblies load and the `AssemblyMarker`s are reachable.

## Infrastructure.Tests
- **Fixtures:** `PostgresFixture` (container, migrate, Respawn with `SchemasToInclude ["platform","identity"]` and `TablesToIgnore platform.__ef_migrations_history`, `CreateServiceProvider(configure?)` builds a *production-identical* container via `AddLogging().AddApplication().AddInfrastructure(config)` with `ValidateOnBuild/ValidateScopes`, SQL helpers `ScalarAsync<T>` and `CountCentresAsync`); `PostgresCollection` (collection name `"postgres"`); `PostgresTestBase` (`[Collection]`, resets DB in `InitializeAsync`); `DispatchExtensions` (`SendAsAsync`/`QueryAsAsync` — new scope, `Set(actor)`, dispatch).
- **Tests:** `CreateCentreTests` (4), `CentreSlugRaceTests` (1; `TaskCompletionSource` gate; `Outcome` enum; catches `DbUpdateException` with inner `PostgresException { SqlState: "23505" }`), `MigrationTests` (4: table, unique index definition via `pg_indexes`, CHECK via `information_schema`, history table in `platform`), `ReadOnlyQueryTests` (raw `INSERT` in a query handler ⇒ SQLSTATE `25006`), `TransactionBoundaryTests` (3, handler `WriteThenFailHandler`), `SystemInfoQueryTests` (latest migration ends with `_InitialPlatform`, up to date), `SystemClockTests` (3, `FakeTimeProvider`).
- Test-only handlers (`SneakyWriteQueryHandler`, `WriteThenFailHandler`) are `internal` and registered through `CreateServiceProvider(configure)`, so they never exist in production registration.

## Api.Tests
- **Fixtures:** `ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime` (container, `Services.ApplyMigrationsAsync()`, Respawn, `ConfigureWebHost` → env `Testing`, `UseSetting("ConnectionStrings:Postgres", …)`, strict DI validation); `ApiCollection` (`"api"`); `ConventionsFactory : ApiFactory` adds `ConfigureTestServices` registrations (handlers, validator, `InMemoryLogSink` as `ILogEventSink`, `ConventionEndpointsStartupFilter`); `ConventionsCollection` (`"conventions"`); `ConventionEndpoints` (+ startup filter) and `ConventionTestTypes` (`ConventionCommandHandler` produces each error kind from the `{kind}` route value; `"throws"` throws with a message containing a planted secret); `InMemoryLogSink` (thread-safe queue, `WaitForAsync` polling up to 5 s because the request-completion log line is written just after the response is sent).
- **Tests:** `HealthEndpointTests` (own `WebApplicationFactory`, unreachable DB `Host=127.0.0.1;Port=1;…;Timeout=2`), `ReadinessWithDatabaseTests`, `CorrelationIdTests` (6), `LoggingConventionTests` (2), `ProblemDetailsTests` (19 methods), `ResultHttpExtensionsTests` (6; executes `IResult` against a `DefaultHttpContext` with a `MemoryStream` body), `SensitiveDataDestructuringPolicyTests` (3; builds a `LoggerConfiguration` with a `DelegateSink`), `SeedCommandTests` (1).
- Two separate factories (`ApiFactory`, `ConventionsFactory`) mean **two PostgreSQL containers** may run in one test session; the `preserveStaticLogger: true` comment in `Program.cs` exists because of this.

## Conventions seen across the suite
Method naming `Method_Condition_Result`; `// Arrange / Act / Assert` comments in many (not all) tests; `Assert.*` only (no FluentAssertions); `new Uri("/path", UriKind.Relative)` to satisfy analyzer CA2234; justified `[SuppressMessage]` on test infrastructure (CA1711 collection suffix, CA1812).
