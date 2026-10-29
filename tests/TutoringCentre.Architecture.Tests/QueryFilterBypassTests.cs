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
        "NpgsqlCommand",
        "CreateCommand",
    ];

    // A call like "x.ExecuteSqlRawAsync(...)" or "x.SqlQuery<T>(...)" still counts: \w* absorbs the Async/Raw/
    // Interpolated suffix, and [(<] admits a generic argument list before the parentheses.
    private static readonly Regex ForbiddenCallPattern = new(
        $@"\b(?:{string.Join('|', ForbiddenMethodNames)})\w*\s*[(<]",
        RegexOptions.Compiled);

    // Exactly: the unit of work (the one place SET TRANSACTION READ ONLY runs), the runtime-role health check,
    // every migration (generated, never hand-reviewed line by line), the code that supports running them, and
    // the membership repository (Task 29.3: locks identity.memberships' active-owner rows with SELECT ... FOR
    // UPDATE — a locking clause PostgreSQL does not allow EF's LINQ translation to express), and the group
    // repository (Task 48.1: GetForEnrollmentAsync locks one academics.groups row with SELECT ... FOR UPDATE
    // before counting its active enrollments), and the class-session repository (Day 53: LockTeacherSchedulesAsync
    // takes a transaction-scoped advisory lock per teacher, which reads no table and has no LINQ form).
    private static readonly string[] AllowListedFiles =
    [
        "src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs",
        "src/TutoringCentre.Infrastructure/Persistence/RuntimeRolePrivilegeHealthCheck.cs",
        "src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs",
        "src/TutoringCentre.Infrastructure/Repositories/MembershipRepository.cs",
        "src/TutoringCentre.Infrastructure/Repositories/GroupRepository.cs",
        "src/TutoringCentre.Infrastructure/Repositories/ClassSessionRepository.cs",
        // Day 57: parameterized FOR UPDATE serializes existing marks without client versions.
        // EF's tenant filter composes over the query; forced RLS also applies. First marks still race on uniqueness.
        "src/TutoringCentre.Infrastructure/Repositories/AttendanceRecordRepository.cs",
        // SKIP LOCKED claim; RLS-exempt table. Each delivered command re-enters its tenant scope.
        "src/TutoringCentre.Infrastructure/Messaging/OutboxStore.cs",
        // Before tenant selection: active provider account lookup returns IDs only.
        "src/TutoringCentre.Infrastructure/Conversations/InboundChannelResolver.cs",
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
