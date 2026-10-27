# `tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 30.7: threat-driven tests for the five staff endpoints — self-escalation, over-posting, a secretary attempting to manage staff, and a role change ending the affected user's session.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 163 lines and 7,901 bytes. Source hash: `3795d2efaa6586cc02f8900985ef34c1b4e2c53f66ae48fef58ae7a805ebf745`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text;`
- Line 3: `using System.Text.Json;`
- Line 4: `using Microsoft.AspNetCore.Http;`
- Line 5: `using Microsoft.Extensions.DependencyInjection;`
- Line 6: `using TutoringCentre.Api.Auth;`
- Line 7: `using TutoringCentre.Api.Cli;`
- Line 8: `using TutoringCentre.Api.Http;`
- Line 9: `using TutoringCentre.Api.Tests.Fixtures;`
- Line 10: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L19): `public sealed class StaffSecurityTests(ApiFactory factory) : IAsyncLifetime`
- [Line 23](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L23): `public async Task InitializeAsync()`
- [Line 29](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L29): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 32](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L32): `public async Task Owner_ChangesOwnRole_Returns422CannotChangeSelf()`
- [Line 46](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L46): `public async Task Owner_DeactivatesSelf_Returns422CannotChangeSelf()`
- [Line 66](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L66): `public async Task LastOwnerError_MapsTo422ThroughTheRealHttpMapping()`
- [Line 86](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L86): `public async Task ChangeRole_OverPostedOrUnknownRole_Returns400(string rawBody)`
- [Line 103](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L103): `public async Task Secretary_PostsValidCreateBody_Returns403AndCreatesNoUserOrMembership()`
- [Line 120](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L120): `public async Task Secretary_GetsStaffList_Returns403()`
- [Line 132](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L132): `public async Task ChangeRole_AffectedUsersNextRequest_Returns401()`
- [Line 158](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffSecurityTests.cs#L158): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Http;
using TutoringCentre.Api.Tests.Fixtures;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Api.Tests.Staff;

/// <summary>
/// Task 30.7: threat-driven tests for the five staff endpoints — self-escalation, over-posting, a secretary
/// attempting to manage staff, and a role change ending the affected user's session.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class StaffSecurityTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri StaffUri = new("/api/staff", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Owner_ChangesOwnRole_Returns422CannotChangeSelf()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, version) = await FindMemberAsync(owner.Client, "owner@nile.test");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, new Uri($"/api/staff/{membershipId}/role", UriKind.Relative), new { role = "teacher", version });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("staff.cannot_change_self", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task Owner_DeactivatesSelf_Returns422CannotChangeSelf()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, version) = await FindMemberAsync(owner.Client, "owner@nile.test");

        using var response = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Post, new Uri($"/api/staff/{membershipId}/deactivate", UriKind.Relative), new { version });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("staff.cannot_change_self", body.RootElement.GetProperty("code").GetString());
    }

    /// <summary>
    /// staff.last_owner cannot be reached over sequential HTTP requests — the self rule fires first whenever
    /// the sole owner with staff.manage tries to touch the only other owner-shaped target, themselves. The
    /// invariant itself stays covered by the real concurrent race (Task 29.12); this asserts only that the
    /// HTTP layer maps the code correctly, the same way ResultHttpExtensionsTests proves every other mapping.
    /// </summary>
    [Fact]
    public async Task LastOwnerError_MapsTo422ThroughTheRealHttpMapping()
    {
        var error = Error.Rule("staff.last_owner", "This centre must keep at least one active owner.");

        using var services = new ServiceCollection().AddLogging().AddOptions().BuildServiceProvider();
        var context = new DefaultHttpContext { RequestServices = services };
        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        await error.ToProblemResult().ExecuteAsync(context);

        buffer.Position = 0;
        using var body = await JsonDocument.ParseAsync(buffer);
        Assert.Equal(StatusCodes.Status422UnprocessableEntity, context.Response.StatusCode);
        Assert.Equal("staff.last_owner", body.RootElement.GetProperty("code").GetString());
    }

    [Theory]
    [InlineData("""{"role":"owner","version":1,"centreId":"11111111-1111-1111-1111-111111111111"}""")]
    [InlineData("""{"role":"admin","version":1}""")]
    public async Task ChangeRole_OverPostedOrUnknownRole_Returns400(string rawBody)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (membershipId, _) = await FindMemberAsync(owner.Client, "secretary@nile.test");
        var token = await AntiforgeryTestHelper.GetCsrfTokenAsync(owner.Client);
```

The excerpt stops at line 90; use the original file for the remaining 73 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
