# tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs

## Purpose

Architecture rules for the Api project: endpoints must not touch Infrastructure, repositories or domain entities, and only `Program` and the CLI may reference Infrastructure.

## Where It Fits

Architecture.Tests. Scans the Api assembly (via `SourceAssemblies.Api`) with NetArchTest, using helpers from `ArchitectureSupport`.

## Walkthrough

Constants (lines 7-9): namespaces `TutoringCentre.Api.Endpoints`, `TutoringCentre.Infrastructure`, `TutoringCentre.Api.Cli`.
- `Endpoints_DoNotDependOnInfrastructureRepositoriesOrDomainEntities` (11-42): builds a *dynamic* forbidden list: (a) all Application interfaces named `I*Repository` (`ArchitectureSupport.IsRepositoryInterface`); (b) every Domain namespace except `TutoringCentre.Domain`, `...Domain.Common` and its children (so `Result`/`Error` mapping stays allowed, entities/value objects do not); (c) the Infrastructure namespace. Guards: asserts the endpoint type list, the repository list and the namespace list are non-empty (so the rule cannot pass vacuously). Then `Types.InAssembly(Api).That().ResideInNamespace(Endpoints).ShouldNot().HaveDependencyOnAny(forbidden)`.
- `OnlyProgramAndCli_ReferenceInfrastructure` (44-54): all Api types except `Program` and those in `TutoringCentre.Api.Cli` must not depend on `TutoringCentre.Infrastructure`.

## Concepts Used

### Architecture tests (fitness functions)

#### What it means

An executable test that fails when a structural rule (who may reference whom) is broken, turning a diagram into an enforced rule. Two angles are used: declared project references and compiled-type usage.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions](../../../PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions).)

#### Where it appears in this file

Both tests.

#### How it works here

Both tests.

#### Why it matters here

Turns 'endpoints only talk to Application via the Dispatcher' from a convention into a failing test.

### Architecture rules beyond dependencies

#### What it means

Beyond 'who references whom', tests can assert design conventions with reflection and IL scanning: every command has exactly one handler, handlers are sealed and internal, repositories live in the right project, endpoints do not touch Infrastructure.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#630-architecture-rules-beyond-dependencies](../../../PROJECT_OVERVIEW2.md#630-architecture-rules-beyond-dependencies).)

#### Where it appears in this file

Dynamic forbidden lists and non-vacuity guards.

#### How it works here

The first test, from `repositoryInterfaces` through the `Assert.NotEmpty` guards (lines 14-32).

#### Why it matters here

The forbidden set is derived from the code itself, so new repositories and new Domain namespaces are covered automatically.

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

Who may know Infrastructure.

#### How it works here

`OnlyProgramAndCli_ReferenceInfrastructure` (44-54).

#### Why it matters here

Only the composition root (`Program`) and the CLI may reference Infrastructure; everything else depends on Application ports.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

`OnlyProgramAndCli_ReferenceInfrastructure` only inspects types compiled into the Api assembly; it cannot see which *namespaces* of other assemblies are legal, only the Infrastructure namespace string.

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/ArchitectureSupport.cs`](ArchitectureSupport.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs`](SourceAssemblies.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs`](IdentityRuleTests.cs.md)
- [`tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`](DependencyRuleTests.cs.md)
- [`src/TutoringCentre.Api/Program.cs`](../../src/TutoringCentre.Api/Program.cs.md)
