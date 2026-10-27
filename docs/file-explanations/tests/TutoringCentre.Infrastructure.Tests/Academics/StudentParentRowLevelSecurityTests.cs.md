# `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 38.4: repeats the decisive row-level security checks on the three new tables, as the real runtime role, independent of EF or the application pipeline. Parents and students have no DELETE privilege at all (Task 38.3); student_parents does, since links are removed and the removal is audited.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 266 lines and 12,502 bytes. Source hash: `80e9727193e5f2c9f108b93ebee7bd46d0ad9f5a1e73e83e8806826b00b0ea94`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 3: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L12): `public sealed class StudentParentRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L17): `public async Task NoSetting_Parents_SelectCountIsZero()`
- [Line 30](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L30): `public async Task NoSetting_Students_SelectCountIsZero()`
- [Line 43](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L43): `public async Task NoSetting_StudentParents_SelectCountIsZero()`
- [Line 57](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L57): `public async Task SettingNile_Parents_SelectsOnlyNileRows()`
- [Line 74](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L74): `public async Task SettingNile_Students_SelectsOnlyNileRows()`
- [Line 91](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L91): `public async Task SettingNile_StudentParents_SelectsOnlyNileRows()`
- [Line 111](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L111): `public async Task SettingNile_InsertParentWithMaadiCentre_Throws42501()`
- [Line 132](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L132): `public async Task SettingNile_InsertStudentWithMaadiCentre_Throws42501()`
- [Line 153](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L153): `public async Task SettingNile_InsertStudentParentWithMaadiCentre_Throws42501()`
- [Line 179](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L179): `public async Task Delete_AnyParent_Throws42501NoPrivilege()`
- [Line 195](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L195): `public async Task Delete_AnyStudent_Throws42501NoPrivilege()`
- [Line 211](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L211): `public async Task SettingNile_DeleteNileLink_AffectsOneRow()`
- [Line 234](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L234): `private async Task<NpgsqlConnection> OpenAppConnectionAsync()`
- [Line 241](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L241): `private static async Task SetCentreAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string value)`
- [Line 248](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L248): `private static async Task<List<Guid>> SelectCentreIdsAsync(NpgsqlConnection connection, NpgsqlTransaction transaction, string table)`
- [Line 261](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentRowLevelSecurityTests.cs#L261): `private static async Task<T> ScalarAsync<T>(NpgsqlConnection connection, NpgsqlTransaction transaction, string sql)`

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
/// Task 38.4: repeats the decisive row-level security checks on the three new tables, as the real runtime
/// role, independent of EF or the application pipeline. Parents and students have no DELETE privilege at all
/// (Task 38.3); student_parents does, since links are removed and the removal is audited.
/// </summary>
public sealed class StudentParentRowLevelSecurityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string PermissionDenied = "42501";

    [Fact]
    public async Task NoSetting_Parents_SelectCountIsZero()
    {
        await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        await Fixture.SeedParentAsync(MaadiCentreId, "Laila Said", "+201112345678");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from academics.parents");

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task NoSetting_Students_SelectCountIsZero()
    {
        await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        await Fixture.SeedStudentAsync(MaadiCentreId, "Sara Adel");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from academics.students");

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task NoSetting_StudentParents_SelectCountIsZero()
    {
        var nileParent = await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        var nileStudent = await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, nileStudent, nileParent, isPrimary: true);

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        var count = await ScalarAsync<long>(connection, transaction, "select count(*) from academics.student_parents");

        Assert.Equal(0, count);
    }

    [Fact]
    public async Task SettingNile_Parents_SelectsOnlyNileRows()
    {
        await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        await Fixture.SeedParentAsync(NileCentreId, "Amal Fathy", "+201212345678");
        await Fixture.SeedParentAsync(MaadiCentreId, "Laila Said", "+201112345678");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        var centreIds = await SelectCentreIdsAsync(connection, transaction, "academics.parents");

        Assert.Equal(2, centreIds.Count);
        Assert.All(centreIds, id => Assert.Equal(NileCentreId, id));
    }

    [Fact]
    public async Task SettingNile_Students_SelectsOnlyNileRows()
    {
        await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        await Fixture.SeedStudentAsync(NileCentreId, "Mona Samir");
        await Fixture.SeedStudentAsync(MaadiCentreId, "Sara Adel");

        await using var connection = await OpenAppConnectionAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await SetCentreAsync(connection, transaction, NileCentreId.ToString());

        var centreIds = await SelectCentreIdsAsync(connection, transaction, "academics.students");

        Assert.Equal(2, centreIds.Count);
        Assert.All(centreIds, id => Assert.Equal(NileCentreId, id));
    }

    [Fact]
```

The excerpt stops at line 90; use the original file for the remaining 176 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
