# tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs

## Purpose

Declared-reference architecture tests: each `src` project's `ProjectReference` list must equal the allowed set.

## Where It Fits

Architecture.Tests. Uses `ProjectFiles`.

## Walkthrough

Allowed sets (typed from the architecture diagram): Domain `[]`, Application `["TutoringCentre.Domain"]`, Infrastructure `["TutoringCentre.Application","TutoringCentre.Domain"]`, Api `["TutoringCentre.Application","TutoringCentre.Infrastructure"]`. Four `[Fact]`s `Assert.Equal(allowed, ProjectFiles.ReadProjectReferences(...))`. Sees a forbidden reference even if no code uses it; blind to package references and `FrameworkReference`.

## Concepts Used

### Architecture tests (fitness functions)

#### What it means

An executable test that fails when a structural rule (who may reference whom) is broken, turning a diagram into an enforced rule. Two angles are used: declared project references and compiled-type usage.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions](../../../PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions).)

#### Where it appears in this file

Declared-reference rule.

#### How it works here

Four tests.

#### Why it matters here

Complements the IL test.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs`](ProjectFiles.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`](DependencyRuleTests.cs.md)
- [`src/TutoringCentre.Api/TutoringCentre.Api.csproj`](../../src/TutoringCentre.Api/TutoringCentre.Api.csproj.md)
