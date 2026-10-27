# `tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs`

## Purpose

Source summary (intent, not independent execution proof): The login rate limiter already partitions by the connection's remote address (Task 34.4's own requirement: change it only if it reads anything else — it does not), so once Task 34.3's forwarded-header processing is wired up, these prove the limiter is automatically keyed by the real client behind the trusted proxy. Deliberately bypasses `AntiforgeryTestHelper`'s per-call random partition header: these tests need the real partitioning logic, not the testing-only escape hatch.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 75 lines and 3,973 bytes. Source hash: `2b9dcacdafa54752575396d70fc98f92ef1169bee3d687031af0ed9b8bc75cf1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Net.Http.Json;`
- Line 3: `using Microsoft.AspNetCore.Mvc.Testing;`
- Line 4: `using TutoringCentre.Api.Auth;`
- Line 5: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs#L17): `public sealed class ForwardedRateLimitTests(ForwardedHeadersFactory factory)`
- [Line 22](../../../../../tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs#L22): `public async Task FromATrustedProxy_TheRateLimitPartitionsByTheForwardedAddress()`
- [Line 41](../../../../../tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs#L41): `public async Task FromAnUntrustedSource_ChangingTheForwardedHeaderDoesNotResetTheBudget()`
- [Line 57](../../../../../tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs#L57): `private static async Task<HttpResponseMessage> LoginAsync(HttpClient client, string forwardedFor, string simulatedDirectAddress)`
- [Line 74](../../../../../tests/TutoringCentre.Api.Tests/Auth/ForwardedRateLimitTests.cs#L74): `private sealed record TokenResponse(string Token);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
