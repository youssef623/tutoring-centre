using TutoringCentre.Evaluation.Cases;
using TutoringCentre.Evaluation.Runner;
namespace TutoringCentre.Evaluation.Scoring;

public interface IScorer { ScoreResult Score(EvalCase evalCase, Transcript transcript); }
