# `tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 49.8, the schedule's Week 8 demo through the whole HTTP pipeline: one seat left, ten parallel requests, one success. Day 48's LastSeatConcurrencyTests proved the use case; this proves nothing between the client and the handler (a shared scope, a retry, a cached count) breaks the guarantee. Ten clients, each with its own cookies and its own antiforgery token fetched before the race (five signed in as the owner, five as the secretary), send at the same instant behind one gate. Repeated with a fresh group each time, because a race test that runs once can pass by luck. Counts are read through the superuser connection.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 173 lines and 8,431 bytes. Source hash: `546a4cdf6114685e29a72b4d6153c367345ce2722d2a7cb0f81bb175a897fd18`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics;`
- Line 2: `using System.Globalization;`
- Line 3: `using System.Net;`
- Line 4: `using System.Net.Http.Json;`
- Line 5: `using System.Text.Json;`
- Line 6: `using TutoringCentre.Api.Auth;`
- Line 7: `using TutoringCentre.Api.Cli;`
- Line 8: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 21](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L21): `public sealed class LastSeatHttpTests(ApiFactory factory) : IAsyncLifetime`
- [Line 29](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L29): `public async Task InitializeAsync()`
- [Line 35](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L35): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 38](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L38): `public async Task TenParallelEnrollRequestsIntoTheLastSeat_ExactlyOne201AndNine409GroupFull_AcrossFiveRepetitions()`
- [Line 64](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L64): `private async Task RunScenarioAsync(HttpClient arranger, List<SignedInClient> clients, Guid subjectId, Guid teacherId, int iteration)`
- [Line 146](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L146): `private static Task<Guid> RegisterStudentAsync(HttpClient client, string fullName) =>`
- [Line 155](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L155): `private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)`
- [Line 162](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L162): `private static string? Code(string body) => JsonDocument.Parse(body).RootElement.GetProperty("code").GetString();`
- [Line 164](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L164): `private static string UniquePhone() => "015" + Interlocked.Increment(ref _nextPhoneSuffix).ToString(CultureInfo.InvariantCulture);`
- [Line 166](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L166): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`
- [Line 172](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L172): `private sealed record Attempt(HttpStatusCode Status, string Body, TimeSpan StartedAt, TimeSpan FinishedAt);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 83 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
