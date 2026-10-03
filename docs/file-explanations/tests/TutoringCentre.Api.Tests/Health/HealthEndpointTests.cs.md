# tests/TutoringCentre.Api.Tests/Health/HealthEndpointTests.cs

## Purpose
HTTP-level tests of the two health endpoints, run against the **real** `Program.cs` hosted in-process. They check that liveness is always 200, and that readiness returns 503 when the database is unreachable.

## Where it fits
Api.Tests. It exercises `Program.cs` and `Infrastructure/DependencyInjection.cs` (the `AddNpgSql` branch).

## Walkthrough
- **Lines 10–11:** `UnreachableDatabase = "Host=127.0.0.1;Port=1;...;Timeout=2"`. Nothing listens on port 1, so the connection is refused immediately; `Timeout=2` caps the wait where the OS retries.
- **Lines 13–25, `Liveness_ReturnsOk`:** `new WebApplicationFactory<Program>()`, `CreateClient()`, `GET /health`, assert 200. There's no config override, so it proves liveness never depends on the DB.
- **Lines 27–41, `Readiness_WhenDatabaseUnreachable_Returns503`:**
  - `baseFactory.WithWebHostBuilder(b => b.UseSetting("ConnectionStrings:Postgres", UnreachableDatabase))`, then `GET /health/ready`, assert 503.
  - `UseSetting` is used because the value must be visible before `Build()` (explained in the old `tests/.semantic.md:25`).
- `await using` and `using` dispose the factory, client and response.

## Concepts used
- **`WebApplicationFactory<TEntryPoint>`:** boots the app in memory with a `TestServer`.
- **Configuration override in tests.**
- **Arrange/Act/Assert.**

## Data and control flow
Test → in-memory HTTP → health middleware → Npgsql check → refused → 503.

## Configuration and environment
Overrides `ConnectionStrings:Postgres`. The liveness test reads whatever `appsettings` and user-secrets exist on the machine.

## Gotchas and issues
- **Happy path untested.** There's no test for readiness *success*; it needs a real DB.
- **Blank-string branch untested.** There's no test for the "not configured" branch either. It also returns 503, which is exactly why the old docs stress that this test must use an unreachable but non-empty string.
- Each test builds its own host. Fine for 2 tests; a shared fixture would help as the suite grows.

## Related files
- [Program.cs](../../../src/TutoringCentre.Api/Program.cs.md)
- [Infrastructure DependencyInjection.cs](../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs.md)
- [tests/.semantic.md](../../.semantic.md.md)
