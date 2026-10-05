# tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs

## Purpose

Shared helpers for the architecture tests: recognising repository interfaces/classes by name and describing a failing result.

## Where It Fits

Architecture.Tests. Used by `ApiRuleTests`, `CqrsRuleTests`, `DependencyRuleTests` and `RepositoryRuleTests`.

## Walkthrough

`ArchitectureSupport` (5-23).
- `IsRepositoryInterface(Type)` (8-12): an interface, name longer than `IRepository`, starts with `I`, ends with `Repository` (so `ICentreRepository` matches, a bare `IRepository` does not).
- `IsRepositoryClass(Type)` (15-16): a class whose name ends with `Repository`.
- `Describe(TestResult)` (18-22): `"ok"` or `"Violating types: ..."` joined from `FailingTypeNames`; used as the assertion message so a failure names the offending types.

## Concepts Used

### Architecture rules beyond dependencies

#### What it means

Beyond 'who references whom', tests can assert design conventions with reflection and IL scanning: every command has exactly one handler, handlers are sealed and internal, repositories live in the right project, endpoints do not touch Infrastructure.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#630-architecture-rules-beyond-dependencies](../../../PROJECT_OVERVIEW2.md#630-architecture-rules-beyond-dependencies).)

#### Where it appears in this file

Name-based classification.

#### How it works here

`IsRepositoryInterface` and `IsRepositoryClass`.

#### Why it matters here

Rules about 'repositories' need a definition; this one is a naming convention, so a repository named differently would escape the rules.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

The classification is purely by name; a repository class that does not end in `Repository` is invisible to `RepositoryRuleTests` and `ApiRuleTests`.

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs`](ApiRuleTests.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs`](CqrsRuleTests.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs`](RepositoryRuleTests.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`](DependencyRuleTests.cs.md)
