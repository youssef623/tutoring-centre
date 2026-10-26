using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TutoringCentre.Api.Auth;
using Xunit;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>
/// Attacks a victim's resource as a given (attacker) client and checks two things: the response itself, and
/// that the victim's row — read through a caller-supplied snapshot, taken over the owner/superuser connection —
/// is unchanged. Feature-agnostic: knows nothing about subjects or any other module, only HTTP and a snapshot
/// function. A probe that checked only the status would prove nothing; this is why the snapshot is required,
/// not optional.
/// </summary>
internal static class CrossTenantProbe
{
    public static async Task RunAsync(
        HttpClient attackerClient,
        HttpMethod method,
        string url,
        object? body,
        HttpStatusCode expectedStatus,
        string? expectedCode,
        Func<Task<string>> snapshotVictimRowAsync)
    {
        ArgumentNullException.ThrowIfNull(snapshotVictimRowAsync);

        var before = await snapshotVictimRowAsync();

        await SendAndAssertAsync(attackerClient, method, url, body, expectedStatus, expectedCode);

        var after = await snapshotVictimRowAsync();
        Assert.Equal(before, after);
    }

    /// <summary>
    /// A reference-attack probe: the URL and body are the ATTACKER's own (a resource they genuinely own), but
    /// one id inside them — a route segment or a body field — names the VICTIM's record instead. An ordinary
    /// <see cref="RunAsync"/> cannot express this: that helper attacks the victim's URL directly and would
    /// always see 404 regardless of whether the reference itself was checked. Both rows are snapshotted and
    /// re-checked, because a reference attack could just as easily corrupt the attacker's own row (e.g. by
    /// linking it to someone else's child record) as the victim's.
    /// </summary>
    public static async Task RunReferenceAsync(
        HttpClient attackerClient,
        HttpMethod method,
        string url,
        object? body,
        HttpStatusCode expectedStatus,
        string? expectedCode,
        Func<Task<string>> snapshotAttackerRowAsync,
        Func<Task<string>> snapshotVictimRowAsync)
    {
        ArgumentNullException.ThrowIfNull(snapshotAttackerRowAsync);
        ArgumentNullException.ThrowIfNull(snapshotVictimRowAsync);

        var attackerBefore = await snapshotAttackerRowAsync();
        var victimBefore = await snapshotVictimRowAsync();

        await SendAndAssertAsync(attackerClient, method, url, body, expectedStatus, expectedCode);

        Assert.Equal(attackerBefore, await snapshotAttackerRowAsync());
        Assert.Equal(victimBefore, await snapshotVictimRowAsync());
    }

    private static async Task SendAndAssertAsync(
        HttpClient attackerClient, HttpMethod method, string url, object? body, HttpStatusCode expectedStatus, string? expectedCode)
    {
        ArgumentNullException.ThrowIfNull(attackerClient);
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(url);

        using var request = new HttpRequestMessage(method, url);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        // Safe methods (GET) need no CSRF token; the real AntiforgeryEndpointFilter only checks unsafe ones.
        if (method != HttpMethod.Get)
        {
            var token = await AntiforgeryTestHelper.GetCsrfTokenAsync(attackerClient);
            request.Headers.Add(AntiforgerySetup.HeaderName, token);
        }

        using var response = await attackerClient.SendAsync(request);
        var text = await response.Content.ReadAsStringAsync();

        Assert.Equal(expectedStatus, response.StatusCode);

        if (expectedCode is not null)
        {
            using var json = JsonDocument.Parse(text);
            Assert.Equal(expectedCode, json.RootElement.GetProperty("code").GetString());
        }
    }
}
