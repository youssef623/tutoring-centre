# Directory.Packages.props

## Purpose
**Central Package Management (CPM).** The single place where every NuGet package version is declared. Projects reference packages without a `Version` attribute.

## Where it fits
Root build config, used by every `.csproj`. Dependabot (`.github/dependabot.yml`) updates the versions here.

## Walkthrough
- **Line 3:** `ManagePackageVersionsCentrally=true` turns CPM on. A `Version=` attribute in any `.csproj` becomes an error.
- **Lines 5–7:** an empty `ItemGroup` whose comment says "PackageVersion entries go here". This is a leftover; the real entries are in the next group.
- **Lines 8–28:** one `PackageVersion` per package:

| Package | Version | Used by |
| --- | --- | --- |
| AspNetCore.HealthChecks.NpgSql | 9.0.0 | Infrastructure |
| coverlet.collector | 10.1.0 | all test projects |
| EFCore.NamingConventions | 10.0.1 | Infrastructure |
| FluentValidation / .DependencyInjectionExtensions | 12.1.1 | Application |
| Microsoft.AspNetCore.Mvc.Testing | 10.0.12 | Api.Tests |
| Microsoft.EntityFrameworkCore | 10.0.12 | Infrastructure |
| Microsoft.EntityFrameworkCore.Design | 10.0.12 | Api |
| Microsoft.Extensions.Configuration.Abstractions | 10.0.12 | Infrastructure |
| Microsoft.Extensions.DependencyInjection | 10.0.12 | Application.Tests |
| Microsoft.Extensions.DependencyInjection.Abstractions | 10.0.12 | Application |
| Microsoft.Extensions.Logging.Abstractions | 10.0.12 | Application |
| Microsoft.Extensions.TimeProvider.Testing | 10.10.0 | Infrastructure.Tests |
| Microsoft.NET.Test.Sdk | 18.10.1 | all test projects |
| NetArchTest.Rules | 1.3.2 | Architecture.Tests |
| Npgsql.EntityFrameworkCore.PostgreSQL | 10.0.3 | Infrastructure |
| Serilog.AspNetCore | 10.0.0 | Api |
| xunit / xunit.runner.visualstudio | 2.9.3 / 4.0.0 | all test projects |

## Concepts used
- **CPM:** one version per package for the whole repo, so there is no drift.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- `AspNetCore.HealthChecks.NpgSql` 9.0.0 trails the .NET 10 major version. It works (tests pass) but is worth watching.
- No trailing newline (line 29), contrary to `.editorconfig`.

## Related files
- [Directory.Build.props](Directory.Build.props.md)
- [dependabot.yml](.github/dependabot.yml.md)
- [Infrastructure csproj](src/TutoringCentre.Infrastructure/TutoringCentre.Infrastructure.csproj.md)
