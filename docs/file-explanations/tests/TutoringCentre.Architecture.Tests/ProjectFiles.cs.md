# tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs

## Purpose
A helper that reads the `<ProjectReference Include="...">` items of a `src` project's `.csproj` as XML. It returns the referenced project names, without paths or extensions, sorted ordinally.

## Where it fits
Architecture.Tests, `internal static`. Used by `ProjectReferenceTests`. It depends on the repository layout (`src/<Name>/<Name>.csproj`) and on `TutoringCentre.slnx` existing at the root.

## Walkthrough
- **Line 8:** `SolutionFileName = "TutoringCentre.slnx"`.
- **Lines 11–24, `ReadProjectReferences(projectName)`:**
  1. Build the path `<root>/src/<name>/<name>.csproj` and `XDocument.Load` it.
  2. `Descendants("ProjectReference")`, take the `Include` attribute, drop nulls with `OfType<string>()`.
  3. Replace `\` with the OS separator, because `dotnet add reference` on Windows writes backslashes and Linux treats them as ordinary characters. Then `Path.GetFileNameWithoutExtension`.
  4. `Order(StringComparer.Ordinal).ToArray()`.
- **Lines 26–36, `FindRepositoryRoot`:** walk up from `AppContext.BaseDirectory` (the test's `bin/...` folder) until a directory contains the `.slnx`. Throw `InvalidOperationException` if it isn't found.

## Concepts used
- **LINQ to XML.**
- **Cross-platform path handling.**
- **Locating the repo root by marker file:** no hard-coded paths, so it behaves the same locally and in CI.

## Data and control flow
Project name → csproj path → XML → names → sorted array.

## Configuration and environment
None.

## Gotchas and issues
- **SDK-style csproj only.** `Descendants("ProjectReference")` works because SDK-style csproj files have no XML namespace. An old-style csproj with `xmlns` would return nothing, and the tests would compare against an empty list.
- **Imported references are invisible.** References added through `Directory.Build.props` or `Condition`s wouldn't be seen.

## Related files
- [ProjectReferenceTests.cs](ProjectReferenceTests.cs.md)
- [TutoringCentre.slnx](../../TutoringCentre.slnx.md)
