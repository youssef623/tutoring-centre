# src/TutoringCentre.Api/TutoringCentre.Api.csproj

## Purpose
Project file for the ASP.NET Core host (`Microsoft.NET.Sdk.Web`). It defines the user-secrets id, references Application and Infrastructure, and adds the EF design-time package and Serilog.

## Where it fits
Api layer, the composition root. Referenced by Api.Tests and Architecture.Tests. `ProjectReferenceTests.Api_ReferencesOnlyApplicationAndInfrastructure` asserts the reference list.

## Walkthrough
- **Line 3:** `<UserSecretsId>a0748b74-413e-4737-b25c-2a2e46b2d4e6</UserSecretsId>` keys the per-machine secret store used by `dotnet user-secrets set ...` (README step 3).
- **Lines 6–7:** references to Application and Infrastructure. The Infrastructure reference is for the composition root only.
- **Line 10:** `InternalsVisibleTo` Architecture.Tests.
- **Lines 13–16:** `Microsoft.EntityFrameworkCore.Design` with `PrivateAssets=all`. It's needed by `dotnet ef` when this is the startup project, and isn't flowed to consumers.
- **Line 17:** `Serilog.AspNetCore`.

## Concepts used
- **Web SDK:** implicit ASP.NET Core framework reference and launch settings.
- **User secrets.**
- **PrivateAssets:** a development-only dependency.

## Data and control flow
Not applicable.

## Configuration and environment
`UserSecretsId`.

## Gotchas and issues
- Line 5's `<ItemGroup>` is indented with 4 spaces instead of 2 (cosmetic).
- UTF-8 BOM at the file start.

## Related files
- [Program.cs](Program.cs.md)
- [.config/dotnet-tools.json](../../.config/dotnet-tools.json.md)
- [ProjectReferenceTests.cs](../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md)
