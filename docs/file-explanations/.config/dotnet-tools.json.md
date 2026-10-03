# .config/dotnet-tools.json

## Purpose
The .NET **local tool manifest**. It pins `dotnet-ef` (Entity Framework Core CLI) to version `10.0.12`, so every developer and CI run the same migrations tool after `dotnet tool restore`.

## Where it fits
Tooling. It pairs with `Microsoft.EntityFrameworkCore.Design` (also 10.0.12), referenced by `src/TutoringCentre.Api/TutoringCentre.Api.csproj:13-16`, which `dotnet-ef` needs in the startup project. No migrations exist yet, so the tool is currently unused.

## Walkthrough
- `"isRoot": true` (line 3): stops the SDK searching parent folders for more manifests.
- `tools.dotnet-ef.version: "10.0.12"`, `commands: ["dotnet-ef"]`.
- `"rollForward": false` (line 10): use exactly this tool version; don't roll forward to a newer runtime.

## Concepts used
- **Local tools:** CLI tools versioned per repo instead of installed globally. Run them with `dotnet tool restore` and then `dotnet ef ...`.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- No migrations exist yet (no `Migrations/` folder anywhere).
- CI does not run `dotnet tool restore`.
- The file has no trailing newline, which goes against `.editorconfig`'s `insert_final_newline`.

## Related files
- [TutoringCentre.Api.csproj](src/TutoringCentre.Api/TutoringCentre.Api.csproj.md)
- [Directory.Packages.props](Directory.Packages.props.md)
- [AppDbContext.cs](src/TutoringCentre.Infrastructure/Persistence/AppDbContext.cs.md)
