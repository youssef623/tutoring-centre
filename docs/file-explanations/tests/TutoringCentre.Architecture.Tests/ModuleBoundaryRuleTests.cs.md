# `tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 30.9: keeps the monolith modular without separate assemblies. Three modules exist today, by Application and Domain namespace: Platform (with Centres), Identity (with Staff), Academics. A module reaches another only through its requests (commands, queries, DTOs — explicitly allowed) or an explicit published interface in that module's own Contracts namespace (Task 43.3, e.g. TutoringCentre.Application.Identity.Contracts); it may never reach into another module's repository, read service, handler, validator or Domain entity directly. A Contracts namespace is itself narrow by construction: every type in one must be an interface or a record, and may not depend on its own module's repositories, read services or Domain entities — a contract publishes a question another module may ask, never a door into how it is answered.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 193 lines and 10,638 bytes. Source hash: `2ea575e420d96de14c2314b8b7367f14cfd8da4c17f362cfd1a324570a6d872c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Reflection;`
- Line 2: `using NetArchTest.Rules;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L13): `/// interface or a record, and may not depend on its own module's repositories, read services or Domain`
- [Line 16](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L16): `public sealed class ModuleBoundaryRuleTests`
- [Line 53](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L53): `public void ApplicationTypes_DoNotDependOnAnotherModulesRepositoriesReadServicesHandlersOrValidators()`
- [Line 59](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L59): `public void ApplicationTypes_DoNotDependOnAnotherModulesDomainTypes()`
- [Line 65](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L65): `public void ContractTypes_AreInterfacesOrRecords()`
- [Line 73](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L73): `Assert.True(violations.Length == 0, "Contracts-namespace type must be an interface or a record: " + string.Join(", ", violations));`
- [Line 77](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L77): `public void ContractTypes_DoNotDependOnTheirOwnModulesPortsOrDomainTypes()`
- [Line 117](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L117): `private static Type[] ContractTypes() =>`
- [Line 120](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L120): `/// <summary>Record class check: the compiler always synthesises a protected &lt;Clone&gt;$ method for one. This codebase's DTOs and contracts are reference-type records, never record structs.</summary>`
- [Line 121](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L121): `private static bool IsRecord(Type type) =>`
- [Line 125](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L125): `private static void AssertNoDependencyOnOtherModulesPorts(string moduleName, string[] ownNamespaces)`
- [Line 140](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L140): `private static void AssertNoDependencyOnOtherModulesDomainTypes(string moduleName, string[] ownApplicationNamespaces)`
- [Line 155](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L155): `private static TestResult RunDependencyCheck(string[] namespaces, string[] forbidden)`
- [Line 166](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L166): `private static bool IsReadServiceInterface(Type type) =>`
- [Line 169](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L169): `private static bool IsHandler(Type type) =>`
- [Line 172](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L172): `private static bool IsHandlerInterface(Type candidate) =>`
- [Line 177](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L177): `private static bool IsValidator(Type type) =>`
- [Line 180](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L180): `private static bool IsSharedKernelApplicationType(Type type) =>`
- [Line 185](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L185): `private static bool IsSharedKernelDomainType(Type type) =>`
- [Line 191](../../../../tests/TutoringCentre.Architecture.Tests/ModuleBoundaryRuleTests.cs#L191): `private static bool IsInAnyNamespace(Type type, string[] namespaces) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/assemble.py:27](../../../onboarding-work/assemble.py#L27)
- [docs/onboarding-work/assemble.py:154](../../../onboarding-work/assemble.py#L154)
- [docs/onboarding-work/extend_backend_purposes.py:10](../../../onboarding-work/extend_backend_purposes.py#L10)
- [docs/onboarding-work/extend_backend_purposes.py:13](../../../onboarding-work/extend_backend_purposes.py#L13)
- [docs/onboarding-work/file_guides.py:24](../../../onboarding-work/file_guides.py#L24)
- [docs/onboarding-work/inspect-tests.py:5](../../../onboarding-work/inspect-tests.py#L5)
- [docs/onboarding-work/inventory.py:17](../../../onboarding-work/inventory.py#L17)
- [docs/onboarding-work/normalize_docs.py:9](../../../onboarding-work/normalize_docs.py#L9)
- [docs/onboarding-work/reconcile_backend_ledger.py:17](../../../onboarding-work/reconcile_backend_ledger.py#L17)
- [docs/onboarding-work/validate_docs.py:39](../../../onboarding-work/validate_docs.py#L39)
- [frontend/e2e/auth.spec.ts:57](../../../../frontend/e2e/auth.spec.ts#L57)
- [frontend/e2e/schedule.spec.ts:74](../../../../frontend/e2e/schedule.spec.ts#L74)
- [frontend/scripts/check-i18n-keys.mjs:65](../../../../frontend/scripts/check-i18n-keys.mjs#L65)
- [frontend/src/api/apiFetch.ts:77](../../../../frontend/src/api/apiFetch.ts#L77)
- [frontend/src/api/fieldErrors.ts:7](../../../../frontend/src/api/fieldErrors.ts#L7)
- [frontend/src/components/shared/DataTable.tsx:43](../../../../frontend/src/components/shared/DataTable.tsx#L43)
- [frontend/src/features/audit/AuditLog.tsx:169](../../../../frontend/src/features/audit/AuditLog.tsx#L169)
- [frontend/src/features/enrollments/GroupRosterTab.test.tsx:88](../../../../frontend/src/features/enrollments/GroupRosterTab.test.tsx#L88)
- [frontend/src/features/enrollments/SeatMeter.tsx:5](../../../../frontend/src/features/enrollments/SeatMeter.tsx#L5)
- [frontend/src/features/enrollments/enrollmentTestData.ts:27](../../../../frontend/src/features/enrollments/enrollmentTestData.ts#L27)
- 45 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Reflection;
using NetArchTest.Rules;

namespace TutoringCentre.Architecture.Tests;

/// <summary>
/// Task 30.9: keeps the monolith modular without separate assemblies. Three modules exist today, by Application
/// and Domain namespace: Platform (with Centres), Identity (with Staff), Academics. A module reaches another
/// only through its requests (commands, queries, DTOs — explicitly allowed) or an explicit published interface
/// in that module's own <c>Contracts</c> namespace (Task 43.3, e.g. <c>TutoringCentre.Application.Identity.Contracts</c>);
/// it may never reach into another module's repository, read service, handler, validator or Domain entity
/// directly. A <c>Contracts</c> namespace is itself narrow by construction: every type in one must be an
/// interface or a record, and may not depend on its own module's repositories, read services or Domain
/// entities — a contract publishes a question another module may ask, never a door into how it is answered.
/// </summary>
public sealed class ModuleBoundaryRuleTests
{
    /// <summary>The module map, by Application namespace prefix. Declared once; every other namespace not listed here and not shared kernel is a bug in this list, not a loophole.</summary>
    private static readonly Dictionary<string, string[]> ApplicationModuleNamespaces = new()
    {
        ["Platform"] = ["TutoringCentre.Application.Platform", "TutoringCentre.Application.Centres"],
        ["Identity"] = ["TutoringCentre.Application.Identity", "TutoringCentre.Application.Staff"],
        ["Academics"] = ["TutoringCentre.Application.Academics"],
        ["Audit"] = ["TutoringCentre.Application.Audit"],
    };

    /// <summary>The same modules, by Domain namespace. Audit has no Domain types of its own (Task 31: it is read-only from the application's point of view), so its entry names a namespace nothing occupies.</summary>
    private static readonly Dictionary<string, string[]> DomainModuleNamespaces = new()
    {
        ["Platform"] = ["TutoringCentre.Domain.Centres"],
        ["Identity"] = ["TutoringCentre.Domain.Identity"],
        ["Academics"] = ["TutoringCentre.Domain.Academics"],
        ["Audit"] = ["TutoringCentre.Domain.Audit"],
    };

    /// <summary>Domain and Application common namespaces: available to every module, never counted as "another module's".</summary>
    private static readonly string[] SharedKernelNamespacePrefixes =
    [
        "TutoringCentre.Application.Common",
        "TutoringCentre.Domain.Common",
    ];

    /// <summary>
    /// Exactly these cross-cutting types, named individually rather than by namespace, so the exception stays
    /// as small as the contract: growing this list to make a test pass is the one thing not allowed here.
    /// </summary>
    private static readonly string[] SharedKernelTypeNames = ["SupportedLocale", "StaffRole", "Permissions", "RolePermissions", "RoleScopes"];

    private static readonly Type[] ApplicationTypes = SourceAssemblies.Application.GetTypes();
    private static readonly Type[] DomainTypes = SourceAssemblies.Domain.GetTypes();

    [Fact]
    public void ApplicationTypes_DoNotDependOnAnotherModulesRepositoriesReadServicesHandlersOrValidators()
    {
        Assert.All(ApplicationModuleNamespaces, module => AssertNoDependencyOnOtherModulesPorts(module.Key, module.Value));
    }

    [Fact]
    public void ApplicationTypes_DoNotDependOnAnotherModulesDomainTypes()
    {
        Assert.All(ApplicationModuleNamespaces, module => AssertNoDependencyOnOtherModulesDomainTypes(module.Key, module.Value));
    }

    [Fact]
    public void ContractTypes_AreInterfacesOrRecords()
    {
        var contractTypes = ContractTypes();

        // Guards against the allowance being untested if nothing ever publishes a contract.
        Assert.NotEmpty(contractTypes);

        var violations = contractTypes.Where(type => !type.IsInterface && !IsRecord(type)).Select(type => type.FullName).ToArray();
        Assert.True(violations.Length == 0, "Contracts-namespace type must be an interface or a record: " + string.Join(", ", violations));
    }

    [Fact]
    public void ContractTypes_DoNotDependOnTheirOwnModulesPortsOrDomainTypes()
    {
        var contractTypes = ContractTypes();
        Assert.NotEmpty(contractTypes);

        foreach (var module in ApplicationModuleNamespaces)
        {
            var ownContractNamespaces = contractTypes
                .Where(type => IsInAnyNamespace(type, module.Value))
                .Select(type => type.Namespace!)
                .Distinct(StringComparer.Ordinal)
                .ToArray();
            if (ownContractNamespaces.Length == 0)
            {
```

The excerpt stops at line 90; use the original file for the remaining 103 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
