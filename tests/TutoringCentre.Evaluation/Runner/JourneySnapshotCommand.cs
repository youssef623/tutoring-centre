using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.RegularExpressions;
using Npgsql;
using TutoringCentre.Api.Tests.Support;
namespace TutoringCentre.Evaluation.Runner;

/// <summary>Read-only browser-journey bridge to the existing Day 84 snapshot, restricted to the disposable task stack.</summary>
public static class JourneySnapshotCommand
{
    public static async Task<int> ExecuteAsync()
    {
        var container = Environment.GetEnvironmentVariable("E2E_POSTGRES_CONTAINER");
        var githubService = Environment.GetEnvironmentVariable("GITHUB_ACTIONS") == "true" && Environment.GetEnvironmentVariable("CI") == "true"
            && container is not null && Regex.IsMatch(container, "^[a-f0-9]{64}$", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
        if (container is null || (!githubService && !Regex.IsMatch(container, "^tutoring-d88-[a-z0-9-]+$", RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1)))) return 2;
        var start = new ProcessStartInfo("docker") { RedirectStandardOutput = true, RedirectStandardError = true, UseShellExecute = false };
        start.ArgumentList.Add("inspect"); start.ArgumentList.Add(container);
        using var process = Process.Start(start) ?? throw new InvalidOperationException("eval.container_required");
        var output = process.StandardOutput.ReadToEndAsync(); var error = process.StandardError.ReadToEndAsync();
        using var budget = new CancellationTokenSource(TimeSpan.FromSeconds(30)); await process.WaitForExitAsync(budget.Token);
        await error; if (process.ExitCode != 0) return 2;
        using var json = JsonDocument.Parse(await output); var item = json.RootElement[0];
        if (githubService)
        {
            if (item.GetProperty("Config").GetProperty("Image").GetString() != "postgres:17") return 2;
        }
        else
        {
            var project = item.GetProperty("Config").GetProperty("Labels").GetProperty("com.docker.compose.project").GetString();
            if (project is null || !project.StartsWith("tutoring-d88-", StringComparison.Ordinal)) return 2;
        }
        var port = item.GetProperty("NetworkSettings").GetProperty("Ports").GetProperty("5432/tcp")[0];
        if (!githubService && port.GetProperty("HostIp").GetString() != "127.0.0.1") return 2;
        var environment = item.GetProperty("Config").GetProperty("Env").EnumerateArray().Select(x => x.GetString()!).Select(x => x.Split('=', 2)).Where(x => x.Length == 2).ToDictionary(x => x[0], x => x[1], StringComparer.Ordinal);
        var connection = new NpgsqlConnectionStringBuilder { Host = "127.0.0.1", Port = int.Parse(port.GetProperty("HostPort").GetString()!, System.Globalization.CultureInfo.InvariantCulture),
            Database = environment["POSTGRES_DB"], Username = environment["POSTGRES_USER"], Password = environment["POSTGRES_PASSWORD"] };
        var snapshot = await DatabaseSnapshot.CaptureAsync(connection.ConnectionString);
        Console.WriteLine(Convert.ToHexString(SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(snapshot))));
        return 0;
    }
}
