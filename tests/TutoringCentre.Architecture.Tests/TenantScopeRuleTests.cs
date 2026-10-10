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
