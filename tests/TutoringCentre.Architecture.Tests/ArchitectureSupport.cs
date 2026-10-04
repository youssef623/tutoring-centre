using NetArchTest.Rules;

namespace TutoringCentre.Architecture.Tests;

internal static class ArchitectureSupport
{
    /// <summary>An interface named I*Repository (for example ICentreRepository).</summary>
    public static bool IsRepositoryInterface(Type type) =>
        type.IsInterface
        && type.Name.Length > "IRepository".Length
        && type.Name.StartsWith('I')
        && type.Name.EndsWith("Repository", StringComparison.Ordinal);

    /// <summary>A class named *Repository (for example CentreRepository).</summary>
    public static bool IsRepositoryClass(Type type) =>
        type.IsClass && type.Name.EndsWith("Repository", StringComparison.Ordinal);

    public static string Describe(TestResult result)
    {
        ArgumentNullException.ThrowIfNull(result);
        return result.IsSuccessful ? "ok" : "Violating types: " + string.Join(", ", result.FailingTypeNames ?? []);
    }
}
