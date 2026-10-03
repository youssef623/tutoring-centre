# src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj

## Purpose
Project file for the adapters layer. It references Application and Domain, and brings in EF Core, the Npgsql provider, snake_case naming, the Npgsql health check and configuration abstractions.

## Where it fits
Infrastructure layer. Referenced by Api (composition root only), Infrastructure.Tests and Architecture.Tests. `ProjectReferenceTests.Infrastructure_ReferencesOnlyApplicationAndDomain` asserts the reference list.

## Walkthrough
- **Lines 3–4:** project references to Application and Domain.
- **Lines 7–11:** packages:
  - `AspNetCore.HealthChecks.NpgSql`
  - `EFCore.NamingConventions`
  - `Microsoft.EntityFrameworkCore`
  - `Microsoft.Extensions.Configuration.Abstractions`
  - `Npgsql.EntityFrameworkCore.PostgreSQL`
- **Line 14:** `InternalsVisibleTo` Architecture.Tests.

## Concepts used
- **Adapters own the third-party dependencies:** only this project knows about Postgres and EF Core.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Infrastructure.Tests has no `InternalsVisibleTo`, so `internal` `UnitOfWork`, `TimestampInterceptor` and `Schemas` can't be tested directly. Only the public `SystemClock` is tested.

## Related files
- [DependencyInjection.cs](DependencyInjection.cs.md)
- [Directory.Packages.props](../../Directory.Packages.props.md)
