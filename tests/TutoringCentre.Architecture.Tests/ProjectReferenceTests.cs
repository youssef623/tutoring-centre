namespace TutoringCentre.Architecture.Tests;

/// <summary>Declared-reference rule: fails when a .csproj DECLARES a reference outside the allowed set,
/// even if no code uses it (the compiler drops unused references, so DependencyRuleTests can't see them).</summary>
public sealed class ProjectReferenceTests
{
    // Typed from diagram B. Sorted ordinally.
    private static readonly string[] DomainAllowed = [];
    private static readonly string[] ApplicationAllowed = ["TutoringCentre.Domain"];
    private static readonly string[] InfrastructureAllowed = ["TutoringCentre.Application", "TutoringCentre.Domain"];
    private static readonly string[] ApiAllowed = ["TutoringCentre.Application", "TutoringCentre.Infrastructure"];

    [Fact]
    public void Domain_ReferencesNoProjects() =>
        Assert.Equal(DomainAllowed, ProjectFiles.ReadProjectReferences("TutoringCentre.Domain"));

    [Fact]
    public void Application_ReferencesOnlyDomain() =>
        Assert.Equal(ApplicationAllowed, ProjectFiles.ReadProjectReferences("TutoringCentre.Application"));

    [Fact]
    public void Infrastructure_ReferencesOnlyApplicationAndDomain() =>
        Assert.Equal(InfrastructureAllowed, ProjectFiles.ReadProjectReferences("TutoringCentre.Infrastructure"));

    [Fact]
    public void Api_ReferencesOnlyApplicationAndInfrastructure() =>
        Assert.Equal(ApiAllowed, ProjectFiles.ReadProjectReferences("TutoringCentre.Api"));
}
