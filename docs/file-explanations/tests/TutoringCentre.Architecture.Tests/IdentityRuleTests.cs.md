# `tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Identity (UserManager, ApplicationUser, password hashing) is an Infrastructure concern. Application and the Api — other than the composition root (Program.cs) and the CLI (which wires up Identity at startup and for the development seeder) — see it only through Application's own ports (IAuthenticationService, IMembershipReadService), never directly.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 1,662 bytes. Source hash: `dc67dee5fd77d6447c8305dc7f01e8f5d06c66c38b42efcd36454f451ae862ec`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using NetArchTest.Rules;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs#L11): `public sealed class IdentityRuleTests`
- [Line 18](../../../../tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs#L18): `public void Application_DoesNotDependOnIdentity()`
- [Line 31](../../../../tests/TutoringCentre.Architecture.Tests/IdentityRuleTests.cs#L31): `public void Api_OutsideCompositionRootAndCli_DoesNotDependOnIdentity()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using NetArchTest.Rules;

namespace TutoringCentre.Architecture.Tests;

/// <summary>
/// Identity (UserManager, ApplicationUser, password hashing) is an Infrastructure concern. Application and the
/// Api — other than the composition root (Program.cs) and the CLI (which wires up Identity at startup and for
/// the development seeder) — see it only through Application's own ports (IAuthenticationService,
/// IMembershipReadService), never directly.
/// </summary>
public sealed class IdentityRuleTests
{
    private const string InfrastructureIdentityNamespace = "TutoringCentre.Infrastructure.Identity";
    private const string AspNetIdentityNamespace = "Microsoft.AspNetCore.Identity";
    private const string CliNamespace = "TutoringCentre.Api.Cli";

    [Fact]
    public void Application_DoesNotDependOnIdentity()
    {
        var types = Types.InAssembly(SourceAssemblies.Application);
        Assert.NotEmpty(types.GetTypes());

        var result = types
            .ShouldNot().HaveDependencyOnAny(InfrastructureIdentityNamespace, AspNetIdentityNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, ArchitectureSupport.Describe(result));
    }

    [Fact]
    public void Api_OutsideCompositionRootAndCli_DoesNotDependOnIdentity()
    {
        var result = Types.InAssembly(SourceAssemblies.Api)
            .That().DoNotHaveName("Program")
            .And().DoNotResideInNamespace(CliNamespace)
            .ShouldNot().HaveDependencyOnAny(InfrastructureIdentityNamespace, AspNetIdentityNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, ArchitectureSupport.Describe(result));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
