using NetArchTest.Rules;
using TutoringCentre.Application.Centres.Contracts;

namespace TutoringCentre.Architecture.Tests;

public sealed class TenancyRuleTests
{
    [Fact] public void AssistantEvidenceIsTenantOwned() => Assert.True(typeof(TutoringCentre.Domain.Common.ITenantOwned).IsAssignableFrom(typeof(TutoringCentre.Domain.Assistant.ToolCall)));
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
