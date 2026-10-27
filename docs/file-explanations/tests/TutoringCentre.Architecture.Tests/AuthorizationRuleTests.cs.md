# `tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 28.7: forgetting to declare a permission, or declaring a misspelt one, becomes a failing build. Rule 1 and 2 use reflection (NetArchTest-style, like `TenantScopeRuleTests`); rule 3 is a source scan (like `QueryFilterBypassTests`), since "does a handler branch on a role" is not something compiled-type inspection alone can see.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 101 lines and 4,775 bytes. Source hash: `6ee068d01a695ca5719dd8a2d2008c10ed96ad245218ce5aafaeff434357c0d6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Runtime.CompilerServices;`
- Line 2: `using System.Text.RegularExpressions;`
- Line 3: `using TutoringCentre.Application.Common.Cqrs;`
- Line 4: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 14](../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L14): `public sealed class AuthorizationRuleTests`
- [Line 34](../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L34): `public void EveryTenantScopedRequest_AlsoDeclaresIRequirePermission()`
- [Line 45](../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L45): `public void EveryRequirePermissionRequest_DeclaresAKeyFromTheCatalogue()`
- [Line 61](../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L61): `public void ApplicationCommandAndQueryHandlers_NeverBranchOnAStaffRoleValue()`
- [Line 88](../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L88): `private static string RequiredPermissionOf(Type requestType) =>`
- [Line 91](../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L91): `private static IEnumerable<Type> Requests() =>`
- [Line 95](../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L95): `private static bool Implements(Type type, Type genericDefinition) =>`
- [Line 98](../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L98): `private static bool IsCommandOrQueryFileUnderApplication(string relativePath) =>`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Architecture.Tests;

/// <summary>
/// Task 28.7: forgetting to declare a permission, or declaring a misspelt one, becomes a failing build.
/// Rule 1 and 2 use reflection (NetArchTest-style, like <see cref="TenantScopeRuleTests"/>); rule 3 is a
/// source scan (like <see cref="QueryFilterBypassTests"/>), since "does a handler branch on a role" is not
/// something compiled-type inspection alone can see.
/// </summary>
public sealed class AuthorizationRuleTests
{
    // Day 29's staff-management handlers handle roles as data (the last-owner guard, the entity's own
    // role-change rule), not as an authorization decision — that distinction is what this allow-list exists
    // to keep narrow and explicit, one named file at a time.
    private static readonly string[] RoleReferenceAllowList =
    [
        "src/TutoringCentre.Application/Staff/Commands/ChangeStaffRole/ChangeStaffRoleHandler.cs",
        "src/TutoringCentre.Application/Staff/Commands/DeactivateStaff/DeactivateStaffHandler.cs",

        // Task 43.8: passes StaffRole.Teacher to IStaffDirectory as the role being asked about, not branched
        // on for an authorization decision of its own.
        "src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs",
    ];

    private static readonly Regex StaffRoleMemberPattern = new(@"\bStaffRole\.(Owner|Secretary|Teacher)\b", RegexOptions.Compiled);

    private static readonly Type[] ApplicationTypes = SourceAssemblies.Application.GetTypes();

    [Fact]
    public void EveryTenantScopedRequest_AlsoDeclaresIRequirePermission()
    {
        var violations = Requests()
            .Where(request => typeof(ITenantScoped).IsAssignableFrom(request) && !typeof(IRequirePermission).IsAssignableFrom(request))
            .Select(request => request.FullName)
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void EveryRequirePermissionRequest_DeclaresAKeyFromTheCatalogue()
    {
        var permissionRequests = ApplicationTypes
            .Where(type => type is { IsClass: true, IsAbstract: false } && typeof(IRequirePermission).IsAssignableFrom(type))
            .ToList();
        Assert.NotEmpty(permissionRequests);

        var violations = permissionRequests
            .Where(type => !Permissions.All.Contains(RequiredPermissionOf(type)))
            .Select(type => type.FullName)
            .ToList();

        Assert.Empty(violations);
    }

    [Fact]
    public void ApplicationCommandAndQueryHandlers_NeverBranchOnAStaffRoleValue()
    {
        var files = SourceFiles.EnumerateProductionSourceFiles()
            .Select(path => (AbsolutePath: path, RelativePath: SourceFiles.ToRepositoryRelativePath(path)))
            .Where(file => IsCommandOrQueryFileUnderApplication(file.RelativePath))
            .ToList();
        Assert.NotEmpty(files);

        var violations = new List<string>();
        foreach (var (absolutePath, relativePath) in files)
        {
            if (RoleReferenceAllowList.Contains(relativePath, StringComparer.Ordinal))
            {
                continue;
            }

            var text = File.ReadAllText(absolutePath);
            if (StaffRoleMemberPattern.IsMatch(text))
            {
                violations.Add(relativePath);
            }
        }

        Assert.True(violations.Count == 0, "Staff-role member reference outside the allow-list in: " + string.Join(", ", violations));
    }

    /// <summary>Reads the permission off an uninitialized instance — safe because every implementation returns a constant, never a value read from instance state.</summary>
    private static string RequiredPermissionOf(Type requestType) =>
        ((IRequirePermission)RuntimeHelpers.GetUninitializedObject(requestType)).RequiredPermission;

```

The excerpt stops at line 90; use the original file for the remaining 11 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
