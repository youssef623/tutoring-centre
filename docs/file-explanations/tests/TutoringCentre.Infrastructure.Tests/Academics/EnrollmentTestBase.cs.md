# `tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs`

## Purpose

Source summary (intent, not independent execution proof): Shared setup for Task 48.10's enrollment integration tests. Bulk rows (the students and the enrollments that fill a group before a race) are seeded directly as the superuser, in one connection, so twenty repetitions stay fast; everything under test goes through the real dispatcher as tutoring_app. Every assertion on persisted state reads through the superuser connection, never through the application's own filtered view.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 118 lines and 6,059 bytes. Source hash: `09b97510b05046ff943cb8f24bb9eb2825fdad3be76a6ce705b1b1a808873620`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Npgsql;`
- Line 2: `using TutoringCentre.Application.Academics.Enrollments;`
- Line 3: `using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;`
- Line 4: `using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;`
- Line 5: `using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;`
- Line 6: `using TutoringCentre.Application.Common.Security;`
- Line 7: `using TutoringCentre.Domain.Common;`
- Line 8: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L18): `public abstract class EnrollmentTestBase(PostgresFixture fixture) : GroupTestBase(fixture)`
- [Line 21](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L21): `private protected async Task<IReadOnlyList<Guid>> SeedStudentsAsync(Guid centreId, int count, string namePrefix)`
- [Line 48](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L48): `private protected async Task SeedEnrollmentsAsync(Guid centreId, Guid groupId, IEnumerable<Guid> studentIds)`
- [Line 69](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L69): `private protected Task<long> ActiveRowsAsync(Guid groupId) =>`
- [Line 72](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L72): `private protected Task<int> CapacityAsync(Guid groupId) =>`
- [Line 75](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L75): `private protected Task<Result<EnrollStudentResult>> EnrollAsync(Actor actor, Guid groupId, Guid studentId) =>`
- [Line 78](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L78): `private protected async Task<Guid> EnrollOkAsync(Actor actor, Guid groupId, Guid studentId)`
- [Line 85](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L85): `private protected async Task<GroupRosterDto> GetRosterAsync(Actor actor, Guid groupId)`
- [Line 92](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L92): `private protected async Task<StudentEnrollmentDto> GetStudentEnrollmentAsync(Actor actor, Guid studentId, Guid enrollmentId)`
- [Line 105](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTestBase.cs#L105): `private protected static async Task<T[]> RunTogetherAsync<T>(IEnumerable<Func<Task<T>>> attempts)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs:14](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTeacherScopeTests.cs#L14)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs:16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTenantIsolationTests.cs#L16)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs:13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentTransferTests.cs#L13)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs:24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/EnrollmentUseCaseTests.cs#L24)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs:17](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/LastSeatConcurrencyTests.cs#L17)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs:22](../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ScheduleTestBase.cs#L22)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Npgsql;
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;
using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Academics;

/// <summary>
/// Shared setup for Task 48.10's enrollment integration tests. Bulk rows (the students and the enrollments that
/// fill a group before a race) are seeded directly as the superuser, in one connection, so twenty repetitions
/// stay fast; everything under test goes through the real dispatcher as tutoring_app. Every assertion on
/// persisted state reads through the superuser connection, never through the application's own filtered view.
/// </summary>
public abstract class EnrollmentTestBase(PostgresFixture fixture) : GroupTestBase(fixture)
{
    /// <summary>Inserts <paramref name="count"/> active students for the centre as the superuser. Returns their ids.</summary>
    private protected async Task<IReadOnlyList<Guid>> SeedStudentsAsync(Guid centreId, int count, string namePrefix)
    {
        var ids = new List<Guid>(count);
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        for (var index = 0; index < count; index++)
        {
            var id = Guid.CreateVersion7();
            var fullName = $"{namePrefix} {index:D3}";
            await using var command = new NpgsqlCommand(
                """
                insert into academics.students (id, centre_id, full_name, search_name, status, links_revision, created_at)
                values (@id, @centre_id, @full_name, @search_name, 'active', 0, now())
                """,
                connection);
            command.Parameters.AddWithValue("id", id);
            command.Parameters.AddWithValue("centre_id", centreId);
            command.Parameters.AddWithValue("full_name", fullName);
            command.Parameters.AddWithValue("search_name", SearchText.Normalize(fullName));
            await command.ExecuteNonQueryAsync();
            ids.Add(id);
        }

        return ids;
    }

    /// <summary>Inserts one active enrollment per student into the group as the superuser.</summary>
    private protected async Task SeedEnrollmentsAsync(Guid centreId, Guid groupId, IEnumerable<Guid> studentIds)
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        foreach (var studentId in studentIds)
        {
            await using var command = new NpgsqlCommand(
                """
                insert into academics.enrollments (id, centre_id, student_id, group_id, status, enrolled_at, created_at)
                values (@id, @centre_id, @student_id, @group_id, 'active', now(), now())
                """,
                connection);
            command.Parameters.AddWithValue("id", Guid.CreateVersion7());
            command.Parameters.AddWithValue("centre_id", centreId);
            command.Parameters.AddWithValue("student_id", studentId);
            command.Parameters.AddWithValue("group_id", groupId);
            await command.ExecuteNonQueryAsync();
        }
    }

    /// <summary>The group's active enrollment rows, counted as the superuser.</summary>
    private protected Task<long> ActiveRowsAsync(Guid groupId) =>
        Fixture.ScalarAsync<long>($"select count(*) from academics.enrollments where group_id = '{groupId}' and status = 'active'");

    private protected Task<int> CapacityAsync(Guid groupId) =>
        Fixture.ScalarAsync<int>($"select capacity from academics.groups where id = '{groupId}'");

    private protected Task<Result<EnrollStudentResult>> EnrollAsync(Actor actor, Guid groupId, Guid studentId) =>
        Fixture.SendAsAsync<EnrollStudentCommand, EnrollStudentResult>(actor, new EnrollStudentCommand(groupId, studentId));

    private protected async Task<Guid> EnrollOkAsync(Actor actor, Guid groupId, Guid studentId)
    {
        var result = await EnrollAsync(actor, groupId, studentId);
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.EnrollmentId;
    }

    private protected async Task<GroupRosterDto> GetRosterAsync(Actor actor, Guid groupId)
    {
        var result = await Fixture.QueryAsAsync<GetGroupRosterQuery, GroupRosterDto>(actor, new GetGroupRosterQuery(groupId));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }
```

The excerpt stops at line 90; use the original file for the remaining 28 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
