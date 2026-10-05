using NetArchTest.Rules;

namespace TutoringCentre.Architecture.Tests;

/// <summary>
/// Identity (UserManager, ApplicationUser, password hashing) is an Infrastructure concern. Application and the
/// Api — other than the composition root (Program.cs) and the CLI (which wires up Identity at startup and for
/// the development seeder) — see it only through Application's own ports (IAuthenticationService,
/// IMembershipReadService), never directly.
/// </summary>
public sealed class IdentityRuleTests
{
    private const string InfrastructureIdentityNamespace = "TutoringCentre.Infrastructure.Identity";
    private const string AspNetIdentityNamespace = "Microsoft.AspNetCore.Identity";
    private const string CliNamespace = "TutoringCentre.Api.Cli";

    [Fact]
    public void Application_DoesNotDependOnIdentity()
    {
        var types = Types.InAssembly(SourceAssemblies.Application);
        Assert.NotEmpty(types.GetTypes());

        var result = types
            .ShouldNot().HaveDependencyOnAny(InfrastructureIdentityNamespace, AspNetIdentityNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, ArchitectureSupport.Describe(result));
    }

    [Fact]
    public void Api_OutsideCompositionRootAndCli_DoesNotDependOnIdentity()
    {
        var result = Types.InAssembly(SourceAssemblies.Api)
            .That().DoNotHaveName("Program")
            .And().DoNotResideInNamespace(CliNamespace)
            .ShouldNot().HaveDependencyOnAny(InfrastructureIdentityNamespace, AspNetIdentityNamespace)
            .GetResult();

        Assert.True(result.IsSuccessful, ArchitectureSupport.Describe(result));
    }
}
