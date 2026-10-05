namespace TutoringCentre.Architecture.Tests;

/// <summary>Reads production .cs source files directly, for rules NetArchTest cannot express (it inspects
/// compiled type dependencies, not method calls or text). Mirrors ProjectFiles' repository-root lookup.</summary>
internal static class SourceFiles
{
    private const string SolutionFileName = "TutoringCentre.slnx";

    /// <summary>Every .cs file under src/, excluding build output — never tests/.</summary>
    public static IEnumerable<string> EnumerateProductionSourceFiles()
    {
        var srcRoot = Path.Combine(FindRepositoryRoot(), "src");
        return Directory.EnumerateFiles(srcRoot, "*.cs", SearchOption.AllDirectories)
            .Where(path => !path.Split(Path.DirectorySeparatorChar).Any(segment => segment is "bin" or "obj"));
    }

    /// <summary>A path relative to the repository root, with forward slashes regardless of platform.</summary>
    public static string ToRepositoryRelativePath(string absolutePath) =>
        Path.GetRelativePath(FindRepositoryRoot(), absolutePath).Replace(Path.DirectorySeparatorChar, '/');

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, SolutionFileName)))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new InvalidOperationException($"Could not find {SolutionFileName} above {AppContext.BaseDirectory}.");
    }
}
