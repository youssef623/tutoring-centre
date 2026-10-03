# src/TutoringCentre.Domain/TutoringCentre.Domain.csproj

## Purpose
Project file for the innermost layer. It has **no project references and no package references**, so Domain depends only on the .NET base library.

## Where it fits
Domain layer. Referenced by Application, Infrastructure, Domain.Tests and Architecture.Tests. `ProjectReferenceTests.Domain_ReferencesNoProjects` asserts the empty reference list.

## Walkthrough
- **Line 1:** `Microsoft.NET.Sdk` (plain class library; framework and nullability come from `Directory.Build.props`).
- **Lines 3–5:** `<InternalsVisibleTo Include="TutoringCentre.Architecture.Tests" />` lets the architecture tests read the `internal` `AssemblyMarker`.

## Concepts used
- **InternalsVisibleTo:** a compiler visibility grant. It is not a dependency, so "Domain references nothing" still holds.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- The file starts with a UTF-8 BOM (`﻿<Project`), as written by `dotnet new`. Harmless.

## Related files
- [AssemblyMarker.cs](AssemblyMarker.cs.md)
- [ProjectReferenceTests.cs](../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md)
