namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>PostgreSQL schema per feature module. Every entity configuration uses these in ToTable(name, schema).</summary>
internal static class Schemas
{
    public const string Platform = "platform";
    public const string Identity = "identity";
    public const string Academics = "academics";
    public const string Audit = "audit";
    public const string Attendance = "attendance";
    public const string Conversations = "conversations";
    public const string Assistant = "assistant";
    public const string Jobs = "jobs";
}
