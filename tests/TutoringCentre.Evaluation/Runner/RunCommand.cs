using System.Text.Json;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using OpenAI;
using TutoringCentre.Api.Tests.Support;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Context;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Evaluation.Cases;
using TutoringCentre.Evaluation.Host;
using TutoringCentre.Evaluation.Models;
using TutoringCentre.Evaluation.Reporting;
using TutoringCentre.Evaluation.Scoring;
using TutoringCentre.Evaluation.World;
using TutoringCentre.Infrastructure.Assistant;
namespace TutoringCentre.Evaluation.Runner;

public static class RunCommand
{
    public static void CheckMode(string mode, string? ci, string? key)
    {
        if (mode is not ("record" or "replay" or "live")) throw new InvalidOperationException("eval.mode_invalid");
        if (mode != "replay" && string.Equals(ci, "true", StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("eval.live_ci_refused");
        if (mode != "replay" && string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("eval.key_missing");
    }
    public static async Task<int> ExecuteAsync(string[] args)
    {
        try { return await RunAsync(args); }
        catch (Exception ex)
        {
            var safeCode = System.Text.RegularExpressions.Regex.IsMatch(ex.Message, "^[a-z][a-z0-9_.-]{1,59}$", System.Text.RegularExpressions.RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1))
                ? ex.Message : "eval.execution_failed";
            Console.Error.WriteLine(safeCode);
            return 2;
        }
    }
    private static async Task<int> RunAsync(string[] args)
    {
        var root = AppContext.BaseDirectory;
        var all = CaseLoader.Load(Path.Combine(root, "cases"));
        if (args.Length == 1 && args[0] == "selftest")
        {
            Console.WriteLine($"Cases: {all.Count}");
            foreach (var g in all.GroupBy(c => c.Tags.Category)) Console.WriteLine($"{g.Key}: {g.Count()}");
            foreach (var g in all.GroupBy(c => c.Tags.LanguageStyle)) Console.WriteLine($"{g.Key}: {g.Count()}");
            var recording = new RecordingLanguageModel(new ScriptedProbeModel());
            var observed = new ObservedLanguageModel(recording);
            await using var host = new EvalHost(observed); await host.InitializeAsync(); await host.ResetAsync();
            var world = new EvalWorld(host); await world.SeedAsync(); var before = await DatabaseSnapshot.CaptureAsync(host.SuperuserConnectionString); await world.SeedAsync(); var after = await DatabaseSnapshot.CaptureAsync(host.SuperuserConnectionString);
            if (!before.SequenceEqual(after)) throw new InvalidOperationException("eval.selftest_failed");
            Console.WriteLine($"World: {world.Personas.Count} parents; {world.Personas.Values.Sum(p => p.Children.Count)} children; seed twice unchanged");
            try { host.RequireOwnedContainer("Host=production;Database=business"); throw new InvalidOperationException("eval.container_check_broken"); }
            catch (InvalidOperationException ex) when (ex.Message == "eval.container_required") { }
            var first = await new CaseRunner(host, observed).RunAsync(all.Single(c => c.Id == "E1"));
            IScorer[] scorers = [new ToolCallScorer(), new ReplyContentScorer(), new CanaryScorer(), new DatabaseDiffScorer()];
            if (scorers.Any(s => s.Score(all[0], first).State != ScoreState.Pass)) throw new InvalidOperationException("eval.probe_failed");
            // Synthetic in-memory contract proof only: never a committed real-model cassette.
            var replay = new ReplayLanguageModel(new("E1", AssistantPrompt.Version, "synthetic-selftest", recording.Entries.ToArray()));
            var replayObserved = new ObservedLanguageModel(replay);
            await using var fresh = new EvalHost(replayObserved); await fresh.InitializeAsync();
            var second = await new CaseRunner(fresh, replayObserved).RunAsync(all[0]);
            var firstDecisions = first.Calls.Select(c => (c.Turn, c.Name, c.Decision, c.Code));
            var secondDecisions = second.Calls.Select(c => (c.Turn, c.Name, c.Decision, c.Code));
            if (replay.Errors.Count > 0 || !first.Replies.SequenceEqual(second.Replies) || !firstDecisions.SequenceEqual(secondDecisions)
                || scorers.Any(s => s.Score(all[0], second).State != ScoreState.Pass)) throw new InvalidOperationException("eval.fresh_replay_failed");
            if (Environment.GetEnvironmentVariable("EVAL_SELFTEST_EXPORT") is { Length: > 0 } export)
                await new Cassette("E1", AssistantPrompt.Version, "synthetic-selftest-not-real-recording", recording.Entries.ToArray()).WriteAsync(export, []);
            Console.WriteLine("E1 scripted pipeline and fresh-container exact-request replay pass; non-container reset refused. No real recording created.");
            return 0;
        }
        if (args.Length == 0 || args[0] is not ("run" or "probe")) throw new InvalidOperationException("eval.command_invalid");
        var options = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var i = 1; i < args.Length; i += 2) { if (i + 1 >= args.Length || args[i] is not ("--mode" or "--cases" or "--out") || !options.TryAdd(args[i], args[i + 1])) throw new InvalidOperationException("eval.options_invalid"); }
        var probe = args[0] == "probe"; var mode = options.GetValueOrDefault("--mode", "replay");
        var key = Environment.GetEnvironmentVariable("EVAL_API_KEY") ?? Environment.GetEnvironmentVariable("OPENAI_API_KEY") ?? Environment.GetEnvironmentVariable("Assistant__ApiKey");
        var endpointText = mode == "replay" ? null : Environment.GetEnvironmentVariable("EVAL_MODEL_ENDPOINT");
        var endpoint = endpointText is null ? null : new Uri(endpointText);
        if (endpoint is not null && (endpoint.Scheme is not ("http" or "https") || (!endpoint.IsLoopback && endpoint.Scheme != "https"))) throw new InvalidOperationException("eval.endpoint_requires_https");
        if (!probe) CheckMode(mode, Environment.GetEnvironmentVariable("CI"), endpoint?.IsLoopback == true ? "local-no-key" : key);
        var selected = options.TryGetValue("--cases", out var selection) ? selection.Split(',').ToHashSet(StringComparer.Ordinal) : all.Select(c => c.Id).ToHashSet(StringComparer.Ordinal);
        if (selected.Count == 0 || selected.Any(id => !all.Any(c => c.Id == id))) throw new InvalidOperationException("eval.case_unknown");
        var modelName = probe ? "scripted-probe" : mode == "replay" ? "cassette" : Environment.GetEnvironmentVariable("EVAL_MODEL") ?? "gpt-4.1-mini";
        var recordedModels = new HashSet<string>(StringComparer.Ordinal);
        var results = new List<CaseResult>();
        var fingerprints = new Dictionary<string, string>(StringComparer.Ordinal);
        foreach (var c in all.Where(c => selected.Contains(c.Id)))
        {
            try
            {
                var cassettePath = Path.Combine("tests", "TutoringCentre.Evaluation", "cassettes", c.Id + ".json");
                ILanguageModel model = probe ? new ScriptedProbeModel(mode == "leak")
                    : mode == "replay" ? CreateReplay(cassettePath, c.Id, recordedModels)
                    : CreateLive(key, endpoint, modelName);
                RecordingLanguageModel? recording = null;
                if (!probe && mode == "record") { recording = new(model); model = recording; }
                var observed = new ObservedLanguageModel(model);
                await using var host = new EvalHost(observed); await host.InitializeAsync();
                var transcript = await new CaseRunner(host, observed).RunAsync(c);
                fingerprints.Add(c.Id, transcript.ContentFingerprint!);
                IScorer[] scorers = [new ToolCallScorer(), new ReplyContentScorer(), new CanaryScorer(), new DatabaseDiffScorer()];
                var scores = scorers.Select(scorer => scorer.Score(c, transcript)).ToList();
                if (transcript.Errors.Length > 0) scores.Add(new("execution", ScorerKind.Quality, ScoreState.Fail, transcript.Errors.Contains("eval.cassette_missing", StringComparer.Ordinal) ? "eval.cassette_missing" : "eval.pipeline_failed"));

                if (recording is { Entries.Count: > 0 } && transcript.Errors.Length == 0)
                    await new Cassette(c.Id, AssistantPrompt.Version, modelName, recording.Entries.ToArray()).WriteAsync(cassettePath, [key!, .. Environment.GetEnvironmentVariable("EVAL_SECRET_PATTERNS")?.Split(';') ?? []]);
                results.Add(new(c.Id, c.Tags, scores.ToArray()));
                Console.WriteLine($"{c.Id}: {(results[^1].Passed ? "pass" : "fail")} {string.Join(",", scores.Where(s => s.State == ScoreState.Fail).Select(s => s.Code))}");
                if (probe) { foreach (var reply in transcript.Replies) Console.WriteLine(reply); foreach (var call in transcript.Calls) Console.WriteLine($"{call.Name}: {call.Decision} {call.Code}"); }
            }
            catch (Exception ex)
            {
                if (probe) Console.Error.WriteLine(ex.GetType().Name + ": " + ex.StackTrace);
                var code = ex is FileNotFoundException ? "eval.cassette_missing" : "eval.case_execution_failed";
                Console.Error.WriteLine(c.Id + ": " + code);
                results.Add(new(c.Id, c.Tags, [new("execution", ScorerKind.Quality, ScoreState.Fail, code), new("canary", ScorerKind.Gate, ScoreState.Fail, "eval.output_evidence_missing"), new("database", ScorerKind.Gate, ScoreState.Fail, "eval.snapshot_missing")]));
            }
        }
        var reportModel = mode == "replay" && !probe
            ? recordedModels.Count == 0 ? "unavailable" : string.Join(",", recordedModels.Order(StringComparer.Ordinal))
            : modelName;
        var report = ReportBuilder.Build(probe ? "probe" : mode, AssistantPrompt.Version, reportModel, results.ToArray(), all.Count);
        var output = options.GetValueOrDefault("--out", "eval-report"); Directory.CreateDirectory(output);
        if (Environment.GetEnvironmentVariable("EVAL_PARITY_PATH") is { Length: > 0 } parityPath)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(parityPath))!);
            await File.WriteAllTextAsync(parityPath, JsonSerializer.Serialize(fingerprints, CaseLoader.Json));
        }
        await ReportJson.WriteAsync(report, Path.Combine(output, "report.json")); await File.WriteAllTextAsync(Path.Combine(output, "report.md"), ReportMarkdown.Render(report));
        var thresholds = JsonSerializer.Deserialize<Thresholds>(File.ReadAllText(Path.Combine(root, "thresholds.json")), CaseLoader.Json)!;
        return thresholds.Passes(report) ? 0 : 1;
    }

    private static ReplayLanguageModel CreateReplay(string path, string caseId, HashSet<string> models)
    {
        var cassette = Cassette.Read(path);
        if (cassette.CaseId != caseId) throw new InvalidDataException("eval.cassette_case_mismatch");
        models.Add(cassette.Model);
        return new(cassette);
    }

    private static ProviderLanguageModel CreateLive(string? key, Uri? endpoint, string modelName)
    {
        var credential = new System.ClientModel.ApiKeyCredential(endpoint?.IsLoopback == true ? "local-no-key" : key!);
        var client = new OpenAIClient(credential, new OpenAIClientOptions { Endpoint = endpoint ?? new Uri("https://api.openai.com/v1") });
        return new(client.GetChatClient(modelName).AsIChatClient(), Options.Create(new AssistantOptions()), NullLogger<ProviderLanguageModel>.Instance, modelName);
    }
}
