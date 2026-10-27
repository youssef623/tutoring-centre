# `tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 49.7: the ownership guard on enrollment, as black-box behaviour through teacher@both.test's real session. The roster inherits its group's rule: 200 for the own group, 404 (never 403) for another teacher's. A student enrolled in both an own and another teacher's group shows the teacher only the own enrollment. Writes are 403 even on the own group, because permission is evaluated before ownership, and they change nothing.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 205 lines and 11,285 bytes. Source hash: `8caba2ebb15907014610cb5246ce53806a14f2fd8329dd3198c6dc33c61fa3f4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Globalization;`
- Line 2: `using System.Net;`
- Line 3: `using System.Text.Json;`
- Line 4: `using TutoringCentre.Api.Cli;`
- Line 5: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L16): `public sealed class EnrollmentTeacherScopeTests(ApiFactory factory) : IAsyncLifetime`
- [Line 22](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L22): `public async Task InitializeAsync()`
- [Line 28](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L28): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 31](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L31): `public async Task LinkedTeacher_RosterOfTheOwnGroup_Returns200WithItsStudents()`
- [Line 45](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L45): `public async Task LinkedTeacher_RosterOfAnotherTeachersGroup_Returns404GroupNotFound()`
- [Line 56](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L56): `public async Task LinkedTeacher_StudentEnrolledInOwnAndOtherGroup_ListsOnlyTheOwnEnrollment()`
- [Line 78](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L78): `public async Task LinkedTeacher_EveryWriteRoute_Returns403PermissionDeniedAndChangesNothing(string route)`
- [Line 101](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L101): `/// Any teacher record already linked to that user in Nile (the demo seed links one) is unlinked first, so`
- [Line 104](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L104): `private async Task<Arrangement> ArrangeAsync()`
- [Line 138](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L138): `private static async Task<Guid> FindUserIdAsync(HttpClient ownerClient, string email)`
- [Line 146](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L146): `private static async Task SetTeacherAccountAsync(HttpClient ownerClient, Guid teacherId, Guid? userId)`
- [Line 155](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L155): `private async Task<Guid> RegisterStudentAsOwnerAsync(string fullName)`
- [Line 161](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L161): `private static Task<Guid> RegisterStudentAsync(HttpClient client, string fullName) =>`
- [Line 170](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L170): `private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)`
- [Line 177](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L177): `private async Task<uint> GetRosterVersionAsOwnerAsync(Guid groupId, Guid enrollmentId)`
- [Line 185](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L185): `private Task<string> SnapshotEnrollmentsAsync() =>`
- [Line 189](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L189): `private static string UniquePhone() => "015" + Interlocked.Increment(ref _nextPhoneSuffix).ToString(CultureInfo.InvariantCulture);`
- [Line 191](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L191): `private static async Task AssertProblemAsync(HttpResponseMessage response, HttpStatusCode expectedStatus, string expectedCode)`
- [Line 198](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L198): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`
- [Line 204](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L204): `private sealed record Arrangement(Guid OwnGroupId, Guid OwnSecondGroupId, Guid OtherGroupId, Guid StudentId, Guid OwnEnrollmentId);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Globalization;
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Academics;

/// <summary>
/// Task 49.7: the ownership guard on enrollment, as black-box behaviour through teacher@both.test's real session.
/// The roster inherits its group's rule: 200 for the own group, 404 (never 403) for another teacher's. A student
/// enrolled in both an own and another teacher's group shows the teacher only the own enrollment. Writes are 403
/// even on the own group, because permission is evaluated before ownership, and they change nothing.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class EnrollmentTeacherScopeTests(ApiFactory factory) : IAsyncLifetime
{
    private const string TwoCentreTeacherEmail = "teacher@both.test";

    private static int _nextPhoneSuffix = 50000000;

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task LinkedTeacher_RosterOfTheOwnGroup_Returns200WithItsStudents()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(new Uri($"/api/groups/{arranged.OwnGroupId}/roster", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var roster = (await ReadJsonAsync(response)).RootElement;
        Assert.Equal(arranged.OwnGroupId, roster.GetProperty("groupId").GetGuid());
        Assert.Equal(arranged.StudentId, Assert.Single(roster.GetProperty("items").EnumerateArray()).GetProperty("studentId").GetGuid());
    }

    [Fact]
    public async Task LinkedTeacher_RosterOfAnotherTeachersGroup_Returns404GroupNotFound()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");

        using var response = await teacher.Client.GetAsync(new Uri($"/api/groups/{arranged.OtherGroupId}/roster", UriKind.Relative));

        await AssertProblemAsync(response, HttpStatusCode.NotFound, "group.not_found");
    }

    [Fact]
    public async Task LinkedTeacher_StudentEnrolledInOwnAndOtherGroup_ListsOnlyTheOwnEnrollment()
    {
        var arranged = await ArrangeAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var teacherResponse = await teacher.Client.GetAsync(new Uri($"/api/students/{arranged.StudentId}/enrollments", UriKind.Relative));
        using var ownerResponse = await owner.Client.GetAsync(new Uri($"/api/students/{arranged.StudentId}/enrollments", UriKind.Relative));

        Assert.Equal(HttpStatusCode.OK, teacherResponse.StatusCode);
        var only = Assert.Single((await ReadJsonAsync(teacherResponse)).RootElement.GetProperty("items").EnumerateArray());
        Assert.Equal(arranged.OwnEnrollmentId, only.GetProperty("enrollmentId").GetGuid());
        Assert.Equal(arranged.OwnGroupId, only.GetProperty("groupId").GetGuid());

        // The same route as the owner shows both, so the teacher's single item is the scope, not missing data.
        Assert.Equal(2, (await ReadJsonAsync(ownerResponse)).RootElement.GetProperty("items").GetArrayLength());
    }

    [Theory]
    [InlineData("enroll")]
    [InlineData("withdraw")]
    [InlineData("transfer")]
    public async Task LinkedTeacher_EveryWriteRoute_Returns403PermissionDeniedAndChangesNothing(string route)
    {
        var arranged = await ArrangeAsync();
        var version = await GetRosterVersionAsOwnerAsync(arranged.OwnGroupId, arranged.OwnEnrollmentId);
        var newStudentId = await RegisterStudentAsOwnerAsync("Scope Write Student");
        var before = await SnapshotEnrollmentsAsync();
        using var teacher = await SignInHelper.SignInAsTwoCentreTeacherAsync(factory, "nile-centre");
        var (url, body) = route switch
        {
            "enroll" => ($"/api/groups/{arranged.OwnGroupId}/enrollments", (object)new { studentId = newStudentId }),
            "withdraw" => ($"/api/enrollments/{arranged.OwnEnrollmentId}/withdraw", new { version }),
            _ => ($"/api/enrollments/{arranged.OwnEnrollmentId}/transfer", new { targetGroupId = arranged.OwnSecondGroupId, version }),
        };
```

The excerpt stops at line 90; use the original file for the remaining 115 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
