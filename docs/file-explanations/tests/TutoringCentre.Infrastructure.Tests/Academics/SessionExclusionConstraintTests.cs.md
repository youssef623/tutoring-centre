# `tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 52.11: the double-booking rules proven against the constraints alone — raw SQL as the superuser, no EF, no handler, no row-level security. The use-case tests prove the same rules again through translation and rollback; these prove the database refuses even a caller that skips the application entirely.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 140 lines and 7,256 bytes. Source hash: `e35cd364921f21a829fb331852aa59c4ae3da41e1f3c32dcf7d8b8678518caeb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L11): `public sealed class SessionExclusionConstraintTests(PostgresFixture fixture) : ScheduleTestBase(fixture)`
- [Line 19](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L19): `public async Task OverlappingScheduledSessionsForOneTeacher_AreRefusedByTheTeacherConstraint()`
- [Line 32](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L32): `public async Task BackToBackSessionsForOneTeacher_AreAllowedBecauseTheRangeIsHalfOpen()`
- [Line 44](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L44): `public async Task OverlapWithACancelledSession_IsAllowedBecauseTheConstraintsArePartial()`
- [Line 55](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L55): `public async Task TheSameTimesAndTeacherIdUnderAnotherCentre_AreAllowedBecauseTheConstraintsIncludeCentreId()`
- [Line 71](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L71): `public async Task OverlappingSessionsForOneGroupWithDifferentTeachers_AreRefusedByTheGroupConstraint()`
- [Line 84](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L84): `public async Task ASecondSessionForTheSameRuleAndLocalDate_IsRefusedByTheUniqueIndex()`
- [Line 97](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L97): `public async Task Catalogue_ClassSessionsHasBothExclusionConstraints_TenantScopedHalfOpenAndPartial()`
- [Line 127](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L127): `private async Task<Seeded> SeedAsync(Guid centreId)`
- [Line 139](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SessionExclusionConstraintTests.cs#L139): `private sealed record Seeded(Guid TeacherA, Guid TeacherB, Guid GroupA, Guid GroupB, Guid RuleA, Guid RuleB);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs:265](../../../../../tests/TutoringCentre.Api.Tests/Auth/LoginFlowTests.cs#L265)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs:102](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentIntegrityTests.cs#L102)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs:90](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentRowLevelSecurityTests.cs#L90)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs:110](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupRowLevelSecurityTests.cs#L110)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs:121](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleRowLevelSecurityTests.cs#L121)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Npgsql;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Task 52.11: the double-booking rules proven against the constraints alone — raw SQL as the superuser, no EF,
/// no handler, no row-level security. The use-case tests prove the same rules again through translation and
/// rollback; these prove the database refuses even a caller that skips the application entirely.
/// </summary>
public sealed class SessionExclusionConstraintTests(PostgresFixture fixture) : ScheduleTestBase(fixture)
{
    private const string ExclusionViolation = "23P01";
    private const string UniqueViolation = "23505";

    private static readonly DateOnly Saturday = new(2027, 3, 6);

    [Fact]
    public async Task OverlappingScheduledSessionsForOneTeacher_AreRefusedByTheTeacherConstraint()
    {
        var nile = await SeedAsync(NileCentreId);
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30)));

        var exception = await Assert.ThrowsAsync<PostgresException>(() =>
            InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupB, nile.TeacherA, nile.RuleB, Saturday, At(17), At(18, 30))));

        Assert.Equal(ExclusionViolation, exception.SqlState);
        Assert.Equal("ex_class_sessions_teacher_overlap", exception.ConstraintName);
    }

    [Fact]
    public async Task BackToBackSessionsForOneTeacher_AreAllowedBecauseTheRangeIsHalfOpen()
    {
        var nile = await SeedAsync(NileCentreId);

        // 17:30-19:00 and 19:00-20:30 local (Cairo winter, UTC+2): one ends at the instant the other starts.
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(15, 30), At(17)));
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupB, nile.TeacherA, nile.RuleB, Saturday, At(17), At(18, 30)));

        Assert.Equal(2, await SessionCountAsync());
    }

    [Fact]
    public async Task OverlapWithACancelledSession_IsAllowedBecauseTheConstraintsArePartial()
    {
        var nile = await SeedAsync(NileCentreId);
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30), "cancelled"));

        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleB, Saturday, At(16), At(17, 30)));

        Assert.Equal(2, await SessionCountAsync());
    }

    [Fact]
    public async Task TheSameTimesAndTeacherIdUnderAnotherCentre_AreAllowedBecauseTheConstraintsIncludeCentreId()
    {
        var nile = await SeedAsync(NileCentreId);
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30)));

        // Same teacher and group ID values, same instants, Maadi's centre_id. No real Maadi teacher can carry Nile's
        // teacher ID (the composite foreign keys forbid it), so foreign-key triggers are skipped for this row only:
        // what is under test is the exclusion constraints' centre_id, which index checks still enforce.
        await InsertSessionAsync(
            new SessionRow(MaadiCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30)),
            withoutForeignKeys: true);

        Assert.Equal(2, await SessionCountAsync());
    }

    [Fact]
    public async Task OverlappingSessionsForOneGroupWithDifferentTeachers_AreRefusedByTheGroupConstraint()
    {
        var nile = await SeedAsync(NileCentreId);
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30)));

        var exception = await Assert.ThrowsAsync<PostgresException>(() =>
            InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherB, nile.RuleB, Saturday, At(17), At(18, 30))));

        Assert.Equal(ExclusionViolation, exception.SqlState);
        Assert.Equal("ex_class_sessions_group_overlap", exception.ConstraintName);
    }

    [Fact]
    public async Task ASecondSessionForTheSameRuleAndLocalDate_IsRefusedByTheUniqueIndex()
    {
        var nile = await SeedAsync(NileCentreId);
        await InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(8), At(9, 30)));

        var exception = await Assert.ThrowsAsync<PostgresException>(() =>
            InsertSessionAsync(new SessionRow(NileCentreId, nile.GroupA, nile.TeacherA, nile.RuleA, Saturday, At(16), At(17, 30))));
```

The excerpt stops at line 90; use the original file for the remaining 50 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
