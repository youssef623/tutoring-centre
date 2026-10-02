using System.Xml.Linq;

namespace TutoringCentre.Architecture.Tests;

/// <summary>Reads the declared ProjectReference items of the src projects.</summary>
internal static class ProjectFiles
{
    private const string SolutionFileName = "TutoringCentre.slnx";

    /// <summary>Returns the referenced project names (without extension), sorted ordinally.</summary>
    public static string[] ReadProjectReferences(string projectName)
    {
        var projectPath = Path.Combine(FindRepositoryRoot(), "src", projectName, projectName + ".csproj");
        var project = XDocument.Load(projectPath);

        return project
            .Descendants("ProjectReference")
            .Select(element => (string?)element.Attribute("Include"))
            .OfType<string>()
            // `dotnet add reference` on Windows writes backslashes; Linux CI treats them as ordinary characters.
            .Select(include => Path.GetFileNameWithoutExtension(include.Replace('\\', Path.DirectorySeparatorChar)))
            .Order(StringComparer.Ordinal)
            .ToArray();
    }

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
