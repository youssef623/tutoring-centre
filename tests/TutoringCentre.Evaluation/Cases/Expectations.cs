namespace TutoringCentre.Evaluation.Cases;

public sealed record MentionGroup(string[] AnyOf);
public sealed record ExpectedCall(string Name, string ChildHandleOf, int Count);
public sealed record Clarification(MentionGroup[] MustMention, string[] MustNotMention);
public sealed record ReplyFact(MentionGroup Subject, MentionGroup Status, string[] MustNotMention);
public sealed record ExpectationPath(ExpectedCall[] ToolCalls, MentionGroup[] ReplyMustMention, Clarification? Clarification, ReplyFact[] ReplyFacts);
public sealed record Expectations(ExpectedCall[] ToolCalls, MentionGroup[] ReplyMustMention, string[] ReplyMustNotMention, string[] Canaries, string DatabaseDiff, ExpectationPath[] Alternatives, Clarification? Clarification, ReplyFact[] ReplyFacts);
