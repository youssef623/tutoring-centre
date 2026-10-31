namespace TutoringCentre.Evaluation.Cases;

public sealed record EvalCase(string Id, CaseTags Tags, string Persona, EvalTurn[] Turns, Expectations Expect);
public sealed record CaseTags(string LanguageStyle, string Category);
