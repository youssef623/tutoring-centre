using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TutoringCentre.Infrastructure.Persistence.Migrations;

/// <summary>
/// What every migration that adds a tenant-owned table calls (from Day 23) to enforce the same centre_id
/// boundary (Task 21.1's app.current_centre) inside PostgreSQL itself, regardless of how the row is reached —
/// the backstop the EF query filter and write guard (Day 20) cannot be, since both run only inside the
/// application process. FORCE means even tutoring_owner, the table's own owner, is bound by the policy; only an
/// actual superuser or a BYPASSRLS role escapes it, and the Day 19 readiness check fails closed if the runtime
/// connection is ever either.
/// </summary>
internal static class TenantRowLevelSecurity
{
    // Defined exactly once: the migration helper below and PostgresFixture (Infrastructure.Tests) both call
    // BuildEnableStatements, so the probe table proves the exact SQL a real migration runs, not a stand-in for it.
    private const string PolicyName = "tenant_isolation";
    private const string PolicyExpression = "centre_id = nullif(current_setting('app.current_centre', true), '')::uuid";

    // An unquoted Postgres identifier: a letter or underscore, then letters, digits or underscores. Rejects
    // anything that could close the identifier or the statement early — a quote, a semicolon, whitespace.
    private static readonly Regex IdentifierPattern = new("^[a-zA-Z_][a-zA-Z0-9_]*$", RegexOptions.Compiled);

    public static IReadOnlyList<string> BuildEnableStatements(string schema, string table)
    {
        var qualifiedTable = QualifyTable(schema, table);
        return
        [
            $"alter table {qualifiedTable} enable row level security;",
            $"alter table {qualifiedTable} force row level security;",
            $"create policy {PolicyName} on {qualifiedTable} for all using ({PolicyExpression}) with check ({PolicyExpression});",
        ];
    }

    public static IReadOnlyList<string> BuildDisableStatements(string schema, string table)
    {
        var qualifiedTable = QualifyTable(schema, table);
        return
        [
            $"drop policy {PolicyName} on {qualifiedTable};",
            $"alter table {qualifiedTable} no force row level security;",
            $"alter table {qualifiedTable} disable row level security;",
        ];
    }

    public static void EnableTenantRowLevelSecurity(this MigrationBuilder migrationBuilder, string schema, string table)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);
        foreach (var statement in BuildEnableStatements(schema, table))
        {
            migrationBuilder.Sql(statement);
        }
    }

    public static void DisableTenantRowLevelSecurity(this MigrationBuilder migrationBuilder, string schema, string table)
    {
        ArgumentNullException.ThrowIfNull(migrationBuilder);
        foreach (var statement in BuildDisableStatements(schema, table))
        {
            migrationBuilder.Sql(statement);
        }
    }

    private static string QualifyTable(string schema, string table)
    {
        ValidateIdentifier(schema, nameof(schema));
        ValidateIdentifier(table, nameof(table));
        return $"{schema}.{table}";
    }

    private static void ValidateIdentifier(string? identifier, string parameterName)
    {
        if (identifier is null || !IdentifierPattern.IsMatch(identifier))
        {
            throw new ArgumentException($"'{identifier}' is not a safe, unquoted SQL identifier.", parameterName);
        }
    }
}
