using System.Text.RegularExpressions;

namespace TutoringCentre.Architecture.Tests;

/// <summary>
/// Source scan (Task 20.7, rule A): NetArchTest inspects compiled type dependencies, not method calls, so a call
/// that bypasses the tenant query filter or runs raw SQL on a tenant table has to be caught by reading the actual
/// .cs text. Everywhere outside the allow-list, these calls are forbidden; the allow-list exists because each
/// listed place already proved it needs them (Day 19/20), not because the rule is inconvenient elsewhere.
/// </summary>
public sealed class QueryFilterBypassTests
{
    private static readonly string[] ForbiddenMethodNames =
    [
        "IgnoreQueryFilters",
        "FromSqlRaw",
        "FromSql",
        "SqlQuery",
        "ExecuteSqlRaw",
        "ExecuteSql",
    ];

    // A call like "x.ExecuteSqlRawAsync(...)" or "x.SqlQuery<T>(...)" still counts: \w* absorbs the Async/Raw/
    // Interpolated suffix, and [(<] admits a generic argument list before the parentheses.
    private static readonly Regex ForbiddenCallPattern = new(
        $@"\b(?:{string.Join('|', ForbiddenMethodNames)})\w*\s*[(<]",
        RegexOptions.Compiled);

    // Exactly: the unit of work (the one place SET TRANSACTION READ ONLY runs), the runtime-role health check,
    // every migration (generated, never hand-reviewed line by line), the code that supports running them, and
    // the membership repository (Task 29.3: locks identity.memberships' active-owner rows with SELECT ... FOR
    // UPDATE — a locking clause PostgreSQL does not allow EF's LINQ translation to express).
    private static readonly string[] AllowListedFiles =
    [
        "src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs",
        "src/TutoringCentre.Infrastructure/Persistence/RuntimeRolePrivilegeHealthCheck.cs",
        "src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs",
        "src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs",
    ];

    private const string MigrationsDirectoryPrefix = "src/TutoringCentre.Infrastructure/Persistence/Migrations/";

    [Fact]
    public void ProductionSource_ForbidsQueryFilterBypassAndRawSqlOutsideTheAllowList()
    {
        var files = SourceFiles.EnumerateProductionSourceFiles().ToList();
        Assert.NotEmpty(files);

        var violations = new List<string>();
        foreach (var file in files)
        {
            var relativePath = SourceFiles.ToRepositoryRelativePath(file);
            if (IsAllowListed(relativePath))
            {
                continue;
            }

            var text = File.ReadAllText(file);
            if (ForbiddenCallPattern.IsMatch(text))
            {
                violations.Add(relativePath);
            }
        }

        Assert.True(violations.Count == 0, "Query-filter bypass or raw SQL outside the allow-list in: " + string.Join(", ", violations));
    }

    private static bool IsAllowListed(string relativePath) =>
        AllowListedFiles.Contains(relativePath, StringComparer.Ordinal)
        || relativePath.StartsWith(MigrationsDirectoryPrefix, StringComparison.Ordinal);
}
