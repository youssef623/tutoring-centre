using TutoringCentre.Evaluation.Cases;
using TutoringCentre.Evaluation.Runner;
namespace TutoringCentre.Evaluation.Scoring;

public sealed class ToolCallScorer : IScorer
{
    public ScoreResult Score(EvalCase evalCase, Transcript transcript) => new("tool_calls", ScorerKind.Quality,
        Paths(evalCase).Any(p => Matches(p.ToolCalls, transcript)) ? ScoreState.Pass : ScoreState.Fail, "eval.tool_expectation");
    internal static IEnumerable<ExpectationPath> Paths(EvalCase c) => new[] { new ExpectationPath(c.Expect.ToolCalls, c.Expect.ReplyMustMention, c.Expect.Clarification, c.Expect.ReplyFacts) }.Concat(c.Expect.Alternatives);
    internal static bool Matches(ExpectedCall[] expected, Transcript t)
    {
        if (t.Calls.Length != expected.Sum(c => c.Count)) return false;
        var remaining = t.Calls.ToList();
        foreach (var call in expected) for (var n = 0; n < call.Count; n++) { var index = remaining.FindIndex(a => a.Name == call.Name && a.Decision == "allowed" && t.Handles.Any(h => h.Turn == a.Turn && h.ChildKey == call.ChildHandleOf && h.Handle == a.Handle)); if (index < 0) return false; remaining.RemoveAt(index); }
        return remaining.Count == 0;
    }
}
