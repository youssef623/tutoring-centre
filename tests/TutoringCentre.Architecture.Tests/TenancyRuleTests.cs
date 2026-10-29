using NetArchTest.Rules;
using TutoringCentre.Application.Centres.Contracts;

namespace TutoringCentre.Architecture.Tests;

public sealed class TenancyRuleTests
{
    [Fact]
    public void CentreDirectory_OnlyInfrastructureJobsMayDependOnIt()
    {
        var result = Types.InAssemblies(SourceAssemblies.All)
            .That().DoNotResideInNamespace("TutoringCentre.Infrastructure.Jobs")
            .ShouldNot().HaveDependencyOn(typeof(ICentreDirectory).FullName!)
            .GetResult();
        Assert.True(result.IsSuccessful, ArchitectureSupport.Describe(result));
    }
}
