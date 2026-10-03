using System.Net;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Http;

[Collection(ConventionsCollection.Name)]
public sealed class CorrelationIdTests(ConventionsFactory factory)
{
    private const string HeaderName = "X-Correlation-Id";

    private readonly HttpClient _client = factory.CreateClient();
    private readonly ConventionsFactory _factory = factory;

    [Fact]
    public async Task NoHeaderSupplied_GeneratesCorrelationId()
    {
        var response = await _client.GetAsync(new Uri("/health", UriKind.Relative));

        Assert.True(response.Headers.TryGetValues(HeaderName, out var values));
        Assert.Equal(32, values!.Single().Length);
    }

    [Fact]
    public async Task ValidHeaderSupplied_IsEchoedBack()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/health", UriKind.Relative));
        request.Headers.Add(HeaderName, "client-supplied-id");

        var response = await _client.SendAsync(request);

        Assert.Equal("client-supplied-id", response.Headers.GetValues(HeaderName).Single());
    }

    [Fact]
    public async Task InvalidHeaderTooLong_IsReplaced()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/health", UriKind.Relative));
        request.Headers.Add(HeaderName, new string('a', 65));

        var response = await _client.SendAsync(request);

        Assert.Equal(32, response.Headers.GetValues(HeaderName).Single().Length);
    }

    [Fact]
    public async Task InvalidHeaderBadCharacters_IsReplaced()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/health", UriKind.Relative));
        request.Headers.Add(HeaderName, "bad id!");

        var response = await _client.SendAsync(request);

        Assert.NotEqual("bad id!", response.Headers.GetValues(HeaderName).Single());
    }

    [Fact]
    public async Task CorrelationId_MatchesBetweenResponseHeaderAndProblemBody()
    {
        var response = await _client.GetAsync(new Uri("/api/test/notfound", UriKind.Relative));

        var header = response.Headers.GetValues(HeaderName).Single();
        var text = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Contains($"\"correlationId\":\"{header}\"", text, StringComparison.Ordinal);
    }

    [Fact]
    public async Task CorrelationId_AppearsInRequestLogLine()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/test/ok", UriKind.Relative));
        request.Headers.Add(HeaderName, "log-check-id");

        await _client.SendAsync(request);

        var logged = await _factory.Logs.WaitForAsync(e => InMemoryLogSink.Text(e, "CorrelationId") == "log-check-id");
        Assert.NotNull(logged);
    }
}
