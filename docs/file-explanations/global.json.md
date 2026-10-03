# global.json

## Purpose

Pins which .NET SDK builds the solution so every laptop and CI runner uses the same compiler and tooling line.

## Where It Fits

Repository root configuration. Read by the `dotnet` CLI (SDK resolver) for every command run below this directory and explicitly by `.github/workflows/ci.yml` and `codeql.yml` (`actions/setup-dotnet` with `global-json-file: global.json`). Nothing in the repo imports it.

## Walkthrough

`{ "sdk": { "version": "10.0.401", "rollForward": "latestFeature" } }` (whole file, 6 lines).

- `version: 10.0.401` - the SDK the repo was built against.
- `rollForward: latestFeature` - if 10.0.401 is not installed, the resolver may pick a newer *feature band* of the same major.minor (10.0.4xx, 10.0.5xx...) but never a different major/minor. Without this key a missing exact version would make every `dotnet` command fail.
- Edge case: a machine with only .NET 9 gets an error from `dotnet`, not a silent downgrade.
- If changed incorrectly: bumping to a version CI cannot install (setup-dotnet downloads it, so usually fine) or to a different major without updating `Directory.Build.props` (`net10.0`) and package versions would break the build.

## Concepts Used

### MSBuild, Central Package Management and analyzers

#### What it means

MSBuild builds .NET projects from XML project files. `Directory.Build.props` is imported into every project below it. Central Package Management keeps all NuGet versions in one file. Analyzers are compile-time rules; `TreatWarningsAsErrors` makes violations fail the build.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#5-startup--bootstrapping](../PROJECT_OVERVIEW.md#5-startup--bootstrapping).)

#### Where it appears in this file

`global.json` is the SDK half of build reproducibility; `Directory.Build.props` and `Directory.Packages.props` are the other halves.

#### How it works here

The SDK resolver reads this file before running any command and selects the SDK; the TFM and package versions then come from the props files.

#### Why it matters here

Guarantees that analyzers (`AnalysisLevel=latest-recommended`) behave identically on every machine, which matters because warnings are errors.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

No environment variables. SDK version `10.0.401` is the only configuration value.

## Gotchas and Issues

None found. Note that the `Microsoft.EntityFrameworkCore` packages are `10.0.12`, so SDK and EF are expected to be bumped together (the file does not enforce this).

## Related Files

- [`Directory.Build.props`](Directory.Build.props.md)
- [`Directory.Packages.props`](Directory.Packages.props.md)
- [`.github/workflows/ci.yml`](.github/workflows/ci.yml.md)
- [`.config/dotnet-tools.json`](.config/dotnet-tools.json.md)
