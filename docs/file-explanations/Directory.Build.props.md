# Directory.Build.props

## Purpose

Applies the same compiler settings to every .NET project in the repository without repeating them in each `.csproj`.

## Where It Fits

Imported automatically by MSBuild into all nine `.csproj` files under the repo (4 in `src/`, 5 in `tests/`). It is the reason the `.csproj` files do not contain `TargetFramework`.

## Walkthrough

Single `PropertyGroup`, lines 3-8:
- `TargetFramework=net10.0` - all projects compile for .NET 10.
- `Nullable=enable` - nullable reference types: `string` cannot be null, `string?` can, and the compiler warns on misuse (visible in the code as `Error? Error`, `string?` fields, `null!` in tests).
- `ImplicitUsings=enable` - the SDK adds common `using` directives (`System`, `System.Linq`, `System.Threading.Tasks`...), which is why files rarely import them. For `Sdk.Web` projects it also adds `Microsoft.AspNetCore.*` namespaces.
- `TreatWarningsAsErrors=true` - any compiler or analyzer warning fails the build.
- `AnalysisLevel=latest-recommended` - turns on the latest recommended .NET code analyzers (CA rules). This is why the source contains many `[SuppressMessage(... Justification = "...")]` attributes (e.g. CA1812, CA1848, CA1873, CA1716).
If changed incorrectly: removing `TreatWarningsAsErrors` silently lets analyzer violations accumulate; changing `Nullable` would produce hundreds of warnings.

## Concepts Used

### MSBuild, Central Package Management and analyzers

#### What it means

MSBuild builds .NET projects from XML project files. `Directory.Build.props` is imported into every project below it. Central Package Management keeps all NuGet versions in one file. Analyzers are compile-time rules; `TreatWarningsAsErrors` makes violations fail the build.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#5-startup--bootstrapping](../PROJECT_OVERVIEW.md#5-startup--bootstrapping).)

#### Where it appears in this file

The whole file.

#### How it works here

MSBuild walks up from each project directory, finds this file and imports it before the project's own properties.

#### Why it matters here

One edit changes all projects; the `src/.semantic.md` note confirms projects intentionally omit these properties so they cannot drift.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

No environment variables. Build settings only.

## Gotchas and Issues

Because warnings are errors, adding a new package that triggers an analyzer warning in existing code fails CI; the repo deals with this by justified suppressions rather than by lowering `AnalysisLevel`.

## Related Files

- [`Directory.Packages.props`](Directory.Packages.props.md)
- [`global.json`](global.json.md)
- [`src/TutoringCentre.Api/TutoringCentre.Api.csproj`](src/TutoringCentre.Api/TutoringCentre.Api.csproj.md)
- [`src/TutoringCentre.Domain/TutoringCentre.Domain.csproj`](src/TutoringCentre.Domain/TutoringCentre.Domain.csproj.md)
