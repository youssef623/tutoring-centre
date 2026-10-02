namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>PostgreSQL schema per feature module. Every entity configuration uses these in ToTable(name, schema).</summary>
internal static class Schemas
{
    public const string Platform = "platform";
    public const string Identity = "identity";
}
