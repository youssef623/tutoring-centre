# `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 47.5: the decisive row-level security checks on the real academics.enrollments table, as the real runtime role, independent of EF or the application pipeline. Rows are seeded as the superuser so the policy under test cannot have biased them.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 134 lines and 6,127 bytes. Source hash: `77728fa53d0875e52e92636e3ca312e67e1918348cbfbe652c56f4662281094c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 3: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L12): `public sealed class EnrollmentRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L17): `public async Task NoSetting_SelectCount_IsZero()`
- [Line 29](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L29): `public async Task SettingNile_SelectsOnlyNileRows()`
- [Line 46](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L46): `public async Task SettingNile_InsertWithMaadiCentre_Throws42501()`
- [Line 71](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L71): `public async Task SettingNile_DeleteOwnEnrollment_Throws42501BecauseDeleteIsNotGranted()`
- [Line 95](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L95): `private async Task<Guid> SeedEnrollmentAsync(Guid centreId)`
- [Line 115](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L115): `private async Task<NpgsqlConnection> OpenAppConnectionAsync()`
- [Line 122](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L122): `private static async Task SetCentreAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, Guid centreId)`
- [Line 129](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L129): `private static async Task<T> ScalarAsync<T>(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 47.5: the decisive row-level security checks on the real academics.enrollments table, as the real
/// runtime role, independent of EF or the application pipeline. Rows are seeded as the superuser so the policy
/// under test cannot have biased them.
/// </summary>
public sealed class EnrollmentRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string PermissionDenied = "42501";

    [Fact]
    public async Task NoSetting_SelectCount_IsZero()
    {
        await SeedEnrollmentAsync(NileCentreId);
        await SeedEnrollmentAsync(MaadiCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();

        Assert.Equal(0, await ScalarAsync<long>(connection, transaction, "select count(*) from academics.enrollments"));
    }

    [Fact]
    public async Task SettingNile_SelectsOnlyNileRows()
    {
        await SeedEnrollmentAsync(NileCentreId);
        await SeedEnrollmentAsync(NileCentreId);
        await SeedEnrollmentAsync(MaadiCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        Assert.Equal(2, await ScalarAsync<long>(connection, transaction, "select count(*) from academics.enrollments"));
        Assert.Equal(
            0,
            await ScalarAsync<long>(connection, transaction, $"select count(*) from academics.enrollments where centre_id <> '{NileCentreId}'"));
    }

    [Fact]
    public async Task SettingNile_InsertWithMaadiCentre_Throws42501()
    {
        var (maadiStudentId, maadiGroupId) = await SeedReferencesAsync(MaadiCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        await using var command = new NpgsqlCommand(
            """
            insert into academics.enrollments (id, centre_id, student_id, group_id, status, enrolled_at, created_at)
            values (@id, @centre_id, @student_id, @group_id, 'active', now(), now())
            """,
            connection,
            transaction);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", MaadiCentreId);
        command.Parameters.AddWithValue("student_id", maadiStudentId);
        command.Parameters.AddWithValue("group_id", maadiGroupId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    [Fact]
    public async Task SettingNile_DeleteOwnEnrollment_Throws42501BecauseDeleteIsNotGranted()
    {
        var nileEnrollmentId = await SeedEnrollmentAsync(NileCentreId);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId);

        await using var delete = new NpgsqlCommand("delete from academics.enrollments where id = @id", connection, transaction);
        delete.Parameters.AddWithValue("id", nileEnrollmentId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => delete.ExecuteNonQueryAsync());
        Assert.Equal(PermissionDenied, exception.SqlState);
    }

    private async Task<(Guid StudentId, Guid GroupId)> SeedReferencesAsync(Guid centreId)
    {
        var studentId = await Fixture.SeedStudentAsync(centreId, "Ahmed Mahmoud");
        var subjectId = await Fixture.SeedSubjectAsync(centreId, $"Subject {Guid.CreateVersion7()}");
        var teacherId = await Fixture.SeedTeacherAsync(centreId, "Seeded Teacher");
```

The excerpt stops at line 90; use the original file for the remaining 44 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
