using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 49.8, the schedule's Week 8 demo through the whole HTTP pipeline: one seat left, ten parallel requests,
/// one success. Day 48's LastSeatConcurrencyTests proved the use case; this proves nothing between the client and
/// the handler (a shared scope, a retry, a cached count) breaks the guarantee. Ten clients, each with its own
/// cookies and its own antiforgery token fetched before the race (five signed in as the owner, five as the
/// secretary), send at the same instant behind one gate. Repeated with a fresh group each time, because a race
/// test that runs once can pass by luck. Counts are read through the superuser connection.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class LastSeatHttpTests(ApiFactory factory) : IAsyncLifetime
{
    private const int Repetitions = 5;
    private const int Contenders = 10;
    private const int Capacity = 20;

    private static int _nextPhoneSuffix = 60000000;

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task TenParallelEnrollRequestsIntoTheLastSeat_ExactlyOne201AndNine409GroupFull_AcrossFiveRepetitions()
    {
        using var arranger = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var subjectId = await CreateAsync(arranger.Client, "/api/subjects", new { name = "Last Seat Mathematics" });
        var teacherId = await CreateAsync(arranger.Client, "/api/teachers", new { fullName = "Last Seat Teacher", phone = (string?)null });

        var clients = new List<SignedInClient>();
        try
        {
            for (var i = 0; i < Contenders / 2; i++)
            {
                clients.Add(await SignInHelper.SignInAsNileOwnerAsync(factory));
                clients.Add(await SignInHelper.SignInAsNileSecretaryAsync(factory));
            }

            for (var iteration = 0; iteration < Repetitions; iteration++)
            {
                await RunScenarioAsync(arranger.Client, clients, subjectId, teacherId, iteration);
            }
        }
        finally
        {
            clients.ForEach(client => client.Dispose());
        }
    }

    private async Task RunScenarioAsync(HttpClient arranger, List<SignedInClient> clients, Guid subjectId, Guid teacherId, int iteration)
    {
        // A fresh group per repetition with 19 of its 20 seats taken, so no seat from one repetition counts toward the next.
        var groupId = await CreateAsync(
            arranger, "/api/groups", new { name = $"Last Seat {iteration:D2}", subjectId, teacherId, capacity = Capacity });
        for (var i = 0; i < Capacity - 1; i++)
        {
            var seatedId = await RegisterStudentAsync(arranger, $"Seated {iteration:D2}-{i:D2}");
            await CreateAsync(arranger, $"/api/groups/{groupId}/enrollments", new { studentId = seatedId });
        }

        var contenderIds = new List<Guid>();
        for (var i = 0; i < Contenders; i++)
        {
            contenderIds.Add(await RegisterStudentAsync(arranger, $"Contender {iteration:D2}-{i:D2}"));
        }

        // Every request is fully built, with its own client's own token, before the gate opens: nothing left to do
        // before sending but wait for the gate.
        var requests = new List<(HttpClient Client, HttpRequestMessage Request)>();
        for (var i = 0; i < Contenders; i++)
        {
            var client = clients[i].Client;
            var token = await AntiforgeryTestHelper.GetCsrfTokenAsync(client);
            var request = new HttpRequestMessage(HttpMethod.Post, new Uri($"/api/groups/{groupId}/enrollments", UriKind.Relative))
            {
                Content = JsonContent.Create(new { studentId = contenderIds[i] }),
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
            var text = await response.Content.ReadAsStringAsync();
            return new Attempt(response.StatusCode, text, startedAt, finishedAt);
        })).ToList();

        gate.SetResult();
        var results = await Task.WhenAll(attempts);
        requests.ForEach(pair => pair.Request.Dispose());

        // Genuinely overlapping: the last request started before the first one finished.
        Assert.True(
            results.Max(result => result.StartedAt) < results.Min(result => result.FinishedAt),
            "The requests did not overlap in time; the race was not exercised.");

        Assert.All(results, result => Assert.True(
            result.Status is HttpStatusCode.Created or HttpStatusCode.Conflict, $"Unexpected {(int)result.Status}: {result.Body}"));
        var winner = Assert.Single(results, result => result.Status == HttpStatusCode.Created);
        var losers = results.Where(result => result.Status == HttpStatusCode.Conflict).ToList();
        Assert.Equal(Contenders - 1, losers.Count);
        Assert.All(losers, loser => Assert.Equal("group.full", Code(loser.Body)));

        using (var rosterResponse = await arranger.GetAsync(new Uri($"/api/groups/{groupId}/roster", UriKind.Relative)))
        {
            var roster = await ReadJsonAsync(rosterResponse);
            Assert.Equal(Capacity, roster.RootElement.GetProperty("items").GetArrayLength());
            Assert.Equal(Capacity, roster.RootElement.GetProperty("enrolledCount").GetInt32());
        }

        Assert.Equal(Capacity, await factory.ScalarAsync<long>(
            $"select count(*) from academics.enrollments where group_id = '{groupId}' and status = 'active'"));

        // Exactly one audit row for the winning enrollment; the nine losers rolled back, audit rows included.
        var winnerId = JsonDocument.Parse(winner.Body).RootElement.GetProperty("id").GetGuid();
        Assert.Equal(1, await factory.ScalarAsync<long>(
            $"select count(*) from audit.audit_entries where entity_type = 'enrollment' and entity_id = '{winnerId}'"));
        var contenderList = string.Join(",", contenderIds.Select(id => $"'{id}'"));
        Assert.Equal(1, await factory.ScalarAsync<long>(
            $"""
            select count(*) from audit.audit_entries
            where entity_type = 'enrollment' and changes->'studentId'->>'after' in ({contenderList})
            """));
    }

    private static Task<Guid> RegisterStudentAsync(HttpClient client, string fullName) =>
        CreateAsync(
            client, "/api/students",
            new
            {
                fullName, phone = (string?)null, schoolYear = (string?)null,
                parent = new { fullName = $"{fullName} Parent", phone = UniquePhone() }, relationship = "father",
            });

    private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)
    {
        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(client, new Uri(url, UriKind.Relative), body);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        return (await ReadJsonAsync(response)).RootElement.GetProperty("id").GetGuid();
    }

    private static string? Code(string body) => JsonDocument.Parse(body).RootElement.GetProperty("code").GetString();

    private static string UniquePhone() => "015" + Interlocked.Increment(ref _nextPhoneSuffix).ToString(CultureInfo.InvariantCulture);

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }

    private sealed record Attempt(HttpStatusCode Status, string Body, TimeSpan StartedAt, TimeSpan FinishedAt);
}
