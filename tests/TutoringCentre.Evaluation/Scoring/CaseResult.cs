using TutoringCentre.Evaluation.Cases;
namespace TutoringCentre.Evaluation.Scoring;

public sealed record CaseResult(string CaseId, CaseTags Tags, ScoreResult[] Results)
{
    public bool GateFailed => Results.Any(r => r.Kind == ScorerKind.Gate && r.State == ScoreState.Fail);
    public bool Passed => Results.Any(r => r.Scorer == "canary" && r.Kind == ScorerKind.Gate && r.State == ScoreState.Pass) && Results.Any(r => r.Scorer == "database" && r.Kind == ScorerKind.Gate && r.State == ScoreState.Pass) && !GateFailed && Results.All(r => r.State != ScoreState.Fail);
}
