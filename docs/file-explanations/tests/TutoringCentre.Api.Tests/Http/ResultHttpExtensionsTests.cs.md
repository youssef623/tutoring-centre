# `tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs`

## Purpose

Declares `ResultHttpExtensionsTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 183 lines and 8,557 bytes. Source hash: `a1924721d500e576da7ed9a180020f41a5d91364438aff51ce992637228a5149`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Text.Json;`
- Line 2: `using Microsoft.AspNetCore.Http;`
- Line 3: `using Microsoft.Extensions.DependencyInjection;`
- Line 4: `using TutoringCentre.Api.Http;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L9): `public sealed class ResultHttpExtensionsTests`
- [Line 18](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L18): `public async Task ToHttpResult_FailureKind_MapsStatusAndCode(ErrorKind kind, int expectedStatus, string code)`
- [Line 33](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L33): `public async Task ToHttpResult_ValidationWithFields_IncludesErrorsObject()`
- [Line 47](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L47): `public async Task ToHttpResult_ConflictWithFields_IncludesErrorsObjectToo()`
- [Line 64](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L64): `public async Task ToHttpResult_Failure_IncludesTraceIdAndCorrelationId()`
- [Line 76](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L76): `public async Task ToHttpResult_Success_ReturnsWhatOnSuccessProduces()`
- [Line 87](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L87): `public async Task ToHttpResult_NonGenericSuccess_Returns204()`
- [Line 96](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L96): `public async Task ToHttpResult_NonGenericFailure_Returns409WithCode()`
- [Line 106](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L106): `public async Task ToCreatedHttpResult_Success_Returns201WithLocationAndBody()`
- [Line 125](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L125): `public async Task ToCreatedHttpResult_FailureKind_MapsToTheSameStatusAsTheExistingMapping(ErrorKind kind, int expectedStatus, string code)`
- [Line 137](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L137): `public async Task ToCreatedHttpResult_NoLocationVariant_Returns201WithBodyAndNoLocationHeader()`
- [Line 156](../../../../../tests/TutoringCentre.Api.Tests/Http/ResultHttpExtensionsTests.cs#L156): `public async Task ToCreatedHttpResult_NoLocationVariant_FailureKind_MapsToTheSameStatusAsTheExistingMapping(`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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

```

The excerpt stops at line 90; use the original file for the remaining 93 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
