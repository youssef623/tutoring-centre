# `tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs`

## Purpose

Declares `RepositoryRuleTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 40 lines and 1,424 bytes. Source hash: `127a52a3b67788a8d10ddc29da51474cd41f7b21aa886788023ad43ceee65c3e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 3](../../../../tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs#L3): `public sealed class RepositoryRuleTests`
- [Line 6](../../../../tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs#L6): `public void RepositoryInterfaces_LiveInTheApplicationProject()`
- [Line 24](../../../../tests/TutoringCentre.Architecture.Tests/RepositoryRuleTests.cs#L24): `public void RepositoryClasses_LiveInTheInfrastructureProject()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Architecture.Tests;

public sealed class RepositoryRuleTests
{
    [Fact]
    public void RepositoryInterfaces_LiveInTheApplicationProject()
    {
        var interfaces = SourceAssemblies.All
            .SelectMany(assembly => assembly.GetTypes())
            .Where(ArchitectureSupport.IsRepositoryInterface)
            .ToList();

        Assert.NotEmpty(interfaces);
        var violations = interfaces
            .Where(type => type.Assembly != SourceAssemblies.Application
                || type.Namespace is null
                || !type.Namespace.StartsWith("TutoringCentre.Application", StringComparison.Ordinal))
            .Select(type => type.FullName)
            .ToList();
        Assert.Empty(violations);
    }

    [Fact]
    public void RepositoryClasses_LiveInTheInfrastructureProject()
    {
        var classes = SourceAssemblies.All
            .SelectMany(assembly => assembly.GetTypes())
            .Where(ArchitectureSupport.IsRepositoryClass)
            .ToList();

        Assert.NotEmpty(classes);
        var violations = classes
            .Where(type => type.Assembly != SourceAssemblies.Infrastructure
                || type.Namespace is null
                || !type.Namespace.StartsWith("TutoringCentre.Infrastructure", StringComparison.Ordinal))
            .Select(type => type.FullName)
            .ToList();
        Assert.Empty(violations);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
