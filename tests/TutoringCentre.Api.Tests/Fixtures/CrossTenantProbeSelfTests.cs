using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>
/// Task 26.8: a probe that cannot fail proves nothing. This proves CrossTenantProbe actually can — using a real
/// subject as a convenient, already-seeded resource (the helper itself still knows nothing about subjects).
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class CrossTenantProbeSelfTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task RunAsync_AttackerIsActuallyAMemberOfTheSameCentre_FailsInsteadOfPassingSilently()
    {
        // Not a real attacker: a legitimate Nile owner probing a Nile subject. The probe is told to expect the
        // isolation outcome (404) anyway — proving that when isolation does NOT hold (here, correctly: nothing
        // is actually broken), the helper reports a failure rather than silently agreeing with whatever happened.
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var subjectId = await GetAnyNileSubjectIdAsync(nileOwner.Client);

        var exception = await Record.ExceptionAsync(() => CrossTenantProbe.RunAsync(
            nileOwner.Client,
            HttpMethod.Get,
            $"/api/subjects/{subjectId}",
            body: null,
            expectedStatus: HttpStatusCode.NotFound,
            expectedCode: "subject.not_found",
            snapshotVictimRowAsync: () => Task.FromResult(string.Empty)));

        Assert.NotNull(exception);
    }

    private static async Task<Guid> GetAnyNileSubjectIdAsync(HttpClient nileOwnerClient)
    {
        using var response = await nileOwnerClient.GetAsync(new Uri("/api/subjects", UriKind.Relative));
        var text = await response.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(text);
        return json.RootElement.GetProperty("items")[0].GetProperty("id").GetGuid();
    }
}
