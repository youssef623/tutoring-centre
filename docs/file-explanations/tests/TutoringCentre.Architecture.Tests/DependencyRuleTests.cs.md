# tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs

## Purpose

Type-level architecture tests: fail when compiled code in a layer uses a forbidden namespace.

## Where It Fits

Architecture.Tests. Inspects the Domain, Application and Infrastructure assemblies via their `AssemblyMarker` types, using `NetArchTest.Rules`.

## Walkthrough

Static assembly handles (10-12) and three string arrays of forbidden namespaces: Domain must not use `TutoringCentre.Application/Infrastructure/Api`, `Microsoft.EntityFrameworkCore`, `Microsoft.AspNetCore`, `Npgsql`; Application must not use Infrastructure/Api/EF/AspNetCore/Npgsql; Infrastructure must not use Api. `AssertNoDependencies(assembly, forbidden)`: `Types.InAssembly(assembly)`; `Assert.NotEmpty(types.GetTypes())` (guard against an empty/wrong assembly passing vacuously); `ShouldNot().HaveDependencyOnAny(forbidden).GetResult()`; assertion message lists failing type names. Three `[Fact]`s call it. Blind spot: a declared-but-unused project reference leaves nothing in IL (compiler drops it) - that is why `ProjectReferenceTests` exists. Also not scanned: the Api assembly.

## Concepts Used

### Architecture tests (fitness functions)

#### What it means

An executable test that fails when a structural rule (who may reference whom) is broken, turning a diagram into an enforced rule. Two angles are used: declared project references and compiled-type usage.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#615-architecture-tests-fitness-functions](../../../PROJECT_OVERVIEW.md#615-architecture-tests-fitness-functions).)

#### Where it appears in this file

Type-level rule.

#### How it works here

`AssertNoDependencies`.

#### Why it matters here

Catches real usage, not just declarations.

### Generics and constraints

#### What it means

Generics let one definition work for many types (`Result<T>`, `ICommandHandler<TCommand,TResponse>`). Constraints (`where TCommand : ICommand<TResponse>`) restrict which types are legal so the compiler can check them; the `in` modifier (contravariance) lets a handler of a base type satisfy a derived one.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Reflection handles.

#### How it works here

`typeof(...AssemblyMarker).Assembly`.

#### Why it matters here

Compile-checked assembly lookup.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

NetArchTest reads IL (via Mono.Cecil); a dependency used only through a base type of another assembly may not be flagged - not verified here.

## Related Files

- [`tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs`](ProjectReferenceTests.cs.md)
- [`src/TutoringCentre.Domain/AssemblyMarker.cs`](../../src/TutoringCentre.Domain/AssemblyMarker.cs.md)
- [`src/TutoringCentre.Application/AssemblyMarker.cs`](../../src/TutoringCentre.Application/AssemblyMarker.cs.md)
- [`src/TutoringCentre.Infrastructure/AssemblyMarker.cs`](../../src/TutoringCentre.Infrastructure/AssemblyMarker.cs.md)
