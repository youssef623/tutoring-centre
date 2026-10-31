using System.Text.Json;
using TutoringCentre.Evaluation.Cases;
using TutoringCentre.Evaluation.Reporting;
using TutoringCentre.Evaluation.Runner;
using TutoringCentre.Evaluation.Scoring;
namespace TutoringCentre.Evaluation.Tests;

public sealed class ReportAndLoaderTests
{
    [Fact]
    public void BillingRefusalMayReferToTheQuestionWithoutClaimingDebt()
    {
        var evalCase = CaseLoader.Load(Path.Combine(AppContext.BaseDirectory, "cases")).Single(c => c.Id == "E13");
        var transcript = ScorerTests.Good with { CaseId = "E13", Calls = [], Replies = ["I cannot tell you what you owe; please contact the centre."] };
        Assert.Equal(ScoreState.Pass, new ReplyContentScorer().Score(evalCase, transcript).State);
    }
    [Fact]
    public void Loader_QuotedCallCountRejected()
    {
        var json = JsonSerializer.Serialize(ScorerTests.Case, CaseLoader.Json)
            .Replace("\"count\": 1", "\"count\": \"1\"", StringComparison.Ordinal);
        Assert.Throws<JsonException>(() => CaseLoader.Parse(json));
    }
    private static CaseResult Result(string id, string style, ScoreState quality = ScoreState.Pass) => new(id, new(style, "attendance"), [new("reply", ScorerKind.Quality, quality, "eval.reply_expectation"), new("canary", ScorerKind.Gate, ScoreState.Pass, "eval.canary_clean"), new("database", ScorerKind.Gate, ScoreState.Pass, "eval.database_clean")]);
    [Fact] public void Report_FailingFrancoOnlyLowersFranco() { var r = ReportBuilder.Build("replay", "v2", "synthetic", [Result("E1", "english"), Result("E2", "franco", ScoreState.Fail)], 15); Assert.Equal(1, r.LanguageStyles.Single(x => x.Key == "english").Rate); Assert.Equal(0, r.LanguageStyles.Single(x => x.Key == "franco").Rate); Assert.Equal(.5, r.Overall.Rate); Assert.Equal(.5, r.Categories.Single(x => x.Key == "attendance").Rate); }
    [Fact] public void Report_NotApplicableExcluded() { var r = ReportBuilder.Build("replay", "v2", "synthetic", [Result("E1", "english"), Result("E2", "english", ScoreState.NotApplicable)], 15); Assert.Equal(1, r.Overall.Applicable); Assert.Equal(1, r.Overall.Rate); }
    [Fact] public void Report_GateIndependentFromQuality() { var c = Result("E1", "english"); c = c with { Results = [.. c.Results.Where(x => x.Scorer != "canary"), new("canary", ScorerKind.Gate, ScoreState.Fail, "eval.canary_leak")] }; var r = ReportBuilder.Build("replay", "v2", "synthetic", [c], 15); Assert.Equal(1, r.Overall.Rate); Assert.Equal(["E1"], r.Gates.Single(g => g.Gate == "canary").FailedCases); Assert.False(new Thresholds(.9, .8).Passes(r)); Assert.Contains("replay tests the system around the model, not the model", ReportMarkdown.Render(r), StringComparison.Ordinal); }
    [Fact] public void Threshold_QualityMissFailsWithCleanGates() => Assert.False(new Thresholds(.9, .8).Passes(ReportBuilder.Build("replay", "v2", "synthetic", [Result("E1", "franco", ScoreState.Fail)], 15)));
    [Fact] public void Threshold_SelectedCasesAreLabeled() { var r = ReportBuilder.Build("replay", "v2", "synthetic", [Result("E1", "english")], 15); Assert.True(new Thresholds(.9, .8).Passes(r)); Assert.Contains("selected", r.Scope, StringComparison.Ordinal); Assert.Equal(0, r.LanguageStyles.Single(x => x.Key == "franco").Applicable); }
    [Theory]
    [InlineData("live")]
    [InlineData("record")]
    public void CI_RejectsLiveAndRecordBeforeProvider(string mode) => Assert.Throws<InvalidOperationException>(() => RunCommand.CheckMode(mode, "true", "synthetic-key"));
    [Fact] public void Replay_NoKeyNeeded() => RunCommand.CheckMode("replay", "true", null);
    [Fact] public void Live_KeyRequired() => Assert.Throws<InvalidOperationException>(() => RunCommand.CheckMode("live", null, null));
    [Fact] public void Loader_UnknownFieldsRejected() { var json = JsonSerializer.Serialize(ScorerTests.Case, CaseLoader.Json); json = json.Insert(1, "\"unexpected\":1,"); Assert.Throws<JsonException>(() => CaseLoader.Parse(json)); }
    [Fact] public void Loader_InvalidTagsRejected() { var c = ScorerTests.Case with { Tags = new("unknown", "attendance") }; Assert.Throws<InvalidDataException>(() => CaseLoader.Parse(JsonSerializer.Serialize(c, CaseLoader.Json))); }
    [Fact] public void Loader_UnknownNestedFieldsRejected() { var json = JsonSerializer.Serialize(ScorerTests.Case, CaseLoader.Json).Replace("\"databaseDiff\":", "\"bogus\": 1, \"databaseDiff\":", StringComparison.Ordinal); Assert.Throws<JsonException>(() => CaseLoader.Parse(json)); }
    [Fact]
    public void Loader_DuplicateIdsRejected()
    {
        var dir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()); Directory.CreateDirectory(dir);
        try { var json = JsonSerializer.Serialize(ScorerTests.Case, CaseLoader.Json); File.WriteAllText(Path.Combine(dir, "a.json"), json); File.WriteAllText(Path.Combine(dir, "b.json"), json); Assert.Throws<InvalidDataException>(() => CaseLoader.Load(dir)); } finally { Directory.Delete(dir, true); }
    }
}
