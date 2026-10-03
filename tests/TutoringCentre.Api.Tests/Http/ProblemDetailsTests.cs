using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Http;

[Collection(ConventionsCollection.Name)]
public sealed class ProblemDetailsTests(ConventionsFactory factory)
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task Ok_ReturnsSuccessBody()
    {
        var response = await _client.GetAsync(new Uri("/api/test/ok", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Validation_Returns400WithProblemJsonAndCode()
    {
        var response = await _client.GetAsync(new Uri("/api/test/validation", UriKind.Relative));

        var body = await ReadProblemAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("test.invalid", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task NotFound_Returns404WithCode()
    {
        var response = await _client.GetAsync(new Uri("/api/test/notfound", UriKind.Relative));

        var body = await ReadProblemAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("test.not_found", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Conflict_Returns409WithCode()
    {
        var response = await _client.GetAsync(new Uri("/api/test/conflict", UriKind.Relative));

        var body = await ReadProblemAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("test.conflict", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Rule_Returns422WithCode()
    {
        var response = await _client.GetAsync(new Uri("/api/test/rule", UriKind.Relative));

        var body = await ReadProblemAsync(response);
        Assert.Equal((HttpStatusCode)422, response.StatusCode);
        Assert.Equal("test.rule", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Forbidden_Returns403WithCode()
    {
        var response = await _client.GetAsync(new Uri("/api/test/forbidden", UriKind.Relative));

        var body = await ReadProblemAsync(response);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("test.forbidden", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task UnhandledException_Returns500WithoutLeakingInternals()
    {
        var response = await _client.GetAsync(new Uri("/api/test/throws", UriKind.Relative));

        var text = await response.Content.ReadAsStringAsync();
        var body = JsonDocument.Parse(text);
        Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        Assert.Equal("server.unexpected", body.RootElement.GetProperty("code").GetString());
        Assert.DoesNotContain("hunter2", text, StringComparison.Ordinal);
        Assert.DoesNotContain("InvalidOperationException", text, StringComparison.Ordinal);
    }

    [Fact]
    public async Task MalformedJsonBody_Returns400RequestMalformed()
    {
        using var content = new StringContent("{ not valid json", System.Text.Encoding.UTF8, "application/json");
        var response = await _client.PostAsync(new Uri("/api/test/name", UriKind.Relative), content);

        var body = await ReadProblemAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task UnknownJsonField_Returns400RequestMalformed()
    {
        var response = await _client.PostAsJsonAsync(new Uri("/api/test/name", UriKind.Relative), new { name = "Bob", extra = "nope" });

        var body = await ReadProblemAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task EmptyName_FailsValidation_Returns400()
    {
        var response = await _client.PostAsJsonAsync(new Uri("/api/test/name", UriKind.Relative), new { name = string.Empty });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task ValidName_ReturnsGreeting()
    {
        var response = await _client.PostAsJsonAsync(new Uri("/api/test/name", UriKind.Relative), new { name = "Sara" });

        var text = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("\"Hello Sara\"", text);
    }

    [Fact]
    public async Task UnknownApiRoute_Returns404RouteNotFound()
    {
        var response = await _client.GetAsync(new Uri("/api/does-not-exist", UriKind.Relative));

        var body = await ReadProblemAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("route.not_found", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task UnknownApiRoute_PostMethod_Returns404RouteNotFound()
    {
        var response = await _client.PostAsync(new Uri("/api/does-not-exist", UriKind.Relative), content: null);

        var body = await ReadProblemAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("route.not_found", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task UnknownNonApiRoute_Returns404ProblemJson()
    {
        var response = await _client.GetAsync(new Uri("/does-not-exist", UriKind.Relative));

        await ReadProblemAsync(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [InlineData("/api/test/notfound")]
    [InlineData("/api/test/throws")]
    [InlineData("/does-not-exist")]
    public async Task ErrorResponses_IncludeTraceId(string path)
    {
        var response = await _client.GetAsync(new Uri(path, UriKind.Relative));

        var body = await ReadProblemAsync(response);
        Assert.False(string.IsNullOrEmpty(body.RootElement.GetProperty("traceId").GetString()));
    }

    [Fact]
    public async Task ErrorResponse_IncludesCorrelationIdMatchingRequestHeader()
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, new Uri("/api/test/notfound", UriKind.Relative));
        request.Headers.Add("X-Correlation-Id", "test-corr-1");

        var response = await _client.SendAsync(request);

        var body = await ReadProblemAsync(response);
        Assert.Equal("test-corr-1", body.RootElement.GetProperty("correlationId").GetString());
    }

    [Fact]
    public async Task ErrorResponses_ContentTypeIsProblemJson()
    {
        var response = await _client.GetAsync(new Uri("/api/test/conflict", UriKind.Relative));

        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task ValidationError_IncludesFieldErrorsDictionary()
    {
        var response = await _client.GetAsync(new Uri("/api/test/validation", UriKind.Relative));

        var body = await ReadProblemAsync(response);
        Assert.Equal(
            "Name is required.",
            body.RootElement.GetProperty("errors").GetProperty("name")[0].GetString());
    }

    [Fact]
    public async Task NonValidationError_DoesNotIncludeErrorsDictionary()
    {
        var response = await _client.GetAsync(new Uri("/api/test/notfound", UriKind.Relative));

        var body = await ReadProblemAsync(response);
        Assert.False(body.RootElement.TryGetProperty("errors", out _));
    }

    private static async Task<JsonDocument> ReadProblemAsync(HttpResponseMessage response)
    {
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
