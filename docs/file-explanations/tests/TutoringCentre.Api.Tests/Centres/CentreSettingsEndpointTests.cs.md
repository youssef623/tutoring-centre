# `tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 32.10: proves the GET/PUT /api/centre/settings contract (Day 32) through the full HTTP pipeline.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 151 lines and 6,740 bytes. Source hash: `5c98ed72a33cdc0314f44cb18925fab0bb2f352c740edd46587725c68889139c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L10): `public sealed class CentreSettingsEndpointTests(ApiFactory factory) : IAsyncLifetime`
- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L14): `public async Task InitializeAsync()`
- [Line 20](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L20): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 23](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L23): `public async Task Get_AsOwner_ReturnsTheContractShape()`
- [Line 39](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L39): `public async Task Put_HappyPath_ThenGetShowsTheNewNameAndVersion_AndMeShowsTheNewName()`
- [Line 64](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L64): `public async Task Put_BodyWithSlug_Returns400()`
- [Line 76](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L76): `public async Task Put_EmptyName_Returns400WithErrorsName()`
- [Line 90](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L90): `public async Task Get_AsSecretary_Returns403()`
- [Line 102](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L102): `public async Task Put_AsSecretary_Returns403()`
- [Line 115](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L115): `public async Task Get_AsTwoCentreTeacher_Returns403()`
- [Line 127](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L127): `public async Task Put_AsTwoCentreTeacher_Returns403()`
- [Line 139](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L139): `private static async Task<uint> GetVersionAsync(HttpClient client)`
- [Line 146](../../../../../tests/TutoringCentre.Api.Tests/Centres/CentreSettingsEndpointTests.cs#L146): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Centres;

/// <summary>Task 32.10: proves the GET/PUT /api/centre/settings contract (Day 32) through the full HTTP pipeline.</summary>
[Collection(ApiCollection.Name)]
public sealed class CentreSettingsEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri SettingsUri = new("/api/centre/settings", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Get_AsOwner_ReturnsTheContractShape()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.GetAsync(SettingsUri);
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Nile Tutoring Centre", body.RootElement.GetProperty("name").GetString());
        Assert.Equal("nile-centre", body.RootElement.GetProperty("slug").GetString());
        Assert.Equal("Africa/Cairo", body.RootElement.GetProperty("timeZoneId").GetString());
        Assert.Equal("ar", body.RootElement.GetProperty("defaultLocale").GetString());
        Assert.True(body.RootElement.GetProperty("version").GetUInt32() >= 0);
    }

    [Fact]
    public async Task Put_HappyPath_ThenGetShowsTheNewNameAndVersion_AndMeShowsTheNewName()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var version = await GetVersionAsync(owner.Client);

        using var putResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, SettingsUri, new { name = "Nile Learning Centre", defaultLocale = "en", version });
        Assert.Equal(HttpStatusCode.NoContent, putResponse.StatusCode);

        using var getResponse = await owner.Client.GetAsync(SettingsUri);
        var getBody = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("Nile Learning Centre", getBody.RootElement.GetProperty("name").GetString());
        Assert.Equal("en", getBody.RootElement.GetProperty("defaultLocale").GetString());
        Assert.NotEqual(version, getBody.RootElement.GetProperty("version").GetUInt32());

        using var meResponse = await owner.Client.GetAsync(new Uri("/api/me", UriKind.Relative));
        var meBody = await ReadJsonAsync(meResponse);
        var activeCentreId = meBody.RootElement.GetProperty("activeCentreId").GetGuid();
        var membership = meBody.RootElement.GetProperty("memberships").EnumerateArray()
            .Single(item => item.GetProperty("centreId").GetGuid() == activeCentreId);
        Assert.Equal("Nile Learning Centre", membership.GetProperty("centreName").GetString());
    }

    [Fact]
    public async Task Put_BodyWithSlug_Returns400()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var version = await GetVersionAsync(owner.Client);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, SettingsUri, new { name = "Nile Learning Centre", defaultLocale = "en", version, slug = "sneaky-slug" });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Put_EmptyName_Returns400WithErrorsName()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var version = await GetVersionAsync(owner.Client);

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, SettingsUri, new { name = "", defaultLocale = "en", version });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("name").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Get_AsSecretary_Returns403()
```

The excerpt stops at line 90; use the original file for the remaining 61 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
