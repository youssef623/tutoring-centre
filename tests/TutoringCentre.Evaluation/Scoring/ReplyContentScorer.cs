using TutoringCentre.Domain.Common;
using TutoringCentre.Evaluation.Cases;
using TutoringCentre.Evaluation.Runner;
using System.Text.RegularExpressions;
namespace TutoringCentre.Evaluation.Scoring;

public sealed class ReplyContentScorer : IScorer
{
    public static bool Contains(string text, string value) => SearchText.Normalize(text).Contains(SearchText.Normalize(value), StringComparison.OrdinalIgnoreCase);
    private static bool Mention(string text, string value) => Regex.IsMatch(SearchText.Normalize(text),
        @"(?<![\p{L}\p{N}_])" + Regex.Escape(SearchText.Normalize(value)) + @"(?![\p{L}\p{N}_])",
        RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1));
    internal static bool MatchesPath(ExpectationPath p, Transcript t)
    {
        if (t.Replies.Length == 0) return false;
        var text = string.Join("\n", t.Replies);
        if (!p.ReplyMustMention.All(g => g.AnyOf.Any(s => Mention(text, s)))) return false;
        var segments = Regex.Split(text, @"[.!?؟;,،؛\n]|\b(?:and|while|but)\b", RegexOptions.CultureInvariant | RegexOptions.IgnoreCase, TimeSpan.FromSeconds(1));
        if (!p.ReplyFacts.All(f => MatchesFact(f, p.ReplyFacts, segments))) return false;
        return p.Clarification is not { } c || ((text.Contains('?') || text.Contains('؟')) && c.MustMention.All(g => g.AnyOf.Any(s => Mention(text, s))) && c.MustNotMention.All(s => !Mention(text, s)));
    }

    private static bool MatchesFact(ReplyFact fact, ReplyFact[] facts, string[] segments)
    {
        var relevant = segments.Where(segment => fact.Subject.AnyOf.Any(subject => Mention(segment, subject))).ToArray();
        if (relevant.Any(segment => fact.MustNotMention.Any(forbidden => Mention(segment, forbidden)))) return false;
        return relevant.Any(segment => fact.Status.AnyOf.Any(status => Mention(segment, status))
            && !facts.Where(other => !ReferenceEquals(other, fact)).Any(other => other.Subject.AnyOf.Any(subject => Mention(segment, subject))));
    }
    public ScoreResult Score(EvalCase evalCase, Transcript transcript)
    {
        var paths = ToolCallScorer.Paths(evalCase).ToArray();
        if (evalCase.Expect.ReplyMustNotMention.Length == 0
            && paths.All(p => p.ReplyMustMention.Length == 0 && p.ReplyFacts.Length == 0 && p.Clarification is null))
            return new("reply", ScorerKind.Quality, ScoreState.NotApplicable, "eval.reply_not_applicable");
        var pass = transcript.Replies.Length == evalCase.Turns.Length
            && evalCase.Expect.ReplyMustNotMention.All(s => transcript.Replies.All(r => !Mention(r, s)))
            && paths.Any(p => p.ToolCalls.Sum(c => c.Count) == transcript.Calls.Length && MatchesPath(p, transcript));
        return new("reply", ScorerKind.Quality, pass ? ScoreState.Pass : ScoreState.Fail, "eval.reply_expectation");
    }
}
