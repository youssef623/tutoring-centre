# `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 47.5: the table's business constraints, as the superuser so row-level security is not what is under test. Composite foreign keys keep an enrollment inside one centre; the partial unique index allows at most one active enrollment per student and group while keeping ended ones as history; the checks make an ended row without its time, or ending before it started, unstorable.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 132 lines and 6,119 bytes. Source hash: `d553ce635e03c46ebd17e820d699bfb1a643ca65d3e882c2aed8d563e0d5509d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`
- Line 3: `using TutoringCentre.Infrastructure.Tests.Tenancy;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L13): `public sealed class EnrollmentIntegrityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)`
- [Line 22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L22): `public async Task NileEnrollmentJoiningNileStudentToMaadiGroup_Throws23503()`
- [Line 34](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L34): `public async Task NileEnrollmentJoiningMaadiStudentToNileGroup_Throws23503()`
- [Line 46](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L46): `public async Task TwoActiveEnrollmentsForOneStudentAndGroup_Throws23505()`
- [Line 59](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L59): `public async Task OneEndedAndOneActiveForOneStudentAndGroup_IsAllowed()`
- [Line 74](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L74): `public async Task EndedWithoutEndedAt_Throws23514()`
- [Line 87](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L87): `public async Task EndedBeforeEnrolled_Throws23514()`
- [Line 99](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L99): `private async Task<Guid> SeedGroupAsync(Guid centreId)`
- [Line 106](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L106): `private async Task InsertAsync(`

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
/// Task 47.5: the table's business constraints, as the superuser so row-level security is not what is under
/// test. Composite foreign keys keep an enrollment inside one centre; the partial unique index allows at most one
/// active enrollment per student and group while keeping ended ones as history; the checks make an ended row
/// without its time, or ending before it started, unstorable.
/// </summary>
public sealed class EnrollmentIntegrityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private const string ForeignKeyViolation = "23503";
    private const string UniqueViolation = "23505";
    private const string CheckViolation = "23514";

    private static readonly DateTimeOffset EnrolledAt = new(2026, 10, 1, 9, 0, 0, TimeSpan.Zero);

    [Fact]
    public async Task NileEnrollmentJoiningNileStudentToMaadiGroup_Throws23503()
    {
        var nileStudentId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Mahmoud");
        var maadiGroupId = await SeedGroupAsync(MaadiCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => InsertAsync(NileCentreId, nileStudentId, maadiGroupId));

        Assert.Equal(ForeignKeyViolation, exception.SqlState);
        Assert.Equal("fk_enrollments_groups", exception.ConstraintName);
    }

    [Fact]
    public async Task NileEnrollmentJoiningMaadiStudentToNileGroup_Throws23503()
    {
        var maadiStudentId = await Fixture.SeedStudentAsync(MaadiCentreId, "James Wilson");
        var nileGroupId = await SeedGroupAsync(NileCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => InsertAsync(NileCentreId, maadiStudentId, nileGroupId));

        Assert.Equal(ForeignKeyViolation, exception.SqlState);
        Assert.Equal("fk_enrollments_students", exception.ConstraintName);
    }

    [Fact]
    public async Task TwoActiveEnrollmentsForOneStudentAndGroup_Throws23505()
    {
        var studentId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Mahmoud");
        var groupId = await SeedGroupAsync(NileCentreId);
        await InsertAsync(NileCentreId, studentId, groupId);

        var exception = await Assert.ThrowsAsync<PostgresException>(() => InsertAsync(NileCentreId, studentId, groupId));

        Assert.Equal(UniqueViolation, exception.SqlState);
        Assert.Equal("ux_enrollments_active_student_group", exception.ConstraintName);
    }

    [Fact]
    public async Task OneEndedAndOneActiveForOneStudentAndGroup_IsAllowed()
    {
        var studentId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Mahmoud");
        var groupId = await SeedGroupAsync(NileCentreId);
        await InsertAsync(NileCentreId, studentId, groupId, status: "ended", endedAt: EnrolledAt.AddDays(10), endReason: "withdrawn");

        await InsertAsync(NileCentreId, studentId, groupId);

        Assert.Equal(
            2,
            await Fixture.ScalarAsync<long>(
                $"select count(*) from academics.enrollments where student_id = '{studentId}' and group_id = '{groupId}'"));
    }

    [Fact]
    public async Task EndedWithoutEndedAt_Throws23514()
    {
        var studentId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Mahmoud");
        var groupId = await SeedGroupAsync(NileCentreId);

        var exception = await Assert.ThrowsAsync<PostgresException>(
            () => InsertAsync(NileCentreId, studentId, groupId, status: "ended", endedAt: null, endReason: "withdrawn"));

        Assert.Equal(CheckViolation, exception.SqlState);
        Assert.Equal("ck_enrollments_end_consistency", exception.ConstraintName);
    }

    [Fact]
    public async Task EndedBeforeEnrolled_Throws23514()
    {
        var studentId = await Fixture.SeedStudentAsync(NileCentreId, "Ahmed Mahmoud");
        var groupId = await SeedGroupAsync(NileCentreId);
```

The excerpt stops at line 90; use the original file for the remaining 42 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
