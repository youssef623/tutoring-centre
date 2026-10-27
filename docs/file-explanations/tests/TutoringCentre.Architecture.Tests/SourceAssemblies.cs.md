# `tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs`

## Purpose

Declares `SourceAssemblies`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 574 bytes. Source hash: `41a44eb6c0902b1fac4aba60985e9afb84a8317bc8a1ab764ff3f6bd42f98bc1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Reflection;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../tests/TutoringCentre.Architecture.Tests/SourceAssemblies.cs#L5): `internal static class SourceAssemblies`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs:14](../../../../tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs#L14)
- [tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs:31](../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L31)
- [tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs:8](../../../../tests/TutoringCentre.Architecture.Tests/CqrsRuleTests.cs#L8)
- [tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs:52](../../../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs#L52)
- [tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs:20](../../../../tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs#L20)
- [tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs:49](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L49)
- [tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs:8](../../../../tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs#L8)
- [tests/TutoringCentre.Architecture.Tests/TenantScopeRuleTests.cs:33](../../../../tests/TutoringCentre.Architecture.Tests/TenantScopeRuleTests.cs#L33)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Reflection;

namespace TutoringCentre.Architecture.Tests;

internal static class SourceAssemblies
{
    public static Assembly Domain => typeof(TutoringCentre.Domain.AssemblyMarker).Assembly;

    public static Assembly Application => typeof(TutoringCentre.Application.AssemblyMarker).Assembly;

    public static Assembly Infrastructure => typeof(TutoringCentre.Infrastructure.AssemblyMarker).Assembly;

    public static Assembly Api => typeof(Program).Assembly;

    public static IReadOnlyList<Assembly> All => [Domain, Application, Infrastructure, Api];
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
