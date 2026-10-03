# Directory.Packages.props

## Purpose

Single source of truth for every NuGet package version (Central Package Management).

## Where It Fits

Root MSBuild file imported by all `.csproj` files. Each project writes `<PackageReference Include="X" />` with no version and MSBuild looks the version up here. Dependabot (`.github/dependabot.yml`, ecosystem `nuget`) edits this file.

## Walkthrough

- `ManagePackageVersionsCentrally=true` (line 3) - switches on CPM. With it, a `Version=` attribute on a `PackageReference` is an error.
- First `ItemGroup` is empty except for a comment ("PackageVersion entries go here as projects add dependencies.") - scaffolding left from the initial commit.
- Second `ItemGroup`: 24 `PackageVersion` items. Grouped by purpose:
  - Runtime: `Microsoft.EntityFrameworkCore` 10.0.12, `Npgsql.EntityFrameworkCore.PostgreSQL` 10.0.3, `EFCore.NamingConventions` 10.0.1, `AspNetCore.HealthChecks.NpgSql` 9.0.0, `FluentValidation` + `.DependencyInjectionExtensions` 12.1.1, `Serilog.AspNetCore` 10.0.0, `Microsoft.Extensions.*` 10.0.12 (Configuration, Configuration.Abstractions, DependencyInjection, DependencyInjection.Abstractions, Logging, Logging.Abstractions, Options.DataAnnotations).
  - Design time: `Microsoft.EntityFrameworkCore.Design` 10.0.12.
  - Test: `xunit` 2.9.3, `xunit.runner.visualstudio` 4.0.0, `Microsoft.NET.Test.Sdk` 18.10.1, `coverlet.collector` 10.1.0, `Microsoft.AspNetCore.Mvc.Testing` 10.0.12, `Testcontainers.PostgreSql` 4.15.0, `Respawn` 7.0.0, `NetArchTest.Rules` 1.3.2, `Microsoft.Extensions.TimeProvider.Testing` 10.10.0.
Note `AspNetCore.HealthChecks.NpgSql` is 9.x while everything else is 10.x; the repo does not say whether this is intentional (it is the latest line I can verify only by the file itself).

## Concepts Used

### MSBuild, Central Package Management and analyzers

#### What it means

MSBuild builds .NET projects from XML project files. `Directory.Build.props` is imported into every project below it. Central Package Management keeps all NuGet versions in one file. Analyzers are compile-time rules; `TreatWarningsAsErrors` makes violations fail the build.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#5-startup--bootstrapping](../PROJECT_OVERVIEW.md#5-startup--bootstrapping).)

#### Where it appears in this file

Whole file; `ManagePackageVersionsCentrally` on line 3.

#### How it works here

A restore resolves each versionless `PackageReference` against these entries. A package missing here fails restore with NU1010.

#### Why it matters here

One place to bump versions and for Dependabot to edit; avoids two projects pinning different EF versions.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None.

## Gotchas and Issues

Gotcha: the `Microsoft.Extensions.*` packages are only referenced by some projects; a project that needs a package not listed here must add a `PackageVersion` first.

## Related Files

- [`Directory.Build.props`](Directory.Build.props.md)
- [`global.json`](global.json.md)
- [`.github/dependabot.yml`](.github/dependabot.yml.md)
- [`src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj`](src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj.md)
