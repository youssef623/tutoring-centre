# tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs

## Purpose

Helper that reads the `ProjectReference` items of a `src` project from its `.csproj` XML.

## Where It Fits

Architecture.Tests, `internal static`. Used by `ProjectReferenceTests`.

## Walkthrough

`ReadProjectReferences(projectName)`: `FindRepositoryRoot()` walks up from `AppContext.BaseDirectory` until a directory contains `TutoringCentre.slnx` (throws if none); loads `src/<name>/<name>.csproj` with `XDocument`; selects `Descendants("ProjectReference")`' `Include` attributes (`OfType<string>()` drops nulls); normalises `\` to the platform separator (Windows-authored paths on Linux CI); `Path.GetFileNameWithoutExtension`; sorts with `StringComparer.Ordinal`. Ordinal sorting means expected arrays in the tests must be written in ordinal order.

## Concepts Used

### Architecture tests (fitness functions)

#### What it means

An executable test that fails when a structural rule (who may reference whom) is broken, turning a diagram into an enforced rule. Two angles are used: declared project references and compiled-type usage.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#615-architecture-tests-fitness-functions](../../../PROJECT_OVERVIEW.md#615-architecture-tests-fitness-functions).)

#### Where it appears in this file

Declared-reference reader.

#### How it works here

Whole class.

#### Why it matters here

Cross-platform, location-independent.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs`](ProjectReferenceTests.cs.md)
- [`TutoringCentre.slnx`](../../TutoringCentre.slnx.md)
