# `tests/TutoringCentre.Infrastructure.Tests/Fixtures/RoleBootstrap.cs`

## Purpose

Source summary (intent, not independent execution proof): Runs the real db/bootstrap-roles.sh (Day 19) inside a throwaway Testcontainers PostgreSQL instance, with test-only passwords, so the test fixture exercises the exact same role split as production instead of a stand-in for it.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/RoleBootstrap.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 47 lines and 1,790 bytes. Source hash: `072812a386a95ce0824f4f46321a1971edd9a644b41af31943ab3350c04f9a2f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Testcontainers.PostgreSql;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/RoleBootstrap.cs#L10): `internal static class RoleBootstrap`
- [Line 14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/RoleBootstrap.cs#L14): `public static async Task RunAsync(PostgreSqlContainer container, string ownerPassword, string appPassword)`
- [Line 34](../../../../../tests/TutoringCentre.Infrastructure.Tests/Fixtures/RoleBootstrap.cs#L34): `private static string FindBootstrapScript()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Testcontainers.PostgreSql;

namespace TutoringCentre.Infrastructure.Tests.Fixtures;

/// <summary>
/// Runs the real db/bootstrap-roles.sh (Day 19) inside a throwaway Testcontainers PostgreSQL instance, with
/// test-only passwords, so the test fixture exercises the exact same role split as production instead of a
/// stand-in for it.
/// </summary>
internal static class RoleBootstrap
{
    private const string ScriptPathInContainer = "/bootstrap-roles.sh";

    public static async Task RunAsync(PostgreSqlContainer container, string ownerPassword, string appPassword)
    {
        var scriptBytes = await File.ReadAllBytesAsync(FindBootstrapScript());
        await container.CopyAsync(scriptBytes, ScriptPathInContainer);
        await container.ExecAsync(["chmod", "+x", ScriptPathInContainer]);

        var result = await container.ExecAsync(
        [
            "bash",
            "-c",
            $"TUTORING_OWNER_PASSWORD='{ownerPassword}' TUTORING_APP_PASSWORD='{appPassword}' {ScriptPathInContainer}",
        ]);

        if (result.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"db/bootstrap-roles.sh failed inside the test container (exit {result.ExitCode}):\n{result.Stderr}");
        }
    }

    private static string FindBootstrapScript()
    {
        for (var directory = new DirectoryInfo(AppContext.BaseDirectory); directory is not null; directory = directory.Parent)
        {
            var candidate = Path.Combine(directory.FullName, "db", "bootstrap-roles.sh");
            if (File.Exists(candidate))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException("Could not find db/bootstrap-roles.sh above the test output directory.");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
