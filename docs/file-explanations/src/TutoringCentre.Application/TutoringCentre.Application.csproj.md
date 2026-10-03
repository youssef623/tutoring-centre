# src/TutoringCentre.Application/TutoringCentre.Application.csproj

## Purpose

Project file for the use-case layer: references Domain only, plus validation and DI/logging *abstractions*.

## Where It Fits

Referenced by Infrastructure, Api and `Application.Tests`/`Architecture.Tests`.

## Walkthrough

`ProjectReference` Domain. Packages: `FluentValidation`, `FluentValidation.DependencyInjectionExtensions` (provides `AddValidatorsFromAssembly`), `Microsoft.Extensions.DependencyInjection.Abstractions` (`IServiceCollection`, `AddScoped`), `Microsoft.Extensions.Logging.Abstractions` (`ILogger<T>`). Only *Abstractions* packages are used so Application does not pick up a concrete host. `InternalsVisibleTo` for `TutoringCentre.Architecture.Tests` and `TutoringCentre.Application.Tests` (the second one lets tests construct `internal` handlers and validators directly).

## Concepts Used

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../PROJECT_OVERVIEW.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

Reference list.

#### How it works here

Application -> Domain only.

#### Why it matters here

Ports are defined here and implemented outward.

### MSBuild, Central Package Management and analyzers

#### What it means

MSBuild builds .NET projects from XML project files. `Directory.Build.props` is imported into every project below it. Central Package Management keeps all NuGet versions in one file. Analyzers are compile-time rules; `TreatWarningsAsErrors` makes violations fail the build.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#5-startup--bootstrapping](../../../PROJECT_OVERVIEW.md#5-startup--bootstrapping).)

#### Where it appears in this file

Versionless `PackageReference`s.

#### How it works here

Versions resolve from `Directory.Packages.props`.

#### Why it matters here

Single version source.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Application must not reference EF/ASP.NET; `DependencyRuleTests.Application_DoesNotDependOnInfrastructureApiOrFrameworks` enforces it on compiled types.

## Related Files

- [`Directory.Packages.props`](../../Directory.Packages.props.md)
- [`src/TutoringCentre.Application/DependencyInjection.cs`](DependencyInjection.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`](../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs.md)
