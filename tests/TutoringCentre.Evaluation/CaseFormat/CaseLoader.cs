using System.Text.Json;
using System.Text.Json.Serialization;
namespace TutoringCentre.Evaluation.Cases;

public static class CaseLoader
{
    public static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web)
    {
        PropertyNameCaseInsensitive = false,
        NumberHandling = JsonNumberHandling.Strict,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        RespectNullableAnnotations = true,
        RespectRequiredConstructorParameters = true,
        WriteIndented = true
    };
    public static IReadOnlyList<EvalCase> Load(string directory)
    {
        var cases = Directory.GetFiles(directory, "*.json").Where(p => Path.GetFileName(p) != "schema.json").Select(p => Parse(File.ReadAllText(p))).OrderBy(c => int.Parse(c.Id[1..], System.Globalization.CultureInfo.InvariantCulture)).ToArray();
        if (cases.Select(c => c.Id).Distinct(StringComparer.Ordinal).Count() != cases.Length) throw new InvalidDataException("eval.duplicate_id");
        return cases;
    }
    public static EvalCase Parse(string text)
    {
        var c = JsonSerializer.Deserialize<EvalCase>(text, Json) ?? throw new InvalidDataException("eval.case_invalid");
        if (!System.Text.RegularExpressions.Regex.IsMatch(c.Id, "^E[1-9][0-9]*$", System.Text.RegularExpressions.RegexOptions.CultureInvariant, TimeSpan.FromSeconds(1)) || !new[] { "egyptian", "msa", "franco", "english", "mixed" }.Contains(c.Tags.LanguageStyle, StringComparer.Ordinal) || !new[] { "attendance", "ambiguity", "attack", "out_of_scope" }.Contains(c.Tags.Category, StringComparer.Ordinal) || string.IsNullOrWhiteSpace(c.Persona) || c.Turns.Length == 0 || c.Turns.Any(t => string.IsNullOrWhiteSpace(t.Text)) || c.Expect.DatabaseDiff != "none") throw new InvalidDataException("eval.case_invalid");
        ValidatePath(new(c.Expect.ToolCalls, c.Expect.ReplyMustMention, c.Expect.Clarification, c.Expect.ReplyFacts));
        foreach (var p in c.Expect.Alternatives) ValidatePath(p);
        return c;
    }
    private static void ValidatePath(ExpectationPath path)
    {
        Validate(path.ToolCalls, path.ReplyMustMention);
        foreach (var fact in path.ReplyFacts)
        {
            Validate([], [fact.Subject, fact.Status]);
            if (fact.MustNotMention.Any(string.IsNullOrWhiteSpace)) throw new InvalidDataException("eval.fact_invalid");
        }
        if (path.Clarification is { } x) { Validate([], x.MustMention); if (path.ToolCalls.Length != 0 || x.MustMention.Length == 0 || x.MustNotMention.Length == 0 || x.MustNotMention.Any(string.IsNullOrWhiteSpace)) throw new InvalidDataException("eval.clarification_invalid"); }
    }
    private static void Validate(ExpectedCall[] calls, MentionGroup[] groups)
    {
        if (calls.Any(c => c.Name != "get_attendance" || string.IsNullOrWhiteSpace(c.ChildHandleOf) || c.Count is < 1 or > 6) || groups.Any(g => g.AnyOf.Length == 0 || g.AnyOf.Any(string.IsNullOrWhiteSpace))) throw new InvalidDataException("eval.expect_invalid");
    }
}
