# `tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs`

## Purpose

Declares `ProblemDetailsTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 211 lines and 8,188 bytes. Source hash: `57ee9d8af3da1fa088f0dfb0fb725d5f689217968e97cb96d91238a3f219ee1f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Net.Http.Json;`
- Line 3: `using System.Text.Json;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L9): `public sealed class ProblemDetailsTests(ConventionsFactory factory)`
- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L14): `public async Task Ok_ReturnsSuccessBody()`
- [Line 22](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L22): `public async Task Validation_Returns400WithProblemJsonAndCode()`
- [Line 32](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L32): `public async Task NotFound_Returns404WithCode()`
- [Line 42](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L42): `public async Task Conflict_Returns409WithCode()`
- [Line 52](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L52): `public async Task Rule_Returns422WithCode()`
- [Line 62](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L62): `public async Task Forbidden_Returns403WithCode()`
- [Line 72](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L72): `public async Task UnhandledException_Returns500WithoutLeakingInternals()`
- [Line 85](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L85): `public async Task MalformedJsonBody_Returns400RequestMalformed()`
- [Line 96](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L96): `public async Task UnknownJsonField_Returns400RequestMalformed()`
- [Line 106](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L106): `public async Task EmptyName_FailsValidation_Returns400()`
- [Line 115](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L115): `public async Task ValidName_ReturnsGreeting()`
- [Line 125](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L125): `public async Task UnknownApiRoute_Returns404RouteNotFound()`
- [Line 135](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L135): `public async Task UnknownApiRoute_PostMethod_Returns404RouteNotFound()`
- [Line 145](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L145): `public async Task UnknownNonApiRoute_Returns404ProblemJson()`
- [Line 157](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L157): `public async Task ErrorResponses_IncludeTraceId(string path)`
- [Line 166](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L166): `public async Task ErrorResponse_IncludesCorrelationIdMatchingRequestHeader()`
- [Line 178](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L178): `public async Task ErrorResponses_ContentTypeIsProblemJson()`
- [Line 186](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L186): `public async Task ValidationError_IncludesFieldErrorsDictionary()`
- [Line 197](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L197): `public async Task NonValidationError_DoesNotIncludeErrorsDictionary()`
- [Line 205](../../../../../tests/TutoringCentre.Api.Tests/Http/ProblemDetailsTests.cs#L205): `private static async Task<JsonDocument> ReadProblemAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

The excerpt stops at line 90; use the original file for the remaining 121 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
