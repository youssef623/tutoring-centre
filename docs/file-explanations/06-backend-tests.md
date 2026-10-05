# `tests/` (backend test projects)

Folder map generated from the per-file explanations. Part of the [file index](INDEX.md); the teaching overview is [`../PROJECT_OVERVIEW2.md`](../PROJECT_OVERVIEW2.md). Each row links to the full explanation of that file (purpose, where it fits, walkthrough, concepts, flow, configuration, gotchas, related files). **65 files.**

## `tests`

| File | Purpose | Explanation |
| --- | --- | --- |
| `.semantic.md` | Generated description of the five test projects at the skeleton stage. | [Explanation](./tests/.semantic.md.md) |

## `tests/TutoringCentre.Api.Tests`

| File | Purpose | Explanation |
| --- | --- | --- |
| `TutoringCentre.Api.Tests.csproj` | Project file for HTTP-level tests that host the real Api in-process. | [Explanation](./tests/TutoringCentre.Api.Tests/TutoringCentre.Api.Tests.csproj.md) |

## `tests/TutoringCentre.Api.Tests/Auth`

| File | Purpose | Explanation |
| --- | --- | --- |
| `LoginFlowTests.cs` | HTTP-level tests of the happy paths of authentication: login, automatic and manual centre selection, re-login while signed in, logout and the session after logout. | [Explanation](./tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs.md) |
| `SecurityTests.cs` | Proves that the attacks the authentication design is meant to close are actually closed: user enumeration, CSRF, cookie tampering, stale and revoked sessions, tenant escalation, password spraying, lockout and forgotten authorization. | [Explanation](./tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs.md) |

## `tests/TutoringCentre.Api.Tests/Cli`

| File | Purpose | Explanation |
| --- | --- | --- |
| `SeedCommandTests.cs` | Proves the seed command is idempotent and creates exactly the two demo centres and the development staff set. | [Explanation](./tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs.md) |

## `tests/TutoringCentre.Api.Tests/Fixtures`

| File | Purpose | Explanation |
| --- | --- | --- |
| `AntiforgeryTestHelper.cs` | Test helper that fetches a real antiforgery token and attaches it (plus a per-call rate-limit partition) to every POST, so tests exercise the CSRF protection instead of bypassing it. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs.md) |
| `ApiCollection.cs` | xUnit collection for tests sharing `ApiFactory`. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ApiCollection.cs.md) |
| `ApiFactory.cs` | Hosts the real Api in-process against a throw-away PostgreSQL 17 container, for database-backed HTTP/CLI tests. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ApiFactory.cs.md) |
| `ConventionEndpoints.cs` | Defines the test-only `/api/test/*` endpoints used to trigger each error kind, strict-JSON failures and sensitive-data logging. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpoints.cs.md) |
| `ConventionEndpointsStartupFilter.cs` | `IStartupFilter` that appends the test-only endpoints after the real middleware pipeline. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ConventionEndpointsStartupFilter.cs.md) |
| `ConventionTestTypes.cs` | Test-only commands, handlers and validator that produce every error kind on demand. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ConventionTestTypes.cs.md) |
| `ConventionsCollection.cs` | xUnit collection for tests sharing `ConventionsFactory`. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ConventionsCollection.cs.md) |
| `ConventionsFactory.cs` | The real Api plus test-only endpoints, handlers and an in-memory log sink, used to test HTTP conventions. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/ConventionsFactory.cs.md) |
| `InMemoryLogSink.cs` | Serilog sink that stores log events in memory so tests can assert on logging. | [Explanation](./tests/TutoringCentre.Api.Tests/Fixtures/InMemoryLogSink.cs.md) |

## `tests/TutoringCentre.Api.Tests/Health`

| File | Purpose | Explanation |
| --- | --- | --- |
| `HealthEndpointTests.cs` | Tests liveness and readiness endpoints without a database. | [Explanation](./tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs.md) |
| `ReadinessWithDatabaseTests.cs` | Tests that readiness returns 200 when a real database is reachable. | [Explanation](./tests/TutoringCentre.Api.Tests/Health/ReadinessWithDatabaseTests.cs.md) |

## `tests/TutoringCentre.Api.Tests/Http`

| File | Purpose | Explanation |
| --- | --- | --- |
| `CorrelationIdTests.cs` | Tests the correlation-ID middleware's generation, validation and propagation. | [Explanation](./tests/TutoringCentre.Api.Tests/Http/CorrelationIdTests.cs.md) |
| `LoggingConventionTests.cs` | Tests log levels for successful and failing requests. | [Explanation](./tests/TutoringCentre.Api.Tests/Http/LoggingConventionTests.cs.md) |
| `ProblemDetailsTests.cs` | Tests the HTTP error conventions end to end through the real pipeline. | [Explanation](./tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs.md) |
| `ResultHttpExtensionsTests.cs` | Unit tests of the `Result` -> HTTP mapping without starting a host. | [Explanation](./tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs.md) |

## `tests/TutoringCentre.Api.Tests/Logging`

| File | Purpose | Explanation |
| --- | --- | --- |
| `SensitiveDataDestructuringPolicyTests.cs` | Tests that the policy masks sensitive properties and leaves others alone. | [Explanation](./tests/TutoringCentre.Api.Tests/Logging/SensitiveDataDestructuringPolicyTests.cs.md) |

## `tests/TutoringCentre.Application.Tests`

| File | Purpose | Explanation |
| --- | --- | --- |
| `TutoringCentre.Application.Tests.csproj` | Project file for Application unit tests; references Application and the DI package used to build a service provider in `DispatcherTests`. | [Explanation](./tests/TutoringCentre.Application.Tests/TutoringCentre.Application.Tests.csproj.md) |

## `tests/TutoringCentre.Application.Tests/Centres`

| File | Purpose | Explanation |
| --- | --- | --- |
| `CreateCentreHandlerTests.cs` | Unit tests for `CreateCentreHandler` branches using fakes. | [Explanation](./tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs.md) |
| `CreateCentreValidatorTests.cs` | Unit tests for `CreateCentreValidator`. | [Explanation](./tests/TutoringCentre.Application.Tests/Centres/CreateCentreValidatorTests.cs.md) |

## `tests/TutoringCentre.Application.Tests/Cqrs`

| File | Purpose | Explanation |
| --- | --- | --- |
| `DispatcherTests.cs` | Tests every branch of the dispatcher pipeline using a call-recording fake unit of work. | [Explanation](./tests/TutoringCentre.Application.Tests/Cqrs/DispatcherTests.cs.md) |
| `TestRequests.cs` | Test-only commands, queries, handlers and validators used to drive `Dispatcher` in isolation. | [Explanation](./tests/TutoringCentre.Application.Tests/Cqrs/TestRequests.cs.md) |

## `tests/TutoringCentre.Application.Tests/Fakes`

| File | Purpose | Explanation |
| --- | --- | --- |
| `FakeCentreRepository.cs` | In-memory `ICentreRepository` with pre-seeded rows and a record of additions. | [Explanation](./tests/TutoringCentre.Application.Tests/Fakes/FakeCentreRepository.cs.md) |
| `FakeMembershipReadService.cs` | In-memory fake of `IMembershipReadService` that returns whatever the test assigned to three properties. | [Explanation](./tests/TutoringCentre.Application.Tests/Fakes/FakeMembershipReadService.cs.md) |
| `FakeUnitOfWork.cs` | In-memory `IUnitOfWork` that records every call in order. | [Explanation](./tests/TutoringCentre.Application.Tests/Fakes/FakeUnitOfWork.cs.md) |

## `tests/TutoringCentre.Application.Tests/Identity`

| File | Purpose | Explanation |
| --- | --- | --- |
| `GetActiveMembershipHandlerTests.cs` | Tests the tenant gate query handler: anonymous actor, no active membership, active membership. | [Explanation](./tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs.md) |
| `GetMyMembershipsHandlerTests.cs` | Tests the handler behind `GET /api/me`: anonymous actor, unknown profile, and how the active centre/role are reported or dropped. | [Explanation](./tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs.md) |
| `ValidateStaffSessionHandlerTests.cs` | Tests the session-revalidation rule: unknown user, wrong security stamp, inactive membership, and the two valid cases. | [Explanation](./tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs.md) |

## `tests/TutoringCentre.Application.Tests/Platform`

| File | Purpose | Explanation |
| --- | --- | --- |
| `GetSystemInfoHandlerTests.cs` | Unit tests for `GetSystemInfoHandler` with a private fake read service. | [Explanation](./tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs.md) |

## `tests/TutoringCentre.Application.Tests/Security`

| File | Purpose | Explanation |
| --- | --- | --- |
| `CurrentActorContextTests.cs` | Tests the actor holder's default and set-once behaviour. | [Explanation](./tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs.md) |

## `tests/TutoringCentre.Architecture.Tests`

| File | Purpose | Explanation |
| --- | --- | --- |
| `ApiRuleTests.cs` | Architecture rules for the Api project: endpoints must not touch Infrastructure, repositories or domain entities, and only `Program` and the CLI may reference Infrastructure. | [Explanation](./tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs.md) |
| `ArchitectureSupport.cs` | Shared helpers for the architecture tests: recognising repository interfaces/classes by name and describing a failing result. | [Explanation](./tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs.md) |
| `CqrsRuleTests.cs` | Reflection-based architecture rules for CQRS: exactly one handler per request, handlers sealed and non-public, query handlers never depend on repositories. | [Explanation](./tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs.md) |
| `DependencyRuleTests.cs` | Type-level architecture tests: fail when compiled code in a layer uses a forbidden namespace. | [Explanation](./tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md) |
| `IdentityRuleTests.cs` | Architecture rules that isolate ASP.NET Core Identity: Application and the Api (except Program and the CLI) must not depend on Identity types or on `TutoringCentre.Infrastructure.Identity`. | [Explanation](./tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs.md) |
| `ProjectFiles.cs` | Helper that reads the `ProjectReference` items of a `src` project from its `.csproj` XML. | [Explanation](./tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs.md) |
| `ProjectReferenceTests.cs` | Declared-reference architecture tests: each `src` project's `ProjectReference` list must equal the allowed set. | [Explanation](./tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md) |
| `RepositoryRuleTests.cs` | Architecture rules about repository placement: `I*Repository` interfaces live in Application, `*Repository` classes live in Infrastructure. | [Explanation](./tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs.md) |
| `SourceAssemblies.cs` | Single place that names the four production assemblies for architecture tests, obtained from marker types. | [Explanation](./tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs.md) |
| `TutoringCentre.Architecture.Tests.csproj` | Project file for the architecture tests; references all four `src` projects and the NetArchTest library. | [Explanation](./tests/TutoringCentre.Architecture.Tests/TutoringCentre.Architecture.Tests.csproj.md) |

## `tests/TutoringCentre.Domain.Tests`

| File | Purpose | Explanation |
| --- | --- | --- |
| `TutoringCentre.Domain.Tests.csproj` | Project file for the Domain unit tests; references only the Domain project. | [Explanation](./tests/TutoringCentre.Domain.Tests/TutoringCentre.Domain.Tests.csproj.md) |

## `tests/TutoringCentre.Domain.Tests/Centres`

| File | Purpose | Explanation |
| --- | --- | --- |
| `CentreTests.cs` | Unit tests for the `Centre.Create` business rules. | [Explanation](./tests/TutoringCentre.Domain.Tests/Centres/CentreTests.cs.md) |

## `tests/TutoringCentre.Domain.Tests/Common`

| File | Purpose | Explanation |
| --- | --- | --- |
| `EntityTests.cs` | Tests the `Entity` base class's identity generation. | [Explanation](./tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs.md) |
| `ErrorTests.cs` | Tests the two `Error.Validation` overloads. | [Explanation](./tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs.md) |
| `ResultTests.cs` | Tests `Result`/`Result<T>` invariants. | [Explanation](./tests/TutoringCentre.Domain.Tests/Common/ResultTests.cs.md) |

## `tests/TutoringCentre.Domain.Tests/Identity`

| File | Purpose | Explanation |
| --- | --- | --- |
| `MembershipTests.cs` | Unit tests of the `Membership` entity: creation rules and the Activate/Deactivate state transitions. | [Explanation](./tests/TutoringCentre.Domain.Tests/Identity/MembershipTests.cs.md) |

## `tests/TutoringCentre.Infrastructure.Tests`

| File | Purpose | Explanation |
| --- | --- | --- |
| `TutoringCentre.Infrastructure.Tests.csproj` | Project file for database-backed Infrastructure tests. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/TutoringCentre.Infrastructure.Tests.csproj.md) |

## `tests/TutoringCentre.Infrastructure.Tests/Centres`

| File | Purpose | Explanation |
| --- | --- | --- |
| `CentreSlugRaceTests.cs` | Proves that two simultaneous creates with the same slug leave exactly one row, because the unique index is the real guarantee. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Centres/CentreSlugRaceTests.cs.md) |
| `CreateCentreTests.cs` | Integration tests of the create-centre use case through the real dispatcher and PostgreSQL. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs.md) |

## `tests/TutoringCentre.Infrastructure.Tests/Fixtures`

| File | Purpose | Explanation |
| --- | --- | --- |
| `DispatchExtensions.cs` | Test helpers that dispatch a command or query in a fresh scope as a chosen actor. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Fixtures/DispatchExtensions.cs.md) |
| `PostgresCollection.cs` | xUnit collection definition that makes all database tests share one `PostgresFixture`. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresCollection.cs.md) |
| `PostgresFixture.cs` | Shared test fixture: starts one PostgreSQL 17 container per test run, applies the real migrations, builds a production-identical DI container and resets data between tests with Respawn. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresFixture.cs.md) |
| `PostgresTestBase.cs` | Base class for database tests: puts the class into the postgres collection and resets all rows before each test. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Fixtures/PostgresTestBase.cs.md) |

## `tests/TutoringCentre.Infrastructure.Tests/Identity`

| File | Purpose | Explanation |
| --- | --- | --- |
| `IdentityAuthenticationServiceTests.cs` | Proves the credential adapter against real PostgreSQL: correct and wrong passwords, unknown email, lockout after five failures, counter reset, and that the email is never logged. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Identity/IdentityAuthenticationServiceTests.cs.md) |
| `MembershipConstraintTests.cs` | Proves that PostgreSQL itself enforces the identity schema: exactly five identity tables, unique (centre, user) membership, foreign key to centres, and no deleting a centre that has members. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipConstraintTests.cs.md) |
| `MembershipReadServiceTests.cs` | Proves the membership read service against real PostgreSQL: who sees which centres, that inactive members see none, and that nobody sees another user's memberships. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Identity/MembershipReadServiceTests.cs.md) |

## `tests/TutoringCentre.Infrastructure.Tests/Persistence`

| File | Purpose | Explanation |
| --- | --- | --- |
| `MigrationTests.cs` | Proves a brand-new database built only from the migrations has the expected objects. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Persistence/MigrationTests.cs.md) |
| `ReadOnlyQueryTests.cs` | Proves the database itself refuses writes made inside a query handler. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Persistence/ReadOnlyQueryTests.cs.md) |
| `TransactionBoundaryTests.cs` | Proves rollback on exception and on failure results, with a success control. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs.md) |

## `tests/TutoringCentre.Infrastructure.Tests/Platform`

| File | Purpose | Explanation |
| --- | --- | --- |
| `SystemInfoQueryTests.cs` | End-to-end test of `GetSystemInfoQuery` against a migrated database. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Platform/SystemInfoQueryTests.cs.md) |

## `tests/TutoringCentre.Infrastructure.Tests/Time`

| File | Purpose | Explanation |
| --- | --- | --- |
| `SystemClockTests.cs` | Unit tests for `SystemClock` using `FakeTimeProvider`. | [Explanation](./tests/TutoringCentre.Infrastructure.Tests/Time/SystemClockTests.cs.md) |
