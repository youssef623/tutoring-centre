using TutoringCentre.Evaluation.Reporting;
using TutoringCentre.Evaluation.Scoring;
namespace TutoringCentre.Evaluation.Runner;

public sealed record Thresholds(double OverallQuality, double PerLanguageQuality)
{
    public bool Passes(EvalReport report) => OverallQuality is >= 0.9 and <= 1 && PerLanguageQuality is >= 0.8 and <= 1
     && report.CaseCount > 0 && report.Cases.All(c => !c.GateFailed && !c.Results.Any(r => r.Scorer == "execution" && r.State == ScoreState.Fail))
     && report.Gates.All(g => g.Applicable == report.CaseCount && g.FailedCases.Length == 0)
     && report.Overall.Rate >= OverallQuality && report.LanguageStyles.Where(r => r.Applicable > 0).All(r => r.Rate >= PerLanguageQuality);
}
