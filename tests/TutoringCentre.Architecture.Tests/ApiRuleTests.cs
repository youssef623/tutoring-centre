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
        // Named per type, not per namespace, so StaffRole (Day 30 contract) can be carved out on its own: a pure
        // enum, not an entity, bound straight off the wire so an unknown role value is refused at binding (400)
        // instead of reaching a handler. Its namespace-mates (Membership, MembershipStatus, Permissions,
        // RolePermissions) stay forbidden.
        var domainEntityTypes = SourceAssemblies.Domain.GetTypes()
            .Where(type => type.Namespace is not null
                && type.Namespace != "TutoringCentre.Domain"
                && type.Namespace != "TutoringCentre.Domain.Common"
                && !type.Namespace.StartsWith("TutoringCentre.Domain.Common.", StringComparison.Ordinal)
                && type.FullName != "TutoringCentre.Domain.Identity.StaffRole")
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
