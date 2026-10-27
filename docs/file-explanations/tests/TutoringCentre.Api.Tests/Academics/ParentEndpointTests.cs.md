# `tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 39.9: proves the four Parent endpoints' behaviour through the full HTTP pipeline, against the Day 39 contract's outcome table.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 268 lines and 12,858 bytes. Source hash: `ec7e39383a04bc48dcd00344a436c578ba8fd83bee41dab0fe104f664e99aad0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L10): `public sealed class ParentEndpointTests(ApiFactory factory) : IAsyncLifetime`
- [Line 15](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L15): `public async Task InitializeAsync()`
- [Line 21](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L21): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 24](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L24): `public async Task Create_ThenLookupWithDifferentSpelling_ThenGet_ThenUpdate_HappyPath()`
- [Line 61](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L61): `public async Task Create_DuplicatePhone_Returns409WithErrorsPhone()`
- [Line 79](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L79): `public async Task Create_InvalidPhone_Returns400PhoneInvalidWithErrorsPhone()`
- [Line 93](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L93): `public async Task Create_OneCharacterName_Returns400WithErrorsFullName()`
- [Line 106](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L106): `public async Task Create_EmptyPhone_Returns400WithErrorsPhone()`
- [Line 119](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L119): `public async Task Create_WithExtraCentreIdField_Returns400RequestMalformedAndCreatesNoRow()`
- [Line 134](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L134): `public async Task Update_StaleVersion_Returns409ConcurrencyStale()`
- [Line 154](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L154): `public async Task Update_UnknownId_Returns404ParentNotFound()`
- [Line 168](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L168): `public async Task Get_UnknownId_Returns404ParentNotFound()`
- [Line 180](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L180): `public async Task Lookup_InvalidPhone_Returns400PhoneInvalidWithErrorsPhone()`
- [Line 193](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L193): `public async Task Lookup_NoSuchParent_Returns200WithNullParent()`
- [Line 205](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L205): `public async Task Get_NoCookie_Returns401()`
- [Line 218](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L218): `public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()`
- [Line 230](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L230): `public async Task Lookup_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()`
- [Line 242](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L242): `public async Task Create_TwoCentreTeacherBeforeSelectingACentre_Returns403TenantNotSelected()`
- [Line 263](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L263): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

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

/// <summary>Task 39.9: proves the four Parent endpoints' behaviour through the full HTTP pipeline, against the Day 39 contract's outcome table.</summary>
[Collection(ApiCollection.Name)]
public sealed class ParentEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri ParentsUri = new("/api/parents", UriKind.Relative);
    private static readonly Uri LookupUri = new("/api/parents/lookup", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_ThenLookupWithDifferentSpelling_ThenGet_ThenUpdate_HappyPath()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, ParentsUri, new { fullName = "Omar Hassan", phone = "01012345678" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location;
        Assert.NotNull(location);
        var createBody = await ReadJsonAsync(createResponse);
        var id = createBody.RootElement.GetProperty("id").GetGuid();

        using var lookupResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, LookupUri, new { phone = "0101 234 5678" });
        var lookupBody = await ReadJsonAsync(lookupResponse);
        Assert.Equal(HttpStatusCode.OK, lookupResponse.StatusCode);
        Assert.Equal("no-store", lookupResponse.Headers.CacheControl?.ToString());
        Assert.Equal(id, lookupBody.RootElement.GetProperty("parent").GetProperty("id").GetGuid());

        using var getResponse = await nileOwner.Client.GetAsync(location);
        var getBody = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("Omar Hassan", getBody.RootElement.GetProperty("fullName").GetString());
        Assert.Equal("+201012345678", getBody.RootElement.GetProperty("phone").GetString());
        Assert.Empty(getBody.RootElement.GetProperty("children").EnumerateArray());
        var version = getBody.RootElement.GetProperty("version").GetUInt32();

        using var updateResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            nileOwner.Client, HttpMethod.Put, location, new { fullName = "Omar Hassan Ali", phone = "+201012345678", version });
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        using var getAfterUpdate = await nileOwner.Client.GetAsync(location);
        var afterBody = await ReadJsonAsync(getAfterUpdate);
        Assert.Equal("Omar Hassan Ali", afterBody.RootElement.GetProperty("fullName").GetString());
    }

    [Fact]
    public async Task Create_DuplicatePhone_Returns409WithErrorsPhone()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using (var first = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, ParentsUri, new { fullName = "Omar Hassan", phone = "01012345678" }))
        {
            Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        }

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, ParentsUri, new { fullName = "Another Parent", phone = "0101 234 5678" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("parent.phone_taken", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("phone").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_InvalidPhone_Returns400PhoneInvalidWithErrorsPhone()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, ParentsUri, new { fullName = "Omar Hassan", phone = "12345" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("phone.invalid", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("phone").GetArrayLength() > 0);
    }
```

The excerpt stops at line 90; use the original file for the remaining 178 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
