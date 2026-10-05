# tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs

## Purpose

Architecture rules about repository placement: `I*Repository` interfaces live in Application, `*Repository` classes live in Infrastructure.

## Where It Fits

Architecture.Tests. Scans all four source assemblies (`SourceAssemblies.All`) with reflection.

## Walkthrough

- `RepositoryInterfaces_LiveInTheApplicationProject` (5-21): collects every interface matching `IsRepositoryInterface` across all assemblies; asserts non-empty; violation = assembly is not Application or namespace does not start with `TutoringCentre.Application`.
- `RepositoryClasses_LiveInTheInfrastructureProject` (23-39): same for classes ending in `Repository`; must be in the Infrastructure assembly and a `TutoringCentre.Infrastructure*` namespace.

## Concepts Used

### Repository pattern and read services

#### What it means

A *repository* looks like a collection of aggregates (`Add`, `ExistsBy...`) and hides how they are stored. A *read service* is a separate query-side abstraction that returns DTOs directly, so reads need not load and map domain entities.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services](../../../PROJECT_OVERVIEW2.md#68-repository-pattern-and-read-services).)

#### Where it appears in this file

Where repositories may live.

#### How it works here

Both tests.

#### Why it matters here

Interface (port) in Application, implementation (adapter) in Infrastructure, enforced by tests.

### Architecture rules beyond dependencies

#### What it means

Beyond 'who references whom', tests can assert design conventions with reflection and IL scanning: every command has exactly one handler, handlers are sealed and internal, repositories live in the right project, endpoints do not touch Infrastructure.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#630-architecture-rules-beyond-dependencies](../../../PROJECT_OVERVIEW2.md#630-architecture-rules-beyond-dependencies).)

#### Where it appears in this file

Non-vacuity guards.

#### How it works here

`Assert.NotEmpty` at 14 and 29.

#### Why it matters here

A rule that finds no types would silently pass; the guard makes it fail instead.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs`](ArchitectureSupport.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs`](SourceAssemblies.cs.md)
