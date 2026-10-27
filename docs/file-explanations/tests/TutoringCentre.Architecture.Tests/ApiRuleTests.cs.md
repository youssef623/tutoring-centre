# `tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs`

## Purpose

Declares `ApiRuleTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 62 lines and 3,056 bytes. Source hash: `c1a461aa82be1c8f8381ef9e34555527fec3c29c34c1cd82d4c353e58477bf09`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using NetArchTest.Rules;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs#L5): `public sealed class ApiRuleTests`
- [Line 12](../../../../tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs#L12): `public void Endpoints_DoNotDependOnInfrastructureRepositoriesOrDomainEntities()`
- [Line 52](../../../../tests/TutoringCentre.Architecture.Tests/ApiRuleTests.cs#L52): `public void OnlyProgramAndCli_ReferenceInfrastructure()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using NetArchTest.Rules;

namespace TutoringCentre.Architecture.Tests;

public sealed class ApiRuleTests
{
    private const string EndpointsNamespace = "TutoringCentre.Api.Endpoints";
    private const string InfrastructureNamespace = "TutoringCentre.Infrastructure";
    private const string CliNamespace = "TutoringCentre.Api.Cli";

    [Fact]
    public void Endpoints_DoNotDependOnInfrastructureRepositoriesOrDomainEntities()
    {
        var repositoryInterfaces = SourceAssemblies.Application.GetTypes()
            .Where(ArchitectureSupport.IsRepositoryInterface)
            .Select(type => type.FullName!)
            .ToArray();

        // Every Domain type except Common: endpoints legitimately map Result/Error, never entities or value objects.
        // Named per type, not per namespace, so StaffRole (Day 30 contract), SupportedLocale (Day 32 contract)
        // and ParentRelationship (Day 41 contract) can be carved out on their own: pure enums, not entities,
        // bound straight off the wire so an unknown value is refused at binding (400) instead of reaching a
        // handler. Their namespace-mates (Membership, MembershipStatus, Permissions, RolePermissions, Centre,
        // Student, Parent, StudentParentLink, StudentStatus) stay forbidden.
        var domainEntityTypes = SourceAssemblies.Domain.GetTypes()
            .Where(type => type.Namespace is not null
                && type.Namespace != "TutoringCentre.Domain"
                && type.Namespace != "TutoringCentre.Domain.Common"
                && !type.Namespace.StartsWith("TutoringCentre.Domain.Common.", StringComparison.Ordinal)
                && type.FullName != "TutoringCentre.Domain.Identity.StaffRole"
                && type.FullName != "TutoringCentre.Domain.Centres.SupportedLocale"
                && type.FullName != "TutoringCentre.Domain.Academics.ParentRelationship")
            .Select(type => type.FullName!)
            .ToArray();

        var endpointTypes = Types.InAssembly(SourceAssemblies.Api).That().ResideInNamespace(EndpointsNamespace).GetTypes().ToList();
        Assert.NotEmpty(endpointTypes);
        Assert.NotEmpty(repositoryInterfaces);
        Assert.NotEmpty(domainEntityTypes);

        var forbidden = new[] { InfrastructureNamespace }.Concat(repositoryInterfaces).Concat(domainEntityTypes).ToArray();

        var result = Types.InAssembly(SourceAssemblies.Api)
            .That().ResideInNamespace(EndpointsNamespace)
            .ShouldNot().HaveDependencyOnAny(forbidden)
            .GetResult();

        Assert.True(result.IsSuccessful, ArchitectureSupport.Describe(result));
    }

    [Fact]
    public void OnlyProgramAndCli_ReferenceInfrastructure()
    {
        var result = Types.InAssembly(SourceAssemblies.Api)
            .That().DoNotHaveName("Program")
            .And().DoNotResideInNamespace(CliNamespace)
            .ShouldNot().HaveDependencyOn(InfrastructureNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, ArchitectureSupport.Describe(result));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
