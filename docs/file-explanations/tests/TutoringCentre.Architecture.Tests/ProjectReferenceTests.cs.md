# `tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Declared-reference rule: fails when a .csproj DECLARES a reference outside the allowed set, even if no code uses it (the compiler drops unused references, so DependencyRuleTests can't see them).

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 1,438 bytes. Source hash: `b69f351fca19eb19972b987d26b25eef43a848ea760cd2dc120826988a540bc2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs#L5): `public sealed class ProjectReferenceTests`
- [Line 14](../../../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs#L14): `public void Domain_ReferencesNoProjects() =>`
- [Line 18](../../../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs#L18): `public void Application_ReferencesOnlyDomain() =>`
- [Line 22](../../../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs#L22): `public void Infrastructure_ReferencesOnlyApplicationAndDomain() =>`
- [Line 26](../../../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs#L26): `public void Api_ReferencesOnlyApplicationAndInfrastructure() =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Architecture.Tests;

/// <summary>Declared-reference rule: fails when a .csproj DECLARES a reference outside the allowed set,
/// even if no code uses it (the compiler drops unused references, so DependencyRuleTests can't see them).</summary>
public sealed class ProjectReferenceTests
{
    // Typed from diagram B. Sorted ordinally.
    private static readonly string[] DomainAllowed = [];
    private static readonly string[] ApplicationAllowed = ["TutoringCentre.Domain"];
    private static readonly string[] InfrastructureAllowed = ["TutoringCentre.Application", "TutoringCentre.Domain"];
    private static readonly string[] ApiAllowed = ["TutoringCentre.Application", "TutoringCentre.Infrastructure"];

    [Fact]
    public void Domain_ReferencesNoProjects() =>
        Assert.Equal(DomainAllowed, ProjectFiles.ReadProjectReferences("TutoringCentre.Domain"));

    [Fact]
    public void Application_ReferencesOnlyDomain() =>
        Assert.Equal(ApplicationAllowed, ProjectFiles.ReadProjectReferences("TutoringCentre.Application"));

    [Fact]
    public void Infrastructure_ReferencesOnlyApplicationAndDomain() =>
        Assert.Equal(InfrastructureAllowed, ProjectFiles.ReadProjectReferences("TutoringCentre.Infrastructure"));

    [Fact]
    public void Api_ReferencesOnlyApplicationAndInfrastructure() =>
        Assert.Equal(ApiAllowed, ProjectFiles.ReadProjectReferences("TutoringCentre.Api"));
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
