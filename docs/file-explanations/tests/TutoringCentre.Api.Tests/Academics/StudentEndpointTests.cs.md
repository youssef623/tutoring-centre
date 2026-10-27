# `tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 41.5: proves the nine student endpoints' behaviour through the full HTTP pipeline, against the Day 41 contract's outcome table.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 632 lines and 34,183 bytes. Source hash: `a937019934b4dfecaccfcb7480935111e1f052119000cf46b06d10c4b08a0233`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L10): `public sealed class StudentEndpointTests(ApiFactory factory) : IAsyncLifetime`
- [Line 14](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L14): `public async Task InitializeAsync()`
- [Line 20](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L20): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 23](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L23): `public async Task Register_NewParent_ThenGet_ThenUpdate_HappyPath()`
- [Line 59](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L59): `public async Task Register_ExistingParentForSibling_SharesTheSameParentId()`
- [Line 74](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L74): `public async Task Register_DuplicateParentPhone_Returns409WithNestedParentPhoneField()`
- [Line 90](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L90): `public async Task Register_BothParentFormsGiven_Returns400WithErrorsParentAndCreatesNoRow()`
- [Line 106](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L106): `public async Task Register_NeitherParentFormGiven_Returns400WithErrorsParent()`
- [Line 120](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L120): `public async Task Register_InvalidNestedParentPhone_Returns400PhoneInvalidWithNestedField()`
- [Line 135](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L135): `public async Task Register_InvalidStudentOwnPhone_Returns400PhoneInvalidWithTopLevelField()`
- [Line 150](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L150): `public async Task Register_OneCharacterName_Returns400WithErrorsFullName()`
- [Line 164](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L164): `public async Task Register_UnknownFieldAtTopLevel_Returns400RequestMalformedAndCreatesNoRow()`
- [Line 180](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L180): `public async Task Register_UnknownFieldInsideNestedParent_Returns400RequestMalformedAndCreatesNoRow()`
- [Line 196](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L196): `public async Task List_EmptySearch_ReturnsActiveStudentsInNormalizedOrder()`
- [Line 215](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L215): `public async Task List_HamzaInsensitiveSearch_FindsStudentRegisteredWithHamza()`
- [Line 228](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L228): `public async Task List_PageSizeTooLarge_Returns400WithErrorsPageSize()`
- [Line 240](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L240): `public async Task List_MalformedCursor_Returns400WithErrorsCursor()`
- [Line 252](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L252): `public async Task List_NextCursorRoundTrips_SecondPageHasNoOverlap()`
- [Line 273](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L273): `public async Task Update_StaleVersion_Returns409ConcurrencyStale()`
- [Line 293](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L293): `public async Task Update_UnknownId_Returns404StudentNotFound()`
- [Line 307](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L307): `public async Task Get_UnknownId_Returns404StudentNotFound()`
- [Line 319](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L319): `public async Task ArchiveThenArchiveAgain_SecondReturns422AlreadyArchived()`
- [Line 338](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L338): `public async Task Archive_WithAnActiveEnrollment_Returns422HasActiveEnrollmentsAndStudentStaysActive()`
- [Line 359](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L359): `public async Task RestoreAnActiveStudent_Returns422NotArchived()`
- [Line 373](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L373): `public async Task UpdateAnArchivedStudent_Returns422ArchivedReadOnly()`
- [Line 392](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L392): `public async Task LinkParent_UnknownParentId_Returns404ParentNotFound()`
- [Line 407](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L407): `public async Task LinkParent_AlreadyLinked_Returns409AlreadyLinked()`
- [Line 422](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L422): `public async Task FullLinkSequence_MakePrimaryThenUnlinkThenLastParentRefused()`
- [Line 455](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L455): `public async Task UnlinkParent_NotLinked_Returns404StudentParentNotLinked()`
- [Line 470](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L470): `public async Task MakePrimary_AlreadyPrimary_Returns422AlreadyPrimary()`
- [Line 485](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L485): `public async Task LinkParent_FifthParent_Returns422TooManyParents()`
- [Line 512](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L512): `public async Task Get_NoCookie_Returns401()`
- [Line 525](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L525): `public async Task Register_WithoutTheAntiforgeryHeader_Returns403CsrfInvalid()`
- [Line 538](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L538): `public async Task Register_TwoCentreTeacherBeforeSelectingACentre_Returns403TenantNotSelected()`
- [Line 553](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L553): `public async Task Register_AsTeacherWithoutPermission_Returns403PermissionDenied()`
- [Line 567](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L567): `public async Task ErrorBody_HasCodeTraceIdAndCorrelationIdAndNoStackTrace()`
- [Line 596](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L596): `private static async Task<Guid> RegisterSecondParentAsync(HttpClient client, string parentFullName, string parentPhone)`
- [Line 606](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L606): `private static async Task<Guid> CreateGroupAsync(HttpClient client, string name)`
- [Line 613](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L613): `private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)`
- [Line 620](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L620): `private static async Task<uint> GetVersionAsync(HttpClient client, Guid studentId)`
- [Line 627](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L627): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

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

/// <summary>Task 41.5: proves the nine student endpoints' behaviour through the full HTTP pipeline, against the Day 41 contract's outcome table.</summary>
[Collection(ApiCollection.Name)]
public sealed class StudentEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri StudentsUri = new("/api/students", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Register_NewParent_ThenGet_ThenUpdate_HappyPath()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "أحمد محمود", phone = (string?)null, schoolYear = "الصف الثالث الثانوي", parent = new { fullName = "محمود حسن", phone = "01012345678" }, relationship = "father" });
        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        var location = createResponse.Headers.Location;
        Assert.NotNull(location);
        var createBody = await ReadJsonAsync(createResponse);
        var studentId = createBody.RootElement.GetProperty("id").GetGuid();
        var parentId = createBody.RootElement.GetProperty("parentId").GetGuid();
        Assert.NotEqual(Guid.Empty, parentId);

        using var getResponse = await owner.Client.GetAsync(location);
        var getBody = await ReadJsonAsync(getResponse);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.Equal("أحمد محمود", getBody.RootElement.GetProperty("fullName").GetString());
        Assert.Equal("active", getBody.RootElement.GetProperty("status").GetString());
        var parent = Assert.Single(getBody.RootElement.GetProperty("parents").EnumerateArray());
        Assert.True(parent.GetProperty("isPrimary").GetBoolean());
        Assert.Equal("father", parent.GetProperty("relationship").GetString());
        var version = getBody.RootElement.GetProperty("version").GetUInt32();

        using var updateResponse = await AntiforgeryTestHelper.SendAsJsonAsync(
            owner.Client, HttpMethod.Put, location,
            new { fullName = "أحمد محمود حسن", phone = (string?)null, schoolYear = "الصف الثالث الثانوي", version });
        Assert.Equal(HttpStatusCode.NoContent, updateResponse.StatusCode);

        using var getAfterUpdate = await owner.Client.GetAsync(location);
        var afterBody = await ReadJsonAsync(getAfterUpdate);
        Assert.Equal("أحمد محمود حسن", afterBody.RootElement.GetProperty("fullName").GetString());
    }

    [Fact]
    public async Task Register_ExistingParentForSibling_SharesTheSameParentId()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var (_, parentId, _) = await RegisterStudentAsync(owner.Client, "أحمد محمود", "محمود حسن", "01012345671");

        using var siblingResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "محمد محمود", phone = (string?)null, schoolYear = (string?)null, parent = new { id = parentId }, relationship = "father" });

        Assert.Equal(HttpStatusCode.Created, siblingResponse.StatusCode);
        var body = await ReadJsonAsync(siblingResponse);
        Assert.Equal(parentId, body.RootElement.GetProperty("parentId").GetGuid());
    }

    [Fact]
    public async Task Register_DuplicateParentPhone_Returns409WithNestedParentPhoneField()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        await RegisterStudentAsync(owner.Client, "أحمد محمود", "محمود حسن", "01012345672");

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StudentsUri,
            new { fullName = "Another Student", phone = (string?)null, schoolYear = (string?)null, parent = new { fullName = "Another Parent", phone = "0101 234 5672" }, relationship = "father" });

        var body = await ReadJsonAsync(response);
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.Equal("parent.phone_taken", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("parent.phone").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Register_BothParentFormsGiven_Returns400WithErrorsParentAndCreatesNoRow()
```

The excerpt stops at line 90; use the original file for the remaining 542 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
