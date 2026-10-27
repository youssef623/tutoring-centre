# `tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 26.9: proves the six Subject endpoints' behaviour through the full HTTP pipeline, against the Day 26 contract.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 255 lines and 12,182 bytes. Source hash: `8e083dbfe76d879b62804a04a980cd8c5ed2f7f38994e9a0cc0e02c3f4f8836c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L10): `public sealed class SubjectEndpointTests(ApiFactory factory) : IAsyncLifetime`
- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L14): `public async Task InitializeAsync()`
- [Line 20](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L20): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 23](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L23): `public async Task List_ReturnsOnlyTheCallersCentresSubjectsInNameOrder()`
- [Line 39](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L39): `public async Task Create_ThenGet_Returns201WithLocationThen200()`
- [Line 56](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L56): `public async Task Create_Duplicate_Returns409WithErrorsName()`
- [Line 75](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L75): `public async Task Create_EmptyName_Returns400WithErrorsName(string name)`
- [Line 87](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L87): `public async Task Create_EightyOneCharacterName_Returns400WithErrorsName()`
- [Line 99](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L99): `public async Task Create_WithExtraCentreIdField_Returns400RequestMalformedAndCreatesNoRow()`
- [Line 113](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L113): `public async Task Rename_HappyPathThenStaleVersion()`
- [Line 130](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L130): `public async Task ArchiveThenRestore_AndTheirRuleViolations()`
- [Line 159](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L159): `public async Task List_NoCookie_Returns401()`
- [Line 172](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L172): `public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()`
- [Line 184](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L184): `public async Task List_TwoCentreTeacherBeforeSelectingACentre_Returns403TenantNotSelected()`
- [Line 197](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L197): `public async Task Get_NonGuidId_Returns404RouteNotFound()`
- [Line 209](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L209): `public async Task ErrorBody_AlwaysHasCodeTraceIdCorrelationIdAndNoStackTrace()`
- [Line 236](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L236): `private static async Task<uint> GetVersionAsync(HttpClient client, Guid id)`
- [Line 243](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L243): `private static async Task<int> CountSubjectsAsync(HttpClient client)`
- [Line 250](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L250): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>Task 26.9: proves the six Subject endpoints' behaviour through the full HTTP pipeline, against the Day 26 contract.</summary>
[Collection(ApiCollection.Name)]
public sealed class SubjectEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri SubjectsUri = new("/api/subjects", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task List_ReturnsOnlyTheCallersCentresSubjectsInNameOrder()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await nileOwner.Client.GetAsync(SubjectsUri);
        var json = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var names = json.RootElement.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("name").GetString()!).ToList();
        Assert.Equal(3, names.Count);
        Assert.DoesNotContain("Mathematics", names);
        Assert.DoesNotContain("Physics", names);
        Assert.Equal(names.OrderBy(name => name, StringComparer.Ordinal), names);
    }

    [Fact]
    public async Task Create_ThenGet_Returns201WithLocationThen200()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, SubjectsUri, new { name = "Geography" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location;
        Assert.NotNull(location);

        using var getResponse = await nileOwner.Client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("Geography", body.RootElement.GetProperty("name").GetString());
        Assert.Equal("active", body.RootElement.GetProperty("status").GetString());
    }

    [Fact]
    public async Task Create_Duplicate_Returns409WithErrorsName()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using (var first = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, SubjectsUri, new { name = "Geography" }))
        {
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        }

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, SubjectsUri, new { name = "geography" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("subject.name_taken", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("name").GetArrayLength() > 0);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_EmptyName_Returns400WithErrorsName(string name)
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, SubjectsUri, new { name });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("name").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_EightyOneCharacterName_Returns400WithErrorsName()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

```

The excerpt stops at line 90; use the original file for the remaining 165 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
