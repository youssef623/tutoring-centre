# `tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 43.14: proves the seven Teacher endpoints' behaviour through the full HTTP pipeline, against the Day 43 contract.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 309 lines and 16,199 bytes. Source hash: `fcc56eaf2f2306e6495217551cb1fcb25eb67c433a69236df5eb32dc67848493`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L10): `public sealed class TeacherEndpointTests(ApiFactory factory) : IAsyncLifetime`
- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L14): `public async Task InitializeAsync()`
- [Line 20](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L20): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 23](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L23): `public async Task Create_ThenGet_Returns201WithLocationThen200()`
- [Line 43](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L43): `public async Task Create_TwoTeachersSharingAName_BothSucceed()`
- [Line 58](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L58): `public async Task Create_EmptyName_Returns400WithErrorsFullName(string fullName)`
- [Line 70](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L70): `public async Task Create_WithExtraUserIdField_Returns400RequestMalformedAndCreatesNoRow()`
- [Line 85](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L85): `public async Task Update_WithExtraUserIdField_Returns400RequestMalformedAndLeavesLinkUnchanged()`
- [Line 100](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L100): `public async Task Update_HappyPathThenStaleVersion()`
- [Line 119](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L119): `public async Task ArchiveThenRestore_AndTheirRuleViolations()`
- [Line 148](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L148): `public async Task Update_ArchivedTeacher_Returns422ArchivedReadOnly()`
- [Line 169](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L169): `public async Task SetAccount_IneligibleUserId_Returns422UserNotEligibleAndLeavesLinkUnchanged()`
- [Line 188](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L188): `public async Task SetAccount_LinkThenUnlink_Returns204EachAndListShowsDisplayName()`
- [Line 214](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L214): `public async Task SetAccount_UserAlreadyLinkedToAnotherTeacher_Returns409AccountTaken()`
- [Line 236](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L236): `public async Task List_NoCookie_Returns401()`
- [Line 249](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L249): `public async Task Create_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()`
- [Line 261](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L261): `public async Task Get_NonGuidId_Returns404RouteNotFound()`
- [Line 290](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L290): `private static async Task<uint> GetVersionAsync(HttpClient client, Guid id)`
- [Line 297](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L297): `private static async Task<int> CountTeachersAsync(HttpClient client)`
- [Line 304](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L304): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

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

/// <summary>Task 43.14: proves the seven Teacher endpoints' behaviour through the full HTTP pipeline, against the Day 43 contract.</summary>
[Collection(ApiCollection.Name)]
public sealed class TeacherEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri TeachersUri = new("/api/teachers", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Create_ThenGet_Returns201WithLocationThen200()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, TeachersUri, new { fullName = "Amina Hassan", phone = "+201012345678" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location;
        Assert.NotNull(location);

        using var getResponse = await nileOwner.Client.GetAsync(location);
        var body = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("Amina Hassan", body.RootElement.GetProperty("fullName").GetString());
        Assert.Equal("+201012345678", body.RootElement.GetProperty("phone").GetString());
        Assert.Equal("active", body.RootElement.GetProperty("status").GetString());
        Assert.True(body.RootElement.GetProperty("userId").ValueKind is JsonValueKind.Null);
    }

    [Fact]
    public async Task Create_TwoTeachersSharingAName_BothSucceed()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var first = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, TeachersUri, new { fullName = "Amina Hassan", phone = (string?)null });
        using var second = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, TeachersUri, new { fullName = "Amina Hassan", phone = (string?)null });

        Assert.Equal(HttpStatusCode.Created, first.StatusCode);
        Assert.Equal(HttpStatusCode.Created, second.StatusCode);
        Assert.NotEqual(first.Headers.Location, second.Headers.Location);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Create_EmptyName_Returns400WithErrorsFullName(string fullName)
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(nileOwner.Client, TeachersUri, new { fullName, phone = (string?)null });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("fullName").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_WithExtraUserIdField_Returns400RequestMalformedAndCreatesNoRow()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var countBefore = await CountTeachersAsync(nileOwner.Client);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            nileOwner.Client, TeachersUri, new { fullName = "Amina Hassan", phone = (string?)null, userId = Guid.NewGuid() });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("request.malformed", body.RootElement.GetProperty("code").GetString());
        Assert.Equal(countBefore, await CountTeachersAsync(nileOwner.Client));
    }

    [Fact]
    public async Task Update_WithExtraUserIdField_Returns400RequestMalformedAndLeavesLinkUnchanged()
    {
        using var nileOwner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (id, version) = await CreateTeacherAsync(nileOwner.Client, "Amina Hassan");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
```

The excerpt stops at line 90; use the original file for the remaining 219 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
