# `tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentIntegrityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 38.4: proves the composite foreign keys and unique constraints hold even for the superuser, who bypasses row-level security entirely — the guarantee RLS alone cannot give (docs/architecture/tenancy.md, Layer 4). Rows are seeded and the violating statement run over the owner connection directly, never through the application.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentIntegrityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 116 lines and 5,718 bytes. Source hash: `2d28096c206697ed87d2fb6119c508e091d9ff681d0c55e3efda662f5966e897`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 3: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentIntegrityTests.cs#L13): `public sealed class StudentParentIntegrityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentIntegrityTests.cs#L16): `public async Task LinkWithNileCentreButMaadiParent_Throws23503()`
- [Line 40](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentIntegrityTests.cs#L40): `public async Task SamePhoneTwiceInNile_Throws23505OnUxParentsCentrePhone()`
- [Line 61](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentIntegrityTests.cs#L61): `public async Task SamePhoneOnceInEachCentre_Succeeds()`
- [Line 73](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentIntegrityTests.cs#L73): `public async Task TwoPrimaryLinksForOneStudent_Throws23505OnUxStudentParentsOnePrimary()`
- [Line 99](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/StudentParentIntegrityTests.cs#L99): `public async Task NonE164Phone_Throws23514()`

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
/// Task 38.4: proves the composite foreign keys and unique constraints hold even for the superuser, who
/// bypasses row-level security entirely — the guarantee RLS alone cannot give (docs/architecture/tenancy.md,
/// Layer 4). Rows are seeded and the violating statement run over the owner connection directly, never through
/// the application.
/// </summary>
public sealed class StudentParentIntegrityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    [Fact]
    public async Task LinkWithNileCentreButMaadiParent_Throws23503()
    {
        var nileStudent = await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        var maadiParent = await Fixture.SeedParentAsync(MaadiCentreId, "Laila Said", "+201112345678");

        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.student_parents (id, centre_id, student_id, parent_id, relationship, is_primary, created_at)
            values (@id, @centre_id, @student_id, @parent_id, 'father', true, now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);
        command.Parameters.AddWithValue("student_id", nileStudent);
        command.Parameters.AddWithValue("parent_id", maadiParent);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("23503", exception.SqlState);
        Assert.Equal("fk_student_parents_parents", exception.ConstraintName);
    }

    [Fact]
    public async Task SamePhoneTwiceInNile_Throws23505OnUxParentsCentrePhone()
    {
        await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");

        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.parents (id, centre_id, full_name, search_name, phone, created_at)
            values (@id, @centre_id, 'Another Parent', 'ANOTHER PARENT', '+201012345678', now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
        Assert.Equal("23505", exception.SqlState);
        Assert.Equal("ux_parents_centre_phone", exception.ConstraintName);
    }

    [Fact]
    public async Task SamePhoneOnceInEachCentre_Succeeds()
    {
        await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");

        var maadiParentId = await Fixture.SeedParentAsync(MaadiCentreId, "Omar Hassan (Maadi)", "+201012345678");

        Assert.Equal(
            1,
            await Fixture.ScalarAsync<long>($"select count(*) from academics.parents where id = '{maadiParentId}'"));
    }

    [Fact]
    public async Task TwoPrimaryLinksForOneStudent_Throws23505OnUxStudentParentsOnePrimary()
    {
        var nileStudent = await Fixture.SeedStudentAsync(NileCentreId, "Youssef Ali");
        var firstParent = await Fixture.SeedParentAsync(NileCentreId, "Omar Hassan", "+201012345678");
        var secondParent = await Fixture.SeedParentAsync(NileCentreId, "Amal Fathy", "+201212345678");
        await Fixture.SeedStudentParentLinkAsync(NileCentreId, nileStudent, firstParent, isPrimary: true);

        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.student_parents (id, centre_id, student_id, parent_id, relationship, is_primary, created_at)
            values (@id, @centre_id, @student_id, @parent_id, 'mother', true, now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", NileCentreId);
        command.Parameters.AddWithValue("student_id", nileStudent);
```

The excerpt stops at line 90; use the original file for the remaining 26 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
