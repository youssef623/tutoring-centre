# tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs

## Purpose

Hosts the real Api in-process against a throw-away PostgreSQL 17 container, for database-backed HTTP/CLI tests.

## Where It Fits

Api.Tests/Fixtures. Base class of `ConventionsFactory`; shared through `ApiCollection`. Depends on `Program`, Testcontainers, Respawn, `ApplyMigrationsAsync`.

## Walkthrough

`public class ApiFactory : WebApplicationFactory<Program>, IAsyncLifetime`. `InitializeAsync`: start container; `await Services.ApplyMigrationsAsync()` (accessing `Services` builds the host, after `ConfigureWebHost` has the connection string); open connection; `Respawner` (schemas `platform`, `identity`; ignore history table). `ConfigureWebHost`: `UseEnvironment("Testing")` (no Development auto-migrate, no user-secrets), `UseSetting("ConnectionStrings:Postgres", ConnectionString)` (visible before build, as `AddInfrastructure` reads it during registration), `UseDefaultServiceProvider(ValidateOnBuild = true, ValidateScopes = true)` (comment: Development enables these by default, Testing does not; missing registrations must fail tests). `ResetAsync`, `ScalarAsync<T>`, new `ExecuteAsync(sql)` (runs a non-query statement directly, e.g. revoking a membership under a live session); `DisposeAsync` disposes host then container. Additions to `ConfigureWebHost`: `UseSetting("Seed:Password", TestSeedPassword)` where `internal const string TestSeedPassword` is a throwaway test value, and `UseSetting("SessionValidation:CacheDuration", "00:00:00")` so every test re-checks the session instead of racing the cache window.

## Concepts Used

### In-process hosting with WebApplicationFactory

#### What it means

`WebApplicationFactory<Program>` starts the real application inside the test process with an in-memory test server, so real middleware, DI and routing run without opening a network port. Tests can override configuration and services before the host is built.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

In-process host.

#### How it works here

Class declaration.

#### Why it matters here

Real DI/middleware/routing without a network port.

### Testcontainers and Respawn

#### What it means

Testcontainers starts a disposable Docker container (here PostgreSQL 17) for the test run. Respawn deletes rows between tests, which is much faster than recreating the database.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests](../../../../PROJECT_OVERVIEW2.md#616-test-doubles-vs-real-database-tests).)

#### Where it appears in this file

Container + Respawn.

#### How it works here

Fields and init.

#### Why it matters here

Real database.

### Dependency Injection (DI) and the container

#### What it means

A class that needs a collaborator can create it itself (`new X()`), which hard-wires the choice, or it can *receive* it, usually as a constructor parameter. That second approach is dependency injection. A DI *container* stores *registrations* ("when asked for type A, build type B with lifetime L") and performs *resolution*: it picks a constructor, resolves each parameter recursively, builds the object and caches it according to the lifetime. This is inversion of control: the class no longer decides what it depends on.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning](../../../../PROJECT_OVERVIEW2.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

Strict DI validation.

#### How it works here

`UseDefaultServiceProvider`.

#### Why it matters here

Wiring mistakes fail loudly.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

Environment name `Testing`; sets `ConnectionStrings:Postgres` to the container string, `Seed:Password` to a throwaway test constant and `SessionValidation:CacheDuration` to zero.

## Gotchas and Issues

Two factory types (`ApiFactory`, `ConventionsFactory`) mean up to two containers per run; `preserveStaticLogger: true` in `Program.cs` exists for that.

## Related Files

- [`tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs`](ConventionsFactory.cs.md)
- [`tests/TutoringCentre.Api.Tests/Fixtures/ApiCollection.cs`](ApiCollection.cs.md)
- [`src/TutoringCentre.Api/Program.cs`](../../../src/TutoringCentre.Api/Program.cs.md)
