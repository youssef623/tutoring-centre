using TutoringCentre.Evaluation.Scoring;
namespace TutoringCentre.Evaluation.Reporting;

public sealed record RateRow(string Key, int Cases, int Applicable, int Passed) { public double? Rate => Applicable == 0 ? null : (double)Passed / Applicable; }
public sealed record GateRow(string Gate, int Applicable, string[] FailedCases);
public sealed record EvalReport(string Mode, string PromptVersion, string Model, DateTimeOffset Date, int CaseCount, int DatasetCaseCount, string Scope, RateRow Overall, RateRow[] LanguageStyles, RateRow[] Categories, GateRow[] Gates, CaseResult[] Cases);
public static class ReportBuilder
{
    private static readonly string[] Styles = ["egyptian", "msa", "franco", "english", "mixed"];
    private static readonly string[] Categories = ["attendance", "ambiguity", "attack", "out_of_scope"];
    private static readonly string[] Gates = ["canary", "database"];
    private static bool Applicable(CaseResult c) => c.Results.Any(r => r.Kind == ScorerKind.Quality && r.State != ScoreState.NotApplicable);
    private static bool QualityPassed(CaseResult c) => c.Results.Where(r => r.Kind == ScorerKind.Quality).All(r => r.State != ScoreState.Fail);
    private static RateRow Row(string key, IEnumerable<CaseResult> source) { var a = source.ToArray(); return new(key, a.Length, a.Count(Applicable), a.Count(c => Applicable(c) && QualityPassed(c))); }
    public static EvalReport Build(string mode, string version, string model, CaseResult[] cases, int datasetCount) => new(mode, version, model, DateTimeOffset.UtcNow, cases.Length, datasetCount, cases.Length == datasetCount ? "full dataset" : "selected cases; does not establish full-dataset coverage", Row("overall", cases),
     Styles.Select(style => Row(style, cases.Where(c => c.Tags.LanguageStyle == style))).ToArray(),
     Categories.Select(category => Row(category, cases.Where(c => c.Tags.Category == category))).ToArray(),
     Gates.Select(g => new GateRow(g, cases.Count(c => c.Results.Any(r => r.Scorer == g && r.State != ScoreState.NotApplicable)), cases.Where(c => c.Results.Any(r => r.Scorer == g && r.State == ScoreState.Fail)).Select(c => c.CaseId).ToArray())).ToArray(), cases);
}
