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

        // Every Domain namespace except Common: endpoints legitimately map Result/Error, never entities or value objects.
        var domainEntityNamespaces = SourceAssemblies.Domain.GetTypes()
            .Select(type => type.Namespace)
            .OfType<string>()
            .Where(ns => ns != "TutoringCentre.Domain"
                && ns != "TutoringCentre.Domain.Common"
                && !ns.StartsWith("TutoringCentre.Domain.Common.", StringComparison.Ordinal))
            .Distinct()
            .ToArray();

        var endpointTypes = Types.InAssembly(SourceAssemblies.Api).That().ResideInNamespace(EndpointsNamespace).GetTypes().ToList();
        Assert.NotEmpty(endpointTypes);
        Assert.NotEmpty(repositoryInterfaces);
        Assert.NotEmpty(domainEntityNamespaces);

        var forbidden = new[] { InfrastructureNamespace }.Concat(repositoryInterfaces).Concat(domainEntityNamespaces).ToArray();

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
