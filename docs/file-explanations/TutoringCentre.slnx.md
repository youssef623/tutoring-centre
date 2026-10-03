# TutoringCentre.slnx

## Purpose

Solution file listing the nine .NET projects so `dotnet build/test TutoringCentre.slnx` operates on the whole repo.

## Where It Fits

Root file used by CI (`dotnet restore/build/test TutoringCentre.slnx`) and by IDEs. `tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs` also finds the repository root by walking up until it finds a file with this exact name.

## Walkthrough

`.slnx` is the XML solution format. Two `Folder` nodes: `/src/` (Api, Application, Domain, Infrastructure) and `/tests/` (Api.Tests, Application.Tests, Architecture.Tests, Domain.Tests, Infrastructure.Tests). Each `Project Path` is a relative path to a `.csproj`. A project missing here would not be built or tested by CI even if it exists on disk.

## Concepts Used

This file introduces no concept that needs a tutorial beyond what its walkthrough already explains.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None.

## Gotchas and Issues

Renaming this file would break `ProjectFiles.SolutionFileName` (tests would throw 'Could not find TutoringCentre.slnx').

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs`](tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs.md)
- [`.github/workflows/ci.yml`](.github/workflows/ci.yml.md)
- [`global.json`](global.json.md)
