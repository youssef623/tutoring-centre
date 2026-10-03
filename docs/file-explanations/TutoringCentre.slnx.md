# TutoringCentre.slnx

## Purpose
The solution file in the new XML `.slnx` format. It lists the 9 projects in two solution folders, `/src/` and `/tests/`, so `dotnet build`/`dotnet test` at the root cover everything.

## Where it fits
Root. CI runs `dotnet restore|build|test TutoringCentre.slnx` (`ci.yml:30-36`, `codeql.yml:47`). `tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs:8,29` uses the file's *presence* as the marker for the repository root.

## Walkthrough
- **Lines 2–7:** `/src/` holds Api, Application, Domain, Infrastructure.
- **Lines 8–14:** `/tests/` holds the five test projects.

## Concepts used
- **.slnx:** the simplified XML solution format introduced in recent .NET SDKs. It replaces the GUID-heavy `.sln`.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Renaming or moving this file breaks `ProjectFiles.FindRepositoryRoot`, so the architecture tests would throw.

## Related files
- [ProjectFiles.cs](tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs.md)
- [ci.yml](.github/workflows/ci.yml.md)
