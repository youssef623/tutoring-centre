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

    private static IEnumerable<Type> Requests() =>
        ApplicationTypes.Where(type => type is { IsClass: true, IsAbstract: false }
            && (Implements(type, typeof(ICommand<>)) || Implements(type, typeof(IQuery<>))));

    private static bool Implements(Type type, Type genericDefinition) =>
        type.GetInterfaces().Any(candidate => candidate.IsGenericType && candidate.GetGenericTypeDefinition() == genericDefinition);

    private static bool IsCommandOrQueryFileUnderApplication(string relativePath) =>
        relativePath.StartsWith("src/TutoringCentre.Application/", StringComparison.Ordinal)
        && (relativePath.Contains("/Commands/", StringComparison.Ordinal) || relativePath.Contains("/Queries/", StringComparison.Ordinal));
}
