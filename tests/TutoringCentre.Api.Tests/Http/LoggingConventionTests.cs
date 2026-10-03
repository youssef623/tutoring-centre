using Serilog.Events;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Http;

[Collection(ConventionsCollection.Name)]
public sealed class LoggingConventionTests(ConventionsFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();
    private readonly ConventionsFactory _factory = factory;

    [Fact]
    public async Task SuccessfulRequest_LogsOneCompletionLineAtInformation()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/test/ok", UriKind.Relative));
        request.Headers.Add("X-Correlation-Id", "logging-ok-check");

        await _client.SendAsync(request);

        var logged = await _factory.Logs.WaitForAsync(e =>
            InMemoryLogSink.Text(e, "CorrelationId") == "logging-ok-check" && e.Level == LogEventLevel.Information);
        Assert.NotNull(logged);
    }

    [Fact]
    public async Task UnhandledException_LogsAtErrorLevel()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/test/throws", UriKind.Relative));
        request.Headers.Add("X-Correlation-Id", "logging-error-check");

        await _client.SendAsync(request);

        var logged = await _factory.Logs.WaitForAsync(e =>
            InMemoryLogSink.Text(e, "CorrelationId") == "logging-error-check" && e.Level == LogEventLevel.Error);
        Assert.NotNull(logged);
    }
}
