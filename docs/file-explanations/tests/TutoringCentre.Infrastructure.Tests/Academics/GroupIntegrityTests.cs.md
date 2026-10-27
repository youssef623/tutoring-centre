# `tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 44.14: the raw constraints, as the superuser so row-level security is not what is under test. The composite foreign keys make a Nile group that points at a Maadi subject or teacher unstorable even through privileged SQL; the check constraint holds capacity to 1–200.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 73 lines and 3,276 bytes. Source hash: `a9a721ddd3d50a37becd178c7547a8b11691b93fe09570dae2db932a5aac5a4c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 3: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs#L12): `public sealed class GroupIntegrityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs#L18): `public async Task NileGroupWithMaadiSubject_Throws23503()`
- [Line 30](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs#L30): `public async Task NileGroupWithMaadiTeacher_Throws23503()`
- [Line 44](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs#L44): `public async Task CapacityOutOfRange_Throws23514(int capacity)`
- [Line 55](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupIntegrityTests.cs#L55): `private async Task<PostgresException> InsertAsync(Guid centreId, Guid subjectId, Guid teacherId, int capacity)`

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
/// Task 44.14: the raw constraints, as the superuser so row-level security is not what is under test. The
/// composite foreign keys make a Nile group that points at a Maadi subject or teacher unstorable even through
/// privileged SQL; the check constraint holds capacity to 1–200.
/// </summary>
public sealed class GroupIntegrityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string ForeignKeyViolation = "23503";
    private const string CheckViolation = "23514";

    [Fact]
    public async Task NileGroupWithMaadiSubject_Throws23503()
    {
        var maadiSubjectId = await Fixture.SeedSubjectAsync(MaadiCentreId, "Mathematics");
        var nileTeacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        var exception = await InsertAsync(NileCentreId, maadiSubjectId, nileTeacherId, 20);

        Assert.Equal(ForeignKeyViolation, exception.SqlState);
        Assert.Equal("fk_groups_subjects", exception.ConstraintName);
    }

    [Fact]
    public async Task NileGroupWithMaadiTeacher_Throws23503()
    {
        var nileSubjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var maadiTeacherId = await Fixture.SeedTeacherAsync(MaadiCentreId, "Mona Said");

        var exception = await InsertAsync(NileCentreId, nileSubjectId, maadiTeacherId, 20);

        Assert.Equal(ForeignKeyViolation, exception.SqlState);
        Assert.Equal("fk_groups_teachers", exception.ConstraintName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public async Task CapacityOutOfRange_Throws23514(int capacity)
    {
        var nileSubjectId = await Fixture.SeedSubjectAsync(NileCentreId, "Mathematics");
        var nileTeacherId = await Fixture.SeedTeacherAsync(NileCentreId, "Amina Hassan");

        var exception = await InsertAsync(NileCentreId, nileSubjectId, nileTeacherId, capacity);

        Assert.Equal(CheckViolation, exception.SqlState);
        Assert.Equal("ck_groups_capacity", exception.ConstraintName);
    }

    private async Task<PostgresException> InsertAsync(Guid centreId, Guid subjectId, Guid teacherId, int capacity)
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            """
            insert into academics.groups (id, centre_id, name, normalized_name, subject_id, teacher_id, capacity, status, created_at)
            values (@id, @centre_id, 'Grade 10 A', 'GRADE 10 A', @subject_id, @teacher_id, @capacity, 'active', now())
            """,
            connection);
        command.Parameters.AddWithValue("id", Guid.CreateVersion7());
        command.Parameters.AddWithValue("centre_id", centreId);
        command.Parameters.AddWithValue("subject_id", subjectId);
        command.Parameters.AddWithValue("teacher_id", teacherId);
        command.Parameters.AddWithValue("capacity", capacity);

        return await Assert.ThrowsAsync<PostgresException>(() => command.ExecuteNonQueryAsync());
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
