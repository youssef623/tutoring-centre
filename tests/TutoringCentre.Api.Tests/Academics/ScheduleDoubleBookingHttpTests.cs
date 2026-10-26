using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 53.4: the double-booking guarantee through the whole HTTP pipeline. Two staff add colliding weekly rules
/// for the same teacher on two of that teacher's groups at the same instant; the exclusion constraint lets exactly
/// one in, and the other's rule and sessions roll back with it. Each request has its own client, cookies and
/// antiforgery token, built before the gate opens. Repeated with fresh groups, because a race test that runs once
/// can pass by luck. Counts are read through the superuser connection. The clock is fixed, so the winner's rule
/// always generates exactly four sessions.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class ScheduleDoubleBookingHttpTests(ApiFactory factory) : IAsyncLifetime
{
    private const int Repetitions = 5;

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
        factory.Time.FixAt(new DateTimeOffset(2027, 4, 20, 8, 0, 0, TimeSpan.Zero));
    }

    public Task DisposeAsync()
    {
        factory.Time.UseRealTime();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task TwoParallelCollidingRulesForOneTeacher_One201AndOne409TeacherDoubleBooked_ExactlyFourSessions()
    {
        using var arranger = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var secretary = await SignInHelper.SignInAsNileSecretaryAsync(factory);
        var subjectId = await CreateAsync(arranger.Client, "/api/subjects", new { name = "Double Booking Physics" });

        for (var iteration = 0; iteration < Repetitions; iteration++)
        {
            // A fresh teacher per repetition, so one repetition's sessions never collide with the next one's.
            var teacherId = await CreateAsync(arranger.Client, "/api/teachers", new { fullName = $"Double Booking Teacher {iteration}", phone = (string?)null });
            var firstGroupId = await CreateAsync(arranger.Client, "/api/groups", new { name = $"Double A {iteration}", subjectId, teacherId, capacity = 20 });
            var secondGroupId = await CreateAsync(arranger.Client, "/api/groups", new { name = $"Double B {iteration}", subjectId, teacherId, capacity = 20 });

            var requests = new List<(HttpClient Client, HttpRequestMessage Request)>();
            foreach (var (client, groupId, startTime) in new[] { (owner.Client, firstGroupId, "18:00"), (secretary.Client, secondGroupId, "18:30") })
            {
                var token = await AntiforgeryTestHelper.GetCsrfTokenAsync(client);
                var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"/api/groups/{groupId}/schedule-rules", UriKind.Relative))
                {
                    Content = JsonContent.Create(new { dayOfWeek = "saturday", startTime, durationMinutes = 90, room = (string?)null, effectiveFrom = "2027-04-20" }),
                };
                request.Headers.Add(AntiforgerySetup.HeaderName, token);
                requests.Add((client, request));
            }

            var gate = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var clock = Stopwatch.StartNew();
            var attempts = requests.Select(pair => Task.Run(async () =>
            {
                await gate.Task;
                var startedAt = clock.Elapsed;
                using var response = await pair.Client.SendAsync(pair.Request);
                var finishedAt = clock.Elapsed;
                return new Attempt(response.StatusCode, await response.Content.ReadAsStringAsync(), startedAt, finishedAt);
            })).ToList();

            gate.SetResult();
            var results = await Task.WhenAll(attempts);
            requests.ForEach(pair => pair.Request.Dispose());

            Assert.True(
                results.Max(result => result.StartedAt) < results.Min(result => result.FinishedAt),
                "The requests did not overlap in time; the race was not exercised.");
            var winner = Assert.Single(results, result => result.Status == HttpStatusCode.Created);
            var loser = Assert.Single(results, result => result.Status != HttpStatusCode.Created);
            Assert.True(loser.Status == HttpStatusCode.Conflict, $"Iteration {iteration}: unexpected {(int)loser.Status}: {loser.Body}");
            Assert.Equal("schedule.teacher_double_booked", JsonDocument.Parse(loser.Body).RootElement.GetProperty("code").GetString());
            Assert.Equal(4, JsonDocument.Parse(winner.Body).RootElement.GetProperty("sessionsCreated").GetInt32());

            Assert.Equal(1, await factory.ScalarAsync<long>(
                $"select count(*) from academics.schedule_rules where group_id in ('{firstGroupId}', '{secondGroupId}')"));
            Assert.Equal(4, await factory.ScalarAsync<long>(
                $"select count(*) from academics.class_sessions where teacher_id = '{teacherId}'"));
        }
    }

    private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri(url, UriKind.Relative), body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text).RootElement.GetProperty("id").GetGuid();
    }

    private sealed record Attempt(HttpStatusCode Status, string Body, TimeSpan StartedAt, TimeSpan FinishedAt);
}
