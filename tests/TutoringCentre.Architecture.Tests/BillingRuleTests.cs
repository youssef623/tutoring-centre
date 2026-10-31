using Mono.Cecil;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Billing;
using TutoringCentre.Domain.Identity;
namespace TutoringCentre.Architecture.Tests;

public sealed class BillingRuleTests
{
    private static readonly string[] FloatingTypes = ["System.Double", "System.Single"];
    [Fact]
    public void BillingHasNoFloatingPointFieldsParametersReturnsOrLocals()
    {
        foreach (var assembly in SourceAssemblies.All)
        {
            using var module = ModuleDefinition.ReadModule(assembly.Location);
            foreach (var type in module.Types.Where(t => t.Namespace.Contains(".Billing", StringComparison.Ordinal) || t.Name.Contains("Billing", StringComparison.Ordinal) || t.Name is nameof(Money) or nameof(BillingMonth)))
            {
                var references = type.Fields.Select(x => x.FieldType).Concat(type.Methods.Select(x => x.ReturnType))
                    .Concat(type.Methods.SelectMany(x => x.Parameters.Select(p => p.ParameterType)))
                    .Concat(type.Methods.Where(x => x.HasBody).SelectMany(x => x.Body.Variables.Select(v => v.VariableType)));
                Assert.All(references, reference => Assert.DoesNotContain(reference.FullName, FloatingTypes));
            }
        }
    }
    [Fact]
    public void NoNormalRoleCanGenerateCharges() => Assert.All(Enum.GetValues<StaffRole>(), role => Assert.False(RolePermissions.Holds(role, Permissions.BillingGenerate)));
    [Fact]
    public void ChargesHaveNoPublicMutationMethods() => Assert.All(typeof(Charge).GetProperties(), property => Assert.False(property.SetMethod?.IsPublic == true));
    [Fact]
    public void BillingUsesAcademicsPublishedContractsOnly()
    {
        var forbidden = SourceAssemblies.Application.GetTypes().Where(type => type.Namespace?.Contains(".Academics", StringComparison.Ordinal) == true
            && !type.Namespace.Contains(".Contracts", StringComparison.Ordinal)).Select(type => type.FullName!).ToArray();
        var result = NetArchTest.Rules.Types.InAssembly(SourceAssemblies.Application).That().ResideInNamespaceStartingWith("TutoringCentre.Application.Billing")
            .ShouldNot().HaveDependencyOnAny(forbidden).GetResult();
        Assert.True(result.IsSuccessful, ArchitectureSupport.Describe(result));
        var read = NetArchTest.Rules.Types.InAssembly(SourceAssemblies.Infrastructure).That().HaveName("BillingReadService")
            .ShouldNot().HaveDependencyOnAny("TutoringCentre.Domain.Academics").GetResult();
        Assert.True(read.IsSuccessful, ArchitectureSupport.Describe(read));
    }
}
