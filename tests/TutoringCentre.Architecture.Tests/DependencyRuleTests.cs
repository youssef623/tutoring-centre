using System.Reflection;
using System.Xml.Linq;
using NetArchTest.Rules;

namespace TutoringCentre.Architecture.Tests;

/// <summary>Type-level dependency rule: fails when compiled code in a layer USES a forbidden namespace.</summary>
public sealed class DependencyRuleTests
{
    private static readonly Assembly DomainAssembly = typeof(TutoringCentre.Domain.AssemblyMarker).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(TutoringCentre.Application.AssemblyMarker).Assembly;
    private static readonly Assembly InfrastructureAssembly = typeof(TutoringCentre.Infrastructure.AssemblyMarker).Assembly;
    private static readonly string[] HangfireAllowedProjects = ["TutoringCentre.Infrastructure", "TutoringCentre.Api"];

    private static readonly string[] ForbiddenForDomain =
    [
        "TutoringCentre.Application",
        "TutoringCentre.Infrastructure",
        "TutoringCentre.Api",
        "Microsoft.EntityFrameworkCore",
        "Microsoft.AspNetCore",
        "Npgsql",
        "Hangfire",
    ];

    private static readonly string[] ForbiddenForApplication =
    [
        "TutoringCentre.Infrastructure",
        "TutoringCentre.Api",
        "Microsoft.EntityFrameworkCore",
        "Microsoft.AspNetCore",
        "Npgsql",
    ];

    private static readonly string[] ForbiddenForInfrastructure =
    [
        "TutoringCentre.Api",
    ];

    [Fact]
    public void Domain_DoesNotDependOnOtherLayersOrFrameworks() =>
        AssertNoDependencies(DomainAssembly, ForbiddenForDomain);

    [Fact]
    public void Application_DoesNotDependOnInfrastructureApiOrFrameworks() =>
        AssertNoDependencies(ApplicationAssembly, ForbiddenForApplication);

    [Fact]
    public void Infrastructure_DoesNotDependOnApi() =>
        AssertNoDependencies(InfrastructureAssembly, ForbiddenForInfrastructure);

    [Fact]
    public void Hangfire_PackagesAreDeclaredOnlyAtInfrastructureAndApiEdges()
    {
        var root = ProjectFiles.FindRepositoryRoot();
        foreach (var project in Directory.EnumerateFiles(Path.Combine(root, "src"), "*.csproj", SearchOption.AllDirectories))
        {
            var packages = XDocument.Load(project).Descendants("PackageReference")
                .Select(element => (string?)element.Attribute("Include"))
                .Where(name => name?.StartsWith("Hangfire", StringComparison.Ordinal) == true).ToList();
            if (packages.Count > 0)
                Assert.Contains(Path.GetFileNameWithoutExtension(project), HangfireAllowedProjects);
        }
    }

    [Fact]
    public void Application_DoesNotDependOnSerilogHangfireOrHttpClient()
    {
        var result = Types.InAssembly(SourceAssemblies.Application)
            .ShouldNot()
            .HaveDependencyOnAny("Serilog", "Hangfire", "System.Net.Http")
            .GetResult();

        Assert.True(result.IsSuccessful, ArchitectureSupport.Describe(result));
    }

    private static void AssertNoDependencies(Assembly assembly, string[] forbidden)
    {
        var types = Types.InAssembly(assembly);

        // Guards against scanning the wrong (or an empty) assembly, which would always pass.
        Assert.NotEmpty(types.GetTypes());

        var result = types.ShouldNot().HaveDependencyOnAny(forbidden).GetResult();

        Assert.True(
            result.IsSuccessful,
            $"{assembly.GetName().Name} has forbidden dependencies in: {string.Join(", ", result.FailingTypeNames ?? [])}");
    }
}
