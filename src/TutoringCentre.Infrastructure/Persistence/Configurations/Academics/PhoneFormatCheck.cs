namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>
/// The database-level backstop for E.164 shape, shared by academics.parents and academics.students (Day 38
/// contract): looser than PhoneNumber's own validation (no Egyptian-prefix check), since the Domain already
/// fully validates before save — this check only catches a row that reached the column some other way.
/// </summary>
internal static class PhoneFormatCheck
{
    public const string Sql = """phone ~ '^\+[1-9][0-9]{7,14}$'""";

    public const string SqlOrNull = $"""phone IS NULL OR {Sql}""";
}
