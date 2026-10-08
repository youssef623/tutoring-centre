using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Api.Http;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Api.Tests.Http;

public sealed class ResultHttpExtensionsTests
{
    [Theory]
    [InlineData(ErrorKind.NotFound, 404, "test.not_found")]
    [InlineData(ErrorKind.Conflict, 409, "test.conflict")]
    [InlineData(ErrorKind.Rule, 422, "test.rule")]
    [InlineData(ErrorKind.Forbidden, 403, "test.forbidden")]
    [InlineData(ErrorKind.Validation, 400, "test.validation")]
    [InlineData(ErrorKind.Unauthenticated, 401, "test.unauthenticated")]
    public async Task ToHttpResult_FailureKind_MapsStatusAndCode(ErrorKind kind, int expectedStatus, string code)
    {
        var result = Result<string>.Failure(new Error(code, "Authored message.", kind));

        var response = await ExecuteAsync(result.ToHttpResult(value => Results.Ok(value)));

        using var body = JsonDocument.Parse(response.Body);
        Assert.Equal(expectedStatus, response.Status);
        Assert.Equal("application/problem+json", response.ContentType);
        Assert.Equal(code, body.RootElement.GetProperty("code").GetString());
        Assert.Equal("Authored message.", body.RootElement.GetProperty("detail").GetString());
        Assert.Equal(expectedStatus, body.RootElement.GetProperty("status").GetInt32());
    }

    [Fact]
    public async Task ToHttpResult_ValidationWithFields_IncludesErrorsObject()
    {
        var fields = new Dictionary<string, string[]> { ["name"] = ["Name is required."] };
        var result = Result<string>.Failure(Error.Validation("validation.failed", "One or more fields are invalid.", fields));

        var response = await ExecuteAsync(result.ToHttpResult(value => Results.Ok(value)));

        using var body = JsonDocument.Parse(response.Body);
        Assert.Equal(400, response.Status);
        Assert.Equal("validation.failed", body.RootElement.GetProperty("code").GetString());
        Assert.Equal("Name is required.", body.RootElement.GetProperty("errors").GetProperty("name")[0].GetString());
    }

    [Fact]
    public async Task ToHttpResult_ConflictWithFields_IncludesErrorsObjectToo()
    {
        // Day 26: a 409 duplicate-name conflict (subject.name_taken) carries Fields just like a validation
        // failure does, and must surface the same "errors" object — not just ErrorKind.Validation.
        var fields = new Dictionary<string, string[]> { ["name"] = ["A subject with this name already exists."] };
        var result = Result<string>.Failure(new Error("subject.name_taken", "A subject with this name already exists.", ErrorKind.Conflict, fields));

        var response = await ExecuteAsync(result.ToHttpResult(value => Results.Ok(value)));

        using var body = JsonDocument.Parse(response.Body);
        Assert.Equal(409, response.Status);
        Assert.Equal("subject.name_taken", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(
            "A subject with this name already exists.", body.RootElement.GetProperty("errors").GetProperty("name")[0].GetString());
    }

    [Fact]
    public async Task ToHttpResult_Failure_IncludesTraceIdAndCorrelationId()
    {
        var result = Result<string>.Failure(Error.NotFound("test.not_found", "Missing."));

        var response = await ExecuteAsync(result.ToHttpResult(value => Results.Ok(value)));

        using var body = JsonDocument.Parse(response.Body);
        Assert.Equal("trace-1", body.RootElement.GetProperty("traceId").GetString());
        Assert.Equal("corr-1", body.RootElement.GetProperty("correlationId").GetString());
    }

    [Fact]
    public async Task ToHttpResult_Success_ReturnsWhatOnSuccessProduces()
    {
        var result = Result<string>.Success("hello");

        var response = await ExecuteAsync(result.ToHttpResult(value => Results.Ok(value)));

        Assert.Equal(200, response.Status);
        Assert.Equal("\"hello\"", response.Body);
    }

    [Fact]
    public async Task ToHttpResult_NonGenericSuccess_Returns204()
    {
        var response = await ExecuteAsync(Result.Success().ToHttpResult());

        Assert.Equal(204, response.Status);
        Assert.Equal(string.Empty, response.Body);
    }

    [Fact]
    public async Task ToHttpResult_NonGenericFailure_Returns409WithCode()
    {
        var response = await ExecuteAsync(Result.Failure(Error.Conflict("test.conflict", "Conflict.")).ToHttpResult());

        using var body = JsonDocument.Parse(response.Body);
        Assert.Equal(409, response.Status);
        Assert.Equal("test.conflict", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task ToCreatedHttpResult_Success_Returns201WithLocationAndBody()
    {
        var result = Result<Guid>.Success(Guid.Parse("11111111-1111-1111-1111-111111111111"));

        var response = await ExecuteAsync(result.ToCreatedHttpResult(id => ($"/api/widgets/{id}", new { id })));

        using var body = JsonDocument.Parse(response.Body);
        Assert.Equal(201, response.Status);
        Assert.Equal("/api/widgets/11111111-1111-1111-1111-111111111111", response.Location);
        Assert.Equal("11111111-1111-1111-1111-111111111111", body.RootElement.GetProperty("id").GetString());
    }

    [Theory]
    [InlineData(ErrorKind.NotFound, 404, "test.not_found")]
    [InlineData(ErrorKind.Conflict, 409, "test.conflict")]
    [InlineData(ErrorKind.Rule, 422, "test.rule")]
    [InlineData(ErrorKind.Forbidden, 403, "test.forbidden")]
    [InlineData(ErrorKind.Validation, 400, "test.validation")]
    [InlineData(ErrorKind.Unauthenticated, 401, "test.unauthenticated")]
    public async Task ToCreatedHttpResult_FailureKind_MapsToTheSameStatusAsTheExistingMapping(ErrorKind kind, int expectedStatus, string code)
    {
        var result = Result<Guid>.Failure(new Error(code, "Authored message.", kind));

        var response = await ExecuteAsync(result.ToCreatedHttpResult(id => ($"/api/widgets/{id}", new { id })));

        using var body = JsonDocument.Parse(response.Body);
        Assert.Equal(expectedStatus, response.Status);
        Assert.Equal(code, body.RootElement.GetProperty("code").GetString());
    }

    private static async Task<(int Status, string? ContentType, string Body, string? Location)> ExecuteAsync(IResult result)
    {
        await using var services = new ServiceCollection().AddLogging().AddOptions().BuildServiceProvider();
        var context = new DefaultHttpContext { RequestServices = services, TraceIdentifier = "trace-1" };
        context.Items["CorrelationId"] = "corr-1";
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        await result.ExecuteAsync(context);

        buffer.Position = 0;
        using var reader = new StreamReader(buffer);
        var text = await reader.ReadToEndAsync();
        return (context.Response.StatusCode, context.Response.ContentType, text, context.Response.Headers.Location);
    }
}
