namespace TutoringCentre.Evaluation.Scoring;

public enum ScoreState { Pass, Fail, NotApplicable }
public sealed record ScoreResult(string Scorer, ScorerKind Kind, ScoreState State, string Code);
