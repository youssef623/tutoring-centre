using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Auth;

/// <summary>
/// The login rate limiter already partitions by the connection's remote address (Task 34.4's own requirement:
/// change it only if it reads anything else — it does not), so once Task 34.3's forwarded-header processing is
/// wired up, these prove the limiter is automatically keyed by the real client behind the trusted proxy.
/// Deliberately bypasses <see cref="AntiforgeryTestHelper"/>'s per-call random partition header: these tests
/// need the real partitioning logic, not the testing-only escape hatch.
/// </summary>
[Collection(ForwardedHeadersCollection.Name)]
public sealed class ForwardedRateLimitTests(ForwardedHeadersFactory factory)
{
    private static readonly Uri LoginUri = new("/api/auth/login", UriKind.Relative);

    [Fact]
    public async Task FromATrustedProxy_TheRateLimitPartitionsByTheForwardedAddress()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        for (var attempt = 1; attempt <= 10; attempt++)
        {
            using var response = await LoginAsync(client, "203.0.113.1", ForwardedHeadersFactory.TrustedDirectAddress);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        using var eleventh = await LoginAsync(client, "203.0.113.1", ForwardedHeadersFactory.TrustedDirectAddress);
        Assert.Equal(HttpStatusCode.TooManyRequests, eleventh.StatusCode);

        // A different forwarded address, still behind the trusted proxy, has its own independent budget.
        using var twelfth = await LoginAsync(client, "203.0.113.2", ForwardedHeadersFactory.TrustedDirectAddress);
        Assert.NotEqual(HttpStatusCode.TooManyRequests, twelfth.StatusCode);
    }

    [Fact]
    public async Task FromAnUntrustedSource_ChangingTheForwardedHeaderDoesNotResetTheBudget()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });

        for (var attempt = 1; attempt <= 10; attempt++)
        {
            using var response = await LoginAsync(client, "203.0.113.1", ForwardedHeadersFactory.UntrustedDirectAddress);
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        // A forged X-Forwarded-For claiming a fresh address has no effect: the partition is the untrusted
        // direct connection's own address, which has already used its ten permits above.
        using var eleventh = await LoginAsync(client, "203.0.113.99", ForwardedHeadersFactory.UntrustedDirectAddress);
        Assert.Equal(HttpStatusCode.TooManyRequests, eleventh.StatusCode);
    }

    private static async Task<HttpResponseMessage> LoginAsync(HttpClient client, string forwardedFor, string simulatedDirectAddress)
    {
        using var tokenRequest = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/auth/antiforgery", UriKind.Relative));
        tokenRequest.Headers.Add(SimulatedDirectConnectionStartupFilter.HeaderName, simulatedDirectAddress);
        using var tokenResponse = await client.SendAsync(tokenRequest);
        var token = (await tokenResponse.Content.ReadFromJsonAsync<TokenResponse>())!.Token;

        using var request = new HttpRequestMessage(HttpMethod.Post, LoginUri)
        {
            Content = JsonContent.Create(new { email = "owner@nile.test", password = "wrong-password" }),
        };
        request.Headers.Add(AntiforgerySetup.HeaderName, token);
        request.Headers.Add("X-Forwarded-For", forwardedFor);
        request.Headers.Add(SimulatedDirectConnectionStartupFilter.HeaderName, simulatedDirectAddress);
        return await client.SendAsync(request);
    }

    private sealed record TokenResponse(string Token);
}
