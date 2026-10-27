# `tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 49.6: the five enrollment endpoints' contract through the full HTTP pipeline — status, code, body shape and headers, never messages. Every test builds its own subjects, teachers, groups, students and enrollments through the earlier endpoints over HTTP, so each one also exercises them, and the seeded demo data never decides a result. Every error row of the Day 49 outcome table has its own case.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 764 lines and 41,393 bytes. Source hash: `3fd0b8eeaff282543c1571bfc016f3ebd01659872b627f9382b946f967ad0223`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Globalization;`
- Line 2: `using System.Net;`
- Line 3: `using System.Net.Http.Json;`
- Line 4: `using System.Text;`
- Line 5: `using System.Text.Json;`
- Line 6: `using TutoringCentre.Api.Auth;`
- Line 7: `using TutoringCentre.Api.Cli;`
- Line 8: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 19](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L19): `public sealed class EnrollmentEndpointTests(ApiFactory factory) : IAsyncLifetime`
- [Line 28](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L28): `public async Task InitializeAsync()`
- [Line 34](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L34): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 37](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L37): `public async Task Enroll_Returns201WithIdAndLocationAtTheRoster_AndTheRosterListsTheStudent()`
- [Line 61](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L61): `public async Task Enroll_SameStudentAgain_Returns409AlreadyEnrolledAndWritesNoRow()`
- [Line 77](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L77): `public async Task Enroll_WithAFieldBeyondStudentId_Returns400RequestMalformedAndWritesNoRow(string extraField)`
- [Line 97](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L97): `public async Task Enroll_MalformedJson_Returns400RequestMalformed()`
- [Line 108](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L108): `public async Task Enroll_EmptyStudentId_Returns400WithErrorsStudentId()`
- [Line 121](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L121): `public async Task Enroll_UnknownGroup_Returns404GroupNotFound()`
- [Line 132](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L132): `public async Task Enroll_UnknownStudent_Returns404StudentNotFound()`
- [Line 144](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L144): `public async Task Enroll_IntoAFullGroup_Returns409GroupFullAndWritesNoRow()`
- [Line 158](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L158): `public async Task Enroll_IntoAnArchivedGroup_Returns422GroupArchived()`
- [Line 172](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L172): `public async Task Enroll_AnArchivedStudent_Returns422StudentArchived()`
- [Line 186](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L186): `public async Task Enroll_WithoutTheAntiforgeryHeader_Returns403CsrfInvalidAndWritesNoRow()`
- [Line 199](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L199): `public async Task Enroll_AsTeacher_Returns403PermissionDenied()`
- [Line 213](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L213): `public async Task Enroll_BeforeSelectingACentre_Returns403TenantNotSelected()`
- [Line 224](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L224): `public async Task Roster_NoCookie_Returns401()`
- [Line 237](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L237): `public async Task Roster_IsOrderedByNormalizedStudentName_HasTheGroupsFactsAndNamesOnly()`
- [Line 270](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L270): `public async Task Roster_UnknownGroup_Returns404GroupNotFound()`
- [Line 280](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L280): `public async Task GroupList_EnrolledCount_EqualsTheRosterLength()`
- [Line 310](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L310): `public async Task StudentEnrollments_ListsActiveFirstThenEnded_WithTheFullShape()`
- [Line 355](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L355): `public async Task StudentEnrollments_UnknownStudent_Returns404StudentNotFound_AndAKnownStudentWithNoneReturnsEmptyItems()`
- [Line 369](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L369): `public async Task StudentDetail_StaysUnchanged_AndEmbedsNoEnrollments()`
- [Line 384](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L384): `public async Task Withdraw_Returns204_ThenAgainReturns422AlreadyEnded_AndTheRowStaysAsHistory()`
- [Line 411](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L411): `public async Task Withdraw_StaleVersion_Returns409ConcurrencyStaleAndTheEnrollmentStaysActive()`
- [Line 425](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L425): `public async Task Withdraw_UnknownEnrollment_Returns404EnrollmentNotFound()`
- [Line 437](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L437): `public async Task Withdraw_MissingVersionOrUnknownField_Returns400RequestMalformed(string json)`
- [Line 447](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L447): `public async Task Transfer_ToASameSubjectGroup_Returns200WithTheNewId_EndsTheOldAsTransferredAndStartsTheNew()`
- [Line 479](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L479): `public async Task Transfer_ToAGroupOfAnotherSubject_Returns422SubjectMismatchAndChangesNothing()`
- [Line 496](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L496): `public async Task Transfer_ToItsOwnGroup_Returns422SameGroup()`
- [Line 511](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L511): `public async Task Transfer_AnEndedEnrollment_Returns422AlreadyEnded()`
- [Line 532](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L532): `public async Task Transfer_ToAnArchivedGroup_Returns422GroupArchivedAndTheStudentStaysInTheSource()`
- [Line 549](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L549): `public async Task Transfer_ToAFullGroup_Returns409GroupFullAndTheStudentStaysInTheSource()`
- [Line 567](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L567): `public async Task Transfer_ToAGroupTheStudentIsAlreadyIn_Returns409AlreadyEnrolled()`
- [Line 585](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L585): `public async Task Transfer_StaleVersion_Returns409ConcurrencyStaleAndRollsBackBothChanges()`
- [Line 600](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L600): `public async Task Transfer_UnknownEnrollment_Returns404EnrollmentNotFound_AndUnknownTarget_Returns404GroupNotFound()`
- [Line 616](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L616): `public async Task Transfer_EmptyTargetGroupId_Returns400WithErrorsTargetGroupId()`
- [Line 628](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L628): `public async Task Transfer_WithAStudentIdInTheBody_Returns400RequestMalformed()`
- [Line 647](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L647): `private static async Task<Guid> CreateGroupAsync(HttpClient client, string name, int capacity = 20)`
- [Line 654](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L654): `private static Task<Guid> CreateGroupAsync(HttpClient client, string name, Guid subjectId, Guid teacherId, int capacity = 20) =>`
- [Line 657](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L657): `private static Task<Guid> CreateSubjectAsync(HttpClient client, string name) => CreateAsync(client, "/api/subjects", new { name });`
- [Line 659](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L659): `private static Task<Guid> CreateTeacherAsync(HttpClient client, string fullName) =>`
- [Line 662](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L662): `private static Task<Guid> RegisterStudentAsync(HttpClient client, string fullName) =>`
- [Line 671](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L671): `private static Task<Guid> EnrollAsync(HttpClient client, Guid groupId, Guid studentId) =>`
- [Line 674](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L674): `private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)`
- [Line 681](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L681): `private static async Task ArchiveGroupAsync(HttpClient client, Guid id)`
- [Line 689](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L689): `private static async Task ArchiveStudentAsync(HttpClient client, Guid id)`
- [Line 697](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L697): `private static async Task<JsonElement> GetRosterAsync(HttpClient client, Guid groupId)`
- [Line 704](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L704): `private static async Task<uint> GetRosterVersionAsync(HttpClient client, Guid groupId, Guid enrollmentId) =>`
- [Line 708](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L708): `private static async Task<List<JsonElement>> ListStudentEnrollmentsAsync(HttpClient client, Guid studentId)`
- [Line 715](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L715): `private static async Task<List<string>> GetStudentPropertyNamesAsync(HttpClient client, Guid studentId)`
- [Line 722](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L722): `private Task<long> CountEnrollmentRowsAsync(Guid groupId) =>`
- [Line 725](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L725): `private Task<string> SnapshotEnrollmentStatusAsync(Guid enrollmentId) =>`
- [Line 729](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L729): `private Task<string> SnapshotEnrollmentsAsync(Guid firstGroupId, Guid secondGroupId) =>`
- [Line 736](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L736): `private static string UniquePhone() => "015" + Interlocked.Increment(ref _nextPhoneSuffix).ToString(CultureInfo.InvariantCulture);`
- [Line 738](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L738): `private static Task<HttpResponseMessage> PostAsync(HttpClient client, string url, object body) =>`
- [Line 741](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L741): `private static async Task<HttpResponseMessage> PostRawAsync(HttpClient client, string url, string json)`
- [Line 752](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L752): `private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedCode)`
- [Line 759](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L759): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Globalization;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TutoringCentre.Api.Auth;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 49.6: the five enrollment endpoints' contract through the full HTTP pipeline — status, code, body shape
/// and headers, never messages. Every test builds its own subjects, teachers, groups, students and enrollments
/// through the earlier endpoints over HTTP, so each one also exercises them, and the seeded demo data never
/// decides a result. Every error row of the Day 49 outcome table has its own case.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class EnrollmentEndpointTests(ApiFactory factory) : IAsyncLifetime
{
    // An interlocked counter never repeats within a run, unlike random digits, so no parent phone ever collides.
    private static int _nextPhoneSuffix = 30000000;

    private static readonly string[] ExpectedRosterOrder = ["ahmed Samir", "Mona Hassan", "Zeinab Ali", "إبراهيم علي", "أحمد سمير"];

    private static readonly string[] RosterEntryFields = ["enrolledAt", "enrollmentId", "studentId", "studentName", "version"];

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Enroll_Returns201WithIdAndLocationAtTheRoster_AndTheRosterListsTheStudent()
    {
        // Only the owner may create teachers, so the owner arranges the group; the secretary does the rest.
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var secretary = await SignInHelper.SignInAsNileSecretaryAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Happy");
        var studentId = await RegisterStudentAsync(secretary.Client, "Salma Fathy");

        using var response = await PostAsync(secretary.Client, $"/api/groups/{groupId}/enrollments", new { studentId });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/api/groups/{groupId}/roster", response.Headers.Location?.OriginalString);
        var body = await ReadJsonAsync(response);
        var enrollmentId = body.RootElement.GetProperty("id").GetGuid();
        Assert.Single(body.RootElement.EnumerateObject());

        using var rosterResponse = await secretary.Client.GetAsync(response.Headers.Location);
        Assert.Equal(HttpStatusCode.OK, rosterResponse.StatusCode);
        var entry = Assert.Single((await ReadJsonAsync(rosterResponse)).RootElement.GetProperty("items").EnumerateArray());
        Assert.Equal(enrollmentId, entry.GetProperty("enrollmentId").GetGuid());
        Assert.Equal(studentId, entry.GetProperty("studentId").GetGuid());
    }

    [Fact]
    public async Task Enroll_SameStudentAgain_Returns409AlreadyEnrolledAndWritesNoRow()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Twice");
        var studentId = await RegisterStudentAsync(owner.Client, "Twice Student");
        await EnrollAsync(owner.Client, groupId, studentId);

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/enrollments", new { studentId });

        await AssertProblemAsync(response, HttpStatusCode.Conflict, "enrollment.already_enrolled");
        Assert.Equal(1, await CountEnrollmentRowsAsync(groupId));
    }

    [Theory]
    [InlineData("groupId")]
    [InlineData("status")]
    public async Task Enroll_WithAFieldBeyondStudentId_Returns400RequestMalformedAndWritesNoRow(string extraField)
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var groupId = await CreateGroupAsync(owner.Client, "Enroll Over Posted");
        var otherGroupId = await CreateGroupAsync(owner.Client, "Enroll Over Posted Other");
        var studentId = await RegisterStudentAsync(owner.Client, "Over Posted Student");
        var body = new Dictionary<string, object>
        {
            ["studentId"] = studentId,
            [extraField] = extraField == "groupId" ? otherGroupId : "active",
        };

        using var response = await PostAsync(owner.Client, $"/api/groups/{groupId}/enrollments", body);

```

The excerpt stops at line 90; use the original file for the remaining 674 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
