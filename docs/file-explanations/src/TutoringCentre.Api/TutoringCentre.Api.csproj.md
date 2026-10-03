# src/TutoringCentre.Api/TutoringCentre.Api.csproj

## Purpose

Project file for the web host: the composition root and HTTP edge.

## Where It Fits

Entry-point project (`Microsoft.NET.Sdk.Web`). References Application and Infrastructure; referenced by `Api.Tests` and `Architecture.Tests`.

## Walkthrough

`UserSecretsId a0748b74-...` enables `dotnet user-secrets` (README step 3 stores the connection string there). ProjectReferences: Application, Infrastructure. `InternalsVisibleTo Architecture.Tests`. Packages: `Microsoft.EntityFrameworkCore.Design` with `IncludeAssets` and `PrivateAssets=all` (needed so `dotnet ef` can use the Api as startup project, without flowing to dependents) and `Serilog.AspNetCore`. The `Web` SDK supplies the ASP.NET Core shared framework.

## Concepts Used

### Secrets and configuration layering

#### What it means

Configuration comes from layered sources (JSON files, user-secrets, environment variables, command line) where later layers override earlier ones. Secrets must stay out of git: this repo keeps the connection string out of `appsettings.json`, reads it from user-secrets or an environment variable, and scans history with gitleaks.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#11-configuration--environment--deployment](../../../PROJECT_OVERVIEW.md#11-configuration--environment--deployment).)

#### Where it appears in this file

`UserSecretsId`.

#### How it works here

First `PropertyGroup`.

#### Why it matters here

Links the project to the per-user secrets store outside the repository.

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../PROJECT_OVERVIEW.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

Infrastructure reference.

#### How it works here

`ProjectReference`.

#### Why it matters here

Present only so `Program.cs` can call `AddInfrastructure` (composition root).

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

`UserSecretsId`: a GUID (not itself a secret) naming the user-secrets folder.

## Gotchas and Issues

Mixed indentation/BOM at the top (cosmetic).

## Related Files

- [`src/TutoringCentre.Api/Program.cs`](Program.cs.md)
- [`Directory.Packages.props`](../../Directory.Packages.props.md)
- [`tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs`](../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md)
