# `tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Type-level dependency rule: fails when compiled code in a layer USES a forbidden namespace.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 73 lines and 2,609 bytes. Source hash: `18afe758a80ea2883057ea92c951fec686f2273d84695ccc5ea88bd1ce81e53d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Reflection;`
- Line 2: `using NetArchTest.Rules;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs#L7): `public sealed class DependencyRuleTests`
- [Line 38](../../../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs#L38): `public void Domain_DoesNotDependOnOtherLayersOrFrameworks() =>`
- [Line 42](../../../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs#L42): `public void Application_DoesNotDependOnInfrastructureApiOrFrameworks() =>`
- [Line 46](../../../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs#L46): `public void Infrastructure_DoesNotDependOnApi() =>`
- [Line 50](../../../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs#L50): `public void Application_DoesNotDependOnSerilogHangfireOrHttpClient()`
- [Line 60](../../../../tests/TutoringCentre.Architecture.Tests/DependencyRuleTests.cs#L60): `private static void AssertNoDependencies(Assembly assembly, string[] forbidden)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Reflection;
using NetArchTest.Rules;

namespace TutoringCentre.Architecture.Tests;

/// <summary>Type-level dependency rule: fails when compiled code in a layer USES a forbidden namespace.</summary>
public sealed class DependencyRuleTests
{
    private static readonly Assembly DomainAssembly = typeof(TutoringCentre.Domain.AssemblyMarker).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(TutoringCentre.Application.AssemblyMarker).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(TutoringCentre.Infrastructure.AssemblyMarker).Assembly;

    private static readonly string[] ForbiddenForDomain =
    [
        "TutoringCentre.Application",
        "TutoringCentre.Infrastructure",
        "TutoringCentre.Api",
        "Microsoft.EntityFrameworkCore",
        "Microsoft.AspNetCore",
        "Npgsql",
    ];

    private static readonly string[] ForbiddenForApplication =
    [
        "TutoringCentre.Infrastructure",
        "TutoringCentre.Api",
        "Microsoft.EntityFrameworkCore",
        "Microsoft.AspNetCore",
        "Npgsql",
    ];

    private static readonly string[] ForbiddenForInfrastructure =
    [
        "TutoringCentre.Api",
    ];

    [Fact]
    public void Domain_DoesNotDependOnOtherLayersOrFrameworks() =>
        AssertNoDependencies(DomainAssembly, ForbiddenForDomain);

    [Fact]
    public void Application_DoesNotDependOnInfrastructureApiOrFrameworks() =>
        AssertNoDependencies(ApplicationAssembly, ForbiddenForApplication);

    [Fact]
    public void Infrastructure_DoesNotDependOnApi() =>
        AssertNoDependencies(InfrastructureAssembly, ForbiddenForInfrastructure);

    [Fact]
    public void Application_DoesNotDependOnSerilogHangfireOrHttpClient()
    {
        var result = Types.InAssembly(SourceAssemblies.Application)
            .ShouldNot()
            .HaveDependencyOnAny("Serilog", "Hangfire", "System.Net.Http")
            .GetResult();

        Assert.True(result.IsSuccessful, ArchitectureSupport.Describe(result));
    }

    private static void AssertNoDependencies(Assembly assembly, string[] forbidden)
    {
        var types = Types.InAssembly(assembly);

        // Guards against scanning the wrong (or an empty) assembly, which would always pass.
        Assert.NotEmpty(types.GetTypes());

        var result = types.ShouldNot().HaveDependencyOnAny(forbidden).GetResult();

        Assert.True(
            result.IsSuccessful,
            $"{assembly.GetName().Name} has forbidden dependencies in: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
