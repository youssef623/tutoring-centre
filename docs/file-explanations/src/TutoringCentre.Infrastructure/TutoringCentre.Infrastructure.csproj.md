# src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj

## Purpose

Project file for the adapter layer: references Application and Domain and brings in EF Core, the Npgsql provider and the health check.

## Where It Fits

Referenced by Api, `Infrastructure.Tests`, `Architecture.Tests`. Asserted by `ProjectReferenceTests.Infrastructure_ReferencesOnlyApplicationAndDomain`.

## Walkthrough

ProjectReferences: Application, Domain. Packages: `AspNetCore.HealthChecks.NpgSql` (the `AddNpgSql` check), `EFCore.NamingConventions` (`UseSnakeCaseNamingConvention`), `Microsoft.AspNetCore.Identity.EntityFrameworkCore` (ASP.NET Core Identity stores and `IdentityUserContext`), `Microsoft.EntityFrameworkCore`, `Microsoft.Extensions.Configuration.Abstractions` (`IConfiguration`), `Microsoft.Extensions.Options.DataAnnotations` (`ValidateDataAnnotations`), `Npgsql.EntityFrameworkCore.PostgreSQL`. `InternalsVisibleTo` Architecture.Tests. Npgsql-specific packages live only here (the Api never references them directly).

## Concepts Used

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

Reference list.

#### How it works here

Infrastructure points inward.

#### Why it matters here

Implements Application's ports without Application knowing.

### MSBuild, Central Package Management and analyzers

#### What it means

MSBuild builds .NET projects from XML project files. `Directory.Build.props` is imported into every project below it. Central Package Management keeps all NuGet versions in one file. Analyzers are compile-time rules; `TreatWarningsAsErrors` makes violations fail the build.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#5-startup--bootstrapping](../../../PROJECT_OVERVIEW2.md#5-startup--bootstrapping).)

#### Where it appears in this file

Versionless references.

#### How it works here

`PackageReference` items.

#### Why it matters here

Versions in `Directory.Packages.props`.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The Api reaches Npgsql types only transitively through this project; nothing forbids using them from `Program.cs` except convention (`ApiRuleTests` now scans the Api: only `Program` and the `Cli` namespace may reference Infrastructure).

## Related Files

- [`Directory.Packages.props`](../../Directory.Packages.props.md)
- [`src/TutoringCentre.Infrastructure/DependencyInjection.cs`](DependencyInjection.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs`](../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md)
