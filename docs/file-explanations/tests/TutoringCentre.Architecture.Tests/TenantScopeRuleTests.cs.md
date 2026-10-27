# `tests/TutoringCentre.Architecture.Tests/TenantScopeRuleTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 22.4: every command or query inside a tenant module must declare `ITenantScoped` (Task 22.1), so the dispatcher's layer-1 step (Task 22.2) can never be skipped by a use case over tenant data. The inverse guard pins that Month 1's non-tenant namespaces (Identity, Centres, Platform) never declare it. Matching is by implemented interface, never by class-name suffix.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/TenantScopeRuleTests.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 71 lines and 3,275 bytes. Source hash: `d964707569ee79301ea83c38f7917cff42b583f1d9b326a61881636aaee2cca7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../tests/TutoringCentre.Architecture.Tests/TenantScopeRuleTests.cs#L11): `public sealed class TenantScopeRuleTests`
- [Line 36](../../../../tests/TutoringCentre.Architecture.Tests/TenantScopeRuleTests.cs#L36): `public void EveryRequestInATenantModule_DeclaresTenantScoped()`
- [Line 47](../../../../tests/TutoringCentre.Architecture.Tests/TenantScopeRuleTests.cs#L47): `public void EveryMonth1Request_DoesNotDeclareTenantScoped()`
- [Line 62](../../../../tests/TutoringCentre.Architecture.Tests/TenantScopeRuleTests.cs#L62): `private static IEnumerable<Type> Requests() =>`
- [Line 66](../../../../tests/TutoringCentre.Architecture.Tests/TenantScopeRuleTests.cs#L66): `private static bool IsInNamespaces(Type type, string[] namespaces) =>`
- [Line 69](../../../../tests/TutoringCentre.Architecture.Tests/TenantScopeRuleTests.cs#L69): `private static bool Implements(Type type, Type genericDefinition) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Architecture.Tests;

/// <summary>
/// Task 22.4: every command or query inside a tenant module must declare <see cref="ITenantScoped"/>
/// (Task 22.1), so the dispatcher's layer-1 step (Task 22.2) can never be skipped by a use case over tenant
/// data. The inverse guard pins that Month 1's non-tenant namespaces (Identity, Centres, Platform) never
/// declare it. Matching is by implemented interface, never by class-name suffix.
/// </summary>
public sealed class TenantScopeRuleTests
{
    // The tenant-module Application namespaces this rule polices; add one line as each later module (Month 3+)
    // is created. Centre settings (Day 32) is tenant-scoped even though its sibling CreateCentre is not, so it
    // is named precisely here rather than by the whole "...Centres" prefix, which stays in NonTenantNamespaces
    // for CreateCentre alone.
    private static readonly string[] TenantModuleNamespaces =
    [
        "TutoringCentre.Application.Academics",
        "TutoringCentre.Application.Staff",
        "TutoringCentre.Application.Centres.Queries.GetCentreSettings",
        "TutoringCentre.Application.Centres.Commands.UpdateCentreSettings",
        "TutoringCentre.Application.Audit",
    ];

    private static readonly string[] NonTenantNamespaces =
    [
        "TutoringCentre.Application.Identity",
        "TutoringCentre.Application.Centres.Commands.CreateCentre",
        "TutoringCentre.Application.Platform",
    ];

    private static readonly Type[] ApplicationTypes = SourceAssemblies.Application.GetTypes();

    [Fact]
    public void EveryRequestInATenantModule_DeclaresTenantScoped()
    {
        var violations = Requests()
            .Where(request => IsInNamespaces(request, TenantModuleNamespaces) && !typeof(ITenantScoped).IsAssignableFrom(request))
            .Select(request => request.FullName)
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void EveryMonth1Request_DoesNotDeclareTenantScoped()
    {
        var nonTenantRequests = Requests().Where(request => IsInNamespaces(request, NonTenantNamespaces)).ToList();

        // Guards against the namespace list drifting out of sync with the real Month 1 request namespaces,
        // which would make this test vacuously (and silently) useless.
        Assert.NotEmpty(nonTenantRequests);

        var violations = nonTenantRequests
            .Where(request => typeof(ITenantScoped).IsAssignableFrom(request))
            .Select(request => request.FullName)
            .ToList();
        Assert.Empty(violations);
    }

    private static IEnumerable<Type> Requests() =>
        ApplicationTypes.Where(type => type is { IsClass: true, IsAbstract: false }
            && (Implements(type, typeof(ICommand<>)) || Implements(type, typeof(IQuery<>))));

    private static bool IsInNamespaces(Type type, string[] namespaces) =>
        type.Namespace is not null && namespaces.Any(ns => type.Namespace.StartsWith(ns, StringComparison.Ordinal));

    private static bool Implements(Type type, Type genericDefinition) =>
        type.GetInterfaces().Any(candidate => candidate.IsGenericType && candidate.GetGenericTypeDefinition() == genericDefinition);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
