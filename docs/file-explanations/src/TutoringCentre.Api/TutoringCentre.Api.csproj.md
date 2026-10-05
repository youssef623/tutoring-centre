# src/TutoringCentre.Api/TutoringCentre.Api.csproj

## Purpose

Project file for the web host: the composition root and HTTP edge, including build-time OpenAPI document generation.

## Where It Fits

Entry-point project (`Microsoft.NET.Sdk.Web`). References Application and Infrastructure; referenced by `Api.Tests` and `Architecture.Tests`. Its OpenAPI settings write `frontend/openapi/TutoringCentre.Api.json`, which CI checks for staleness.

## Walkthrough

`UserSecretsId a0748b74-...` enables `dotnet user-secrets` (connection string and `Seed:Password` live there in development). `OpenApiGenerateDocuments true` and `OpenApiDocumentsDirectory $(MSBuildProjectDirectory)/../../frontend/openapi` (framework behaviour of `Microsoft.Extensions.ApiDescription.Server`: after build it starts the app in a tool host and writes the OpenAPI document into that folder). ProjectReferences: Application, Infrastructure. `InternalsVisibleTo Architecture.Tests`. Packages: `Microsoft.AspNetCore.OpenApi` (`AddOpenApi`/`MapOpenApi`), `Microsoft.EntityFrameworkCore.Design` (design-time for `dotnet ef`, `PrivateAssets=all`), `Microsoft.Extensions.ApiDescription.Server` (build-time generation, `PrivateAssets=all`) and `Serilog.AspNetCore`.

## Concepts Used

### OpenAPI document generation

#### What it means

OpenAPI is a JSON description of every endpoint, request and response. ASP.NET Core can build it from the endpoint metadata (`Produces`, `WithName`...), and a build step can write it to a file that is committed and diffed in CI.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#631-contract-first-api-client-generation](../../../PROJECT_OVERVIEW2.md#631-contract-first-api-client-generation).)

#### Where it appears in this file

Build-time document generation.

#### How it works here

Lines 4-5 and the two OpenAPI package references.

#### Why it matters here

Every build refreshes the committed contract; `git diff --exit-code -- frontend/openapi` in CI fails if someone forgot to commit it.

### Secrets and configuration layering

#### What it means

Configuration comes from layered sources (JSON files, user-secrets, environment variables, command line) where later layers override earlier ones. Secrets must stay out of git: this repo keeps the connection string out of `appsettings.json`, reads it from user-secrets or an environment variable, and scans history with gitleaks.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#11-configuration--environment--deployment](../../../PROJECT_OVERVIEW2.md#11-configuration--environment--deployment).)

#### Where it appears in this file

User secrets id.

#### How it works here

`UserSecretsId`.

#### Why it matters here

A GUID naming the user-secrets folder; not a secret itself.

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

Only Application and Infrastructure referenced.

#### How it works here

ProjectReferences.

#### Why it matters here

The Api is the composition root.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

`UserSecretsId`: a GUID naming the user-secrets folder.

## Gotchas and Issues

Mixed indentation/BOM at the top (cosmetic). Generation runs `Program.cs` inside `GetDocument.Insider`, which is why `Program.cs` has the `isDocumentGeneration` guard.

## Related Files

- [`src/TutoringCentre.Api/Program.cs`](Program.cs.md)
- [`Directory.Packages.props`](../../Directory.Packages.props.md)
- `frontend/openapi/TutoringCentre.Api.json` (generated / lockfile / media: no separate explanation, see INDEX)
- [`tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs`](../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md)
