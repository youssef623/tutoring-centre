# `tests/TutoringCentre.Architecture.Tests/SourceFiles.cs`

## Purpose

Source summary (intent, not independent execution proof): Reads production .cs source files directly, for rules NetArchTest cannot express (it inspects compiled type dependencies, not method calls or text). Mirrors ProjectFiles' repository-root lookup.

## Where it fits

Category: **test**. [Open current file](../../../../tests/TutoringCentre.Architecture.Tests/SourceFiles.cs). [Complete file index](../../INDEX.md). [Current teaching overview](../../../PROJECT_OVERVIEW3.md).

The reviewed file has 32 lines and 1,567 bytes. Source hash: `91f9775c056c1941740bfa32cf271fb9b353bab285b3e21e093dc21502afb7a5`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../tests/TutoringCentre.Architecture.Tests/SourceFiles.cs#L5): `internal static class SourceFiles`
- [Line 10](../../../../tests/TutoringCentre.Architecture.Tests/SourceFiles.cs#L10): `public static IEnumerable<string> EnumerateProductionSourceFiles()`
- [Line 18](../../../../tests/TutoringCentre.Architecture.Tests/SourceFiles.cs#L18): `public static string ToRepositoryRelativePath(string absolutePath) =>`
- [Line 21](../../../../tests/TutoringCentre.Architecture.Tests/SourceFiles.cs#L21): `private static string FindRepositoryRoot()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs:63](../../../../tests/TutoringCentre.Architecture.Tests/AuthorizationRuleTests.cs#L63)
- [tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs:51](../../../../tests/TutoringCentre.Architecture.Tests/QueryFilterBypassTests.cs#L51)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
