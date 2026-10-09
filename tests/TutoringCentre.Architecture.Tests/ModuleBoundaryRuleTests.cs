using NetArchTest.Rules;

namespace TutoringCentre.Architecture.Tests;

/// <summary>
/// Task 30.9: keeps the monolith modular without separate assemblies. Three modules exist today, by Application
/// and Domain namespace: Platform (with Centres), Identity (with Staff), Academics. A module reaches another
/// only through its requests (commands, queries, DTOs — explicitly allowed) or an explicit Application
/// interface it is handed; it may never reach into another module's repository, read service, handler,
/// validator or Domain entity directly.
/// </summary>
public sealed class ModuleBoundaryRuleTests
{
    /// <summary>The module map, by Application namespace prefix. Declared once; every other namespace not listed here and not shared kernel is a bug in this list, not a loophole.</summary>
    private static readonly Dictionary<string, string[]> ApplicationModuleNamespaces = new()
    {
        ["Platform"] = ["TutoringCentre.Application.Platform", "TutoringCentre.Application.Centres"],
        ["Identity"] = ["TutoringCentre.Application.Identity", "TutoringCentre.Application.Staff"],
        ["Academics"] = ["TutoringCentre.Application.Academics"],
    };

    /// <summary>The same three modules, by Domain namespace.</summary>
    private static readonly Dictionary<string, string[]> DomainModuleNamespaces = new()
    {
        ["Platform"] = ["TutoringCentre.Domain.Centres"],
        ["Identity"] = ["TutoringCentre.Domain.Identity"],
        ["Academics"] = ["TutoringCentre.Domain.Academics"],
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
    private static readonly string[] SharedKernelTypeNames = ["SupportedLocale", "StaffRole", "Permissions", "RolePermissions"];

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

    private static void AssertNoDependencyOnOtherModulesPorts(string moduleName, string[] ownNamespaces)
    {
        var forbidden = ApplicationTypes
            .Where(type => !IsInAnyNamespace(type, ownNamespaces) && !IsSharedKernelApplicationType(type))
            .Where(type => ArchitectureSupport.IsRepositoryInterface(type) || IsReadServiceInterface(type) || IsHandler(type) || IsValidator(type))
            .Select(type => type.FullName!)
            .ToArray();

        // Guards against every "other module" namespace list being empty, which would make this pass vacuously.
        Assert.True(forbidden.Length > 0, $"No other-module ports found while checking module '{moduleName}' — the module map has likely drifted from the real namespaces.");

        var result = RunDependencyCheck(ownNamespaces, forbidden);
        Assert.True(result.IsSuccessful, $"Module '{moduleName}': {ArchitectureSupport.Describe(result)}");
    }

    private static void AssertNoDependencyOnOtherModulesDomainTypes(string moduleName, string[] ownApplicationNamespaces)
    {
        var ownDomainNamespaces = DomainModuleNamespaces[moduleName];
        var forbidden = DomainTypes
            .Where(type => !IsInAnyNamespace(type, ownDomainNamespaces) && !IsSharedKernelDomainType(type))
            .Select(type => type.FullName!)
            .ToArray();

        Assert.True(forbidden.Length > 0, $"No other-module Domain types found while checking module '{moduleName}' — the module map has likely drifted from the real namespaces.");

        var result = RunDependencyCheck(ownApplicationNamespaces, forbidden);
        Assert.True(result.IsSuccessful, $"Module '{moduleName}': {ArchitectureSupport.Describe(result)}");
    }

    /// <summary>Every Application type belonging to this module (across however many namespace prefixes it has) must not depend on any of <paramref name="forbidden"/>.</summary>
    private static TestResult RunDependencyCheck(string[] namespaces, string[] forbidden)
    {
        var condition = Types.InAssembly(SourceAssemblies.Application).That().ResideInNamespaceStartingWith(namespaces[0]);
        for (var i = 1; i < namespaces.Length; i++)
        {
            condition = condition.Or().ResideInNamespaceStartingWith(namespaces[i]);
        }

        return condition.ShouldNot().HaveDependencyOnAny(forbidden).GetResult();
    }

    private static bool IsReadServiceInterface(Type type) =>
        type.IsInterface && type.Name.Length > "IReadService".Length && type.Name.StartsWith('I') && type.Name.EndsWith("ReadService", StringComparison.Ordinal);

    private static bool IsHandler(Type type) =>
        type is { IsClass: true, IsAbstract: false } && type.GetInterfaces().Any(IsHandlerInterface);

    private static bool IsHandlerInterface(Type candidate) =>
        candidate.IsGenericType
        && (candidate.GetGenericTypeDefinition() == typeof(TutoringCentre.Application.Common.Cqrs.ICommandHandler<,>)
            || candidate.GetGenericTypeDefinition() == typeof(TutoringCentre.Application.Common.Cqrs.IQueryHandler<,>));

    private static bool IsValidator(Type type) =>
        type is { IsClass: true, IsAbstract: false } && type.Name.EndsWith("Validator", StringComparison.Ordinal);

    private static bool IsSharedKernelApplicationType(Type type) =>
        type.Namespace is null
        || type.Namespace == "TutoringCentre.Application"
        || SharedKernelNamespacePrefixes.Any(prefix => type.Namespace.StartsWith(prefix, StringComparison.Ordinal));

    private static bool IsSharedKernelDomainType(Type type) =>
        type.Namespace is null
        || type.Namespace == "TutoringCentre.Domain"
        || SharedKernelNamespacePrefixes.Any(prefix => type.Namespace.StartsWith(prefix, StringComparison.Ordinal))
        || SharedKernelTypeNames.Contains(type.Name);

    private static bool IsInAnyNamespace(Type type, string[] namespaces) =>
        type.Namespace is not null && namespaces.Any(ns => type.Namespace.StartsWith(ns, StringComparison.Ordinal));
}
