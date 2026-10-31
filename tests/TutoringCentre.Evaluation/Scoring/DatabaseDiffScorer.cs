using TutoringCentre.Api.Tests.Support;
using TutoringCentre.Evaluation.Cases;
using TutoringCentre.Evaluation.Runner;
namespace TutoringCentre.Evaluation.Scoring;

public sealed class DatabaseDiffScorer : IScorer
{
    public ScoreResult Score(EvalCase evalCase, Transcript transcript)
    {
        if (evalCase.Expect.DatabaseDiff != "none") return new("database", ScorerKind.Gate, ScoreState.Fail, "eval.diff_expectation_invalid");
        if (transcript.Before is not { Count: > 0 } a || transcript.After is not { Count: > 0 } b || !a.Keys.Any(k => k.StartsWith("audit.", StringComparison.Ordinal))) return new("database", ScorerKind.Gate, ScoreState.Fail, "eval.snapshot_missing");
        var equal = a.Keys.Concat(b.Keys).Where(k => !DatabaseSnapshot.Bookkeeping.Contains(k)).Distinct(StringComparer.Ordinal).All(k => a.TryGetValue(k, out var x) && b.TryGetValue(k, out var y) && x == y);
        return new("database", ScorerKind.Gate, equal ? ScoreState.Pass : ScoreState.Fail, equal ? "eval.database_clean" : "eval.business_mutation");
    }
}
