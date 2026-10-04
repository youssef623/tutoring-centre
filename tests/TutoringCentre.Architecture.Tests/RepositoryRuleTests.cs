namespace TutoringCentre.Architecture.Tests;

public sealed class RepositoryRuleTests
{
    [Fact]
    public void RepositoryInterfaces_LiveInTheApplicationProject()
    {
        var interfaces = SourceAssemblies.All
            .SelectMany(assembly => assembly.GetTypes())
            .Where(ArchitectureSupport.IsRepositoryInterface)
            .ToList();

        Assert.NotEmpty(interfaces);
        var violations = interfaces
            .Where(type => type.Assembly != SourceAssemblies.Application
                || type.Namespace is null
                || !type.Namespace.StartsWith("TutoringCentre.Application", StringComparison.Ordinal))
            .Select(type => type.FullName)
            .ToList();
        Assert.Empty(violations);
    }

    [Fact]
    public void RepositoryClasses_LiveInTheInfrastructureProject()
    {
        var classes = SourceAssemblies.All
            .SelectMany(assembly => assembly.GetTypes())
            .Where(ArchitectureSupport.IsRepositoryClass)
            .ToList();

        Assert.NotEmpty(classes);
        var violations = classes
            .Where(type => type.Assembly != SourceAssemblies.Infrastructure
                || type.Namespace is null
                || !type.Namespace.StartsWith("TutoringCentre.Infrastructure", StringComparison.Ordinal))
            .Select(type => type.FullName)
            .ToList();
        Assert.Empty(violations);
    }
}
