using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Http;

[Collection(ForwardedHeadersCollection.Name)]
public sealed class ForwardedHeadersTests(ForwardedHeadersFactory factory)
{
    private const string ForwardedFor = "203.0.113.9";
    private readonly HttpClient _client = factory.CreateClient();
    private readonly ForwardedHeadersFactory _factory = factory;

    [Fact]
    public async Task FromATrustedProxy_TheForwardedAddressIsLogged()
    {
        using var request = NewRequest("forwarded-trusted-check", ForwardedHeadersFactory.TrustedDirectAddress);

        await _client.SendAsync(request);

        var logged = await _factory.Logs.WaitForAsync(e => InMemoryLogSink.Text(e, "CorrelationId") == "forwarded-trusted-check");
        Assert.NotNull(logged);
        Assert.Equal(ForwardedFor, InMemoryLogSink.Text(logged, "ClientIp"));
    }

    [Fact]
    public async Task FromAnUntrustedSource_TheForwardedAddressIsIgnored()
    {
        using var request = NewRequest("forwarded-untrusted-check", ForwardedHeadersFactory.UntrustedDirectAddress);

        await _client.SendAsync(request);

        var logged = await _factory.Logs.WaitForAsync(e => InMemoryLogSink.Text(e, "CorrelationId") == "forwarded-untrusted-check");
        Assert.NotNull(logged);
        Assert.Equal(ForwardedHeadersFactory.UntrustedDirectAddress, InMemoryLogSink.Text(logged, "ClientIp"));
    }

    private static HttpRequestMessage NewRequest(string correlationId, string simulatedDirectAddress)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/system/info", UriKind.Relative));
        request.Headers.Add("X-Correlation-Id", correlationId);
        request.Headers.Add("X-Forwarded-For", ForwardedFor);
        request.Headers.Add(SimulatedDirectConnectionStartupFilter.HeaderName, simulatedDirectAddress);
        return request;
    }
}
