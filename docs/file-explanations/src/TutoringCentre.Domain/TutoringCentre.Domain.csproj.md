# src/TutoringCentre.Domain/TutoringCentre.Domain.csproj

## Purpose

Project file for the innermost layer. Its defining feature is what it does *not* contain: no project references and no packages.

## Where It Fits

Part of `TutoringCentre.slnx`; referenced by Application, Infrastructure and `Domain.Tests`/`Architecture.Tests`. `ProjectReferenceTests.Domain_ReferencesNoProjects` asserts it has zero `ProjectReference` items.

## Walkthrough

Only one item: `<InternalsVisibleTo Include="TutoringCentre.Architecture.Tests" />`, which lets the architecture tests see the `internal` `AssemblyMarker`. `TargetFramework`, nullable and implicit usings come from `Directory.Build.props`. No `PackageReference` means Domain cannot use EF, ASP.NET or FluentValidation; `DependencyRuleTests.Domain_DoesNotDependOnOtherLayersOrFrameworks` checks the compiled types too.

## Concepts Used

### Dependency inversion, ports and adapters

#### What it means

A *dependency* is something a piece of code needs in order to work. Normally high-level business code ends up depending on low-level details (database, HTTP). **Dependency inversion** reverses that: the business layer declares an interface (a *port*) describing what it needs, and the low-level layer supplies a class implementing it (an *adapter*). The compiler-level arrow then points from detail to policy, so business code can be tested and reused without the detail.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root](../../../PROJECT_OVERVIEW2.md#61-clean-architecture-dependency-inversion-and-the-composition-root).)

#### Where it appears in this file

The empty reference list.

#### How it works here

Domain is the policy centre; everything else points at it.

#### Why it matters here

Business rules can be tested with no infrastructure.

### Architecture tests (fitness functions)

#### What it means

An executable test that fails when a structural rule (who may reference whom) is broken, turning a diagram into an enforced rule. Two angles are used: declared project references and compiled-type usage.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions](../../../PROJECT_OVERVIEW2.md#615-architecture-tests-fitness-functions).)

#### Where it appears in this file

`InternalsVisibleTo`.

#### How it works here

Grants compile-time access for `typeof(TutoringCentre.Domain.AssemblyMarker)`.

#### Why it matters here

Avoids string-based assembly loading.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

No settings; inherits from `Directory.Build.props`.

## Gotchas and Issues

Adding any `PackageReference` here would pass the build but should be treated as an architecture violation.

## Related Files

- [`Directory.Build.props`](../../Directory.Build.props.md)
- [`tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs`](../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs.md)
- [`src/TutoringCentre.Domain/AssemblyMarker.cs`](AssemblyMarker.cs.md)
