# `tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 30.7: proves the five staff endpoints' contract behaviour through the full HTTP pipeline (Day 30 contract).

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 235 lines and 11,369 bytes. Source hash: `6b09ab1ff2cda1349c3d528ba2c591de18d16ef096563e6745fbb80910444400`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L10): `public sealed class StaffEndpointTests(ApiFactory factory) : IAsyncLifetime`
- [Line 17](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L17): `public async Task InitializeAsync()`
- [Line 23](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L23): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 26](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L26): `public async Task List_AsOwner_ReturnsAllNileMembersWithOnlyContractFields()`
- [Line 45](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L45): `public async Task Create_NewEmail_Returns201WithPasswordAndNoStore()`
- [Line 60](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L60): `public async Task Create_EmailOfAnExistingAccount_Returns201WithNullPasswordAndNoStore()`
- [Line 74](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L74): `public async Task Create_EmailAlreadyAMemberOfThisCentre_Returns409WithErrorsEmail()`
- [Line 88](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L88): `public async Task Create_UnknownRole_Returns400RequestMalformed()`
- [Line 101](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L101): `public async Task Create_OverPostedCentreId_Returns400RequestMalformedAndCreatesNoRow()`
- [Line 119](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L119): `public async Task Create_InvalidEmail_Returns400WithErrorsEmail(string email)`
- [Line 132](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L132): `public async Task ChangeRole_HappyPathThenStaleVersion()`
- [Line 149](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L149): `public async Task ChangeRole_ValidationErrors_HaveFieldNames()`
- [Line 161](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L161): `public async Task Deactivate_ThenReactivate_FlipsStatusInTheList()`
- [Line 180](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L180): `public async Task Reactivate_AlreadyActive_Returns422MembershipAlreadyActive()`
- [Line 194](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L194): `public async Task Deactivate_AlreadyInactive_Returns422MembershipAlreadyInactive()`
- [Line 215](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L215): `private static async Task<string> MemberStatusAsync(HttpClient client, Guid membershipId)`
- [Line 223](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L223): `private static async Task<int> CountStaffAsync(HttpClient client)`
- [Line 230](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffEndpointTests.cs#L230): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Staff;

/// <summary>Task 30.7: proves the five staff endpoints' contract behaviour through the full HTTP pipeline (Day 30 contract).</summary>
[Collection(ApiCollection.Name)]
public sealed class StaffEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri StaffUri = new("/api/staff", UriKind.Relative);

    private static readonly string[] ExpectedStaffMemberFields =
        ["displayName", "email", "isCurrentUser", "membershipId", "role", "status", "userId", "version"];

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task List_AsOwner_ReturnsAllNileMembersWithOnlyContractFields()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await owner.Client.GetAsync(StaffUri);
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var items = body.RootElement.GetProperty("items").EnumerateArray().ToList();
        Assert.Equal(4, items.Count);
        Assert.Contains(items, item => item.GetProperty("status").GetString() == "inactive");
        foreach (var item in items)
        {
            var fields = item.EnumerateObject().Select(p => p.Name).OrderBy(n => n, StringComparer.Ordinal);
            Assert.Equal(ExpectedStaffMemberFields, fields);
        }
    }

    [Fact]
    public async Task Create_NewEmail_Returns201WithPasswordAndNoStore()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StaffUri, new { email = "new-contract@nile.test", displayName = "New Contract", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
        Assert.Null(response.Headers.Location);
        Assert.False(string.IsNullOrEmpty(body.RootElement.GetProperty("temporaryPassword").GetString()));
    }

    [Fact]
    public async Task Create_EmailOfAnExistingAccount_Returns201WithNullPasswordAndNoStore()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StaffUri, new { email = "owner@maadi.test", displayName = "Maadi Owner", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("no-store", response.Headers.CacheControl?.ToString());
        Assert.Equal(JsonValueKind.Null, body.RootElement.GetProperty("temporaryPassword").ValueKind);
    }

    [Fact]
    public async Task Create_EmailAlreadyAMemberOfThisCentre_Returns409WithErrorsEmail()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StaffUri, new { email = "secretary@nile.test", displayName = "Dup", role = "teacher", preferredLocale = "en" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("staff.already_member", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("email").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Create_UnknownRole_Returns400RequestMalformed()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
```

The excerpt stops at line 90; use the original file for the remaining 145 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
