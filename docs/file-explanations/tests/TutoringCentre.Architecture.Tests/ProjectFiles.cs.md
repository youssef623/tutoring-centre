# `tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs`

## Purpose

Source summary (intent, not independent execution proof): Reads the declared ProjectReference items of the src projects.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 37 lines and 1,529 bytes. Source hash: `e01c0d21983724656e0282f32045010d92ff715bb9870799fb850d83a058c84d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Xml.Linq;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs#L6): `internal static class ProjectFiles`
- [Line 11](../../../../tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs#L11): `public static string[] ReadProjectReferences(string projectName)`
- [Line 26](../../../../tests/TutoringCentre.Architecture.Tests/ProjectFiles.cs#L26): `private static string FindRepositoryRoot()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs:15](../../../../tests/TutoringCentre.Architecture.Tests/ProjectReferenceTests.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
