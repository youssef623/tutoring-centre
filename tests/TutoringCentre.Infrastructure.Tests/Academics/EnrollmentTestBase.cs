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

    private protected async Task<StudentEnrollmentDto> GetStudentEnrollmentAsync(Actor actor, Guid studentId, Guid enrollmentId)
    {
        var result = await Fixture.QueryAsAsync<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>(
            actor, new ListStudentEnrollmentsQuery(studentId));
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value.Single(enrollment => enrollment.EnrollmentId == enrollmentId);
    }

    /// <summary>
    /// Starts every attempt on its own thread pool task, each dispatching in its own DI scope and so on its own
    /// connection (DispatchExtensions), held at one gate so they reach the database together, and returns every
    /// result. Never shares a scope across tasks.
    /// </summary>
    private protected static async Task<T[]> RunTogetherAsync<T>(IEnumerable<Func<Task<T>>> attempts)
    {
        var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var running = attempts
            .Select(attempt => Task.Run(async () =>
            {
                await gate.Task;
                return await attempt();
            }))
            .ToArray();
        gate.SetResult();
        return await Task.WhenAll(running);
    }
}
