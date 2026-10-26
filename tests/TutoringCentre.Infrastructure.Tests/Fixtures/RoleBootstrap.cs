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
