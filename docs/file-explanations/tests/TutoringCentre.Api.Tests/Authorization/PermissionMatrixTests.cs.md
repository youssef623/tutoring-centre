# `tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 30.8 (extended Task 32.11, Task 43.14, Task 45.9, Task 49.9, Task 53.4): one table of role x endpoint -> expected status, executed over real HTTP against Nile's three roles, now including the seven teacher endpoints (21 more literal cases), the eight group endpoints (24 more), the five enrollment endpoints (15 more) and the six schedule endpoints (18 more). The matrix records the permission decision only: Teacher is allowed on the two group reads, the two enrollment reads and the two schedule reads (and reads a group it owns here), while what a teacher may see is ownership, proven separately by GroupTeacherScopeTests, EnrollmentTeacherScopeTests and ScheduleTeacherScopeTests. The schedule cases run on a fixed application clock, so no case depends on today. Allowed cases assert a non-403 success status; denied cases assert 403 auth.permission_denied and an unchanged database, compared against the exact pre-request state, not a snapshot taken after the request already ran. Values are written literally here, never derived from RolePermissions — that would prove the matrix agrees with itself, not with the contract.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 1486 lines and 71,644 bytes. Source hash: `2a23ec6874719bf5e1cabc44ef2d9965279b269ce39454f8dde1b5fcb492c5ba`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Globalization;`
- Line 2: `using System.Net;`
- Line 3: `using System.Text.Json;`
- Line 4: `using TutoringCentre.Api.Cli;`
- Line 5: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 24](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L24): `public sealed class PermissionMatrixTests(ApiFactory factory) : IAsyncLifetime`
- [Line 84](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L84): `public async Task InitializeAsync()`
- [Line 91](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L91): `public Task DisposeAsync()`
- [Line 101](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L101): `public async Task SubjectsList(string role, HttpStatusCode expected)`
- [Line 114](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L114): `public async Task SubjectsGetById(string role, HttpStatusCode expected)`
- [Line 128](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L128): `public async Task SubjectsCreate(string role, HttpStatusCode expected)`
- [Line 148](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L148): `public async Task SubjectsRename(string role, HttpStatusCode expected)`
- [Line 168](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L168): `public async Task SubjectsArchive(string role, HttpStatusCode expected)`
- [Line 188](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L188): `public async Task SubjectsRestore(string role, HttpStatusCode expected)`
- [Line 208](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L208): `public async Task StaffList(string role, HttpStatusCode expected)`
- [Line 225](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L225): `public async Task StaffCreate(string role, HttpStatusCode expected)`
- [Line 248](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L248): `public async Task StaffChangeRole(string role, HttpStatusCode expected)`
- [Line 268](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L268): `public async Task StaffDeactivate(string role, HttpStatusCode expected)`
- [Line 288](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L288): `public async Task StaffReactivate(string role, HttpStatusCode expected)`
- [Line 308](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L308): `public async Task AuditList(string role, HttpStatusCode expected)`
- [Line 325](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L325): `public async Task CentreSettingsGet(string role, HttpStatusCode expected)`
- [Line 342](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L342): `public async Task CentreSettingsUpdate(string role, HttpStatusCode expected)`
- [Line 364](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L364): `public async Task ParentsCreate(string role, HttpStatusCode expected)`
- [Line 385](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L385): `public async Task ParentsGetById(string role, HttpStatusCode expected)`
- [Line 403](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L403): `public async Task ParentsUpdate(string role, HttpStatusCode expected)`
- [Line 425](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L425): `public async Task ParentsLookup(string role, HttpStatusCode expected)`
- [Line 443](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L443): `public async Task StudentsList(string role, HttpStatusCode expected)`
- [Line 460](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L460): `public async Task StudentsGetById(string role, HttpStatusCode expected)`
- [Line 478](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L478): `public async Task StudentsRegister(string role, HttpStatusCode expected)`
- [Line 503](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L503): `public async Task StudentsUpdate(string role, HttpStatusCode expected)`
- [Line 525](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L525): `public async Task StudentsArchive(string role, HttpStatusCode expected)`
- [Line 545](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L545): `public async Task StudentsRestore(string role, HttpStatusCode expected)`
- [Line 565](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L565): `public async Task StudentsLinkParent(string role, HttpStatusCode expected)`
- [Line 587](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L587): `public async Task StudentsUnlinkParent(string role, HttpStatusCode expected)`
- [Line 608](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L608): `public async Task StudentsMakePrimary(string role, HttpStatusCode expected)`
- [Line 628](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L628): `public async Task TeachersList(string role, HttpStatusCode expected)`
- [Line 645](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L645): `public async Task TeachersGetById(string role, HttpStatusCode expected)`
- [Line 663](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L663): `public async Task TeachersCreate(string role, HttpStatusCode expected)`
- [Line 683](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L683): `public async Task TeachersUpdate(string role, HttpStatusCode expected)`
- [Line 704](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L704): `public async Task TeachersSetAccount(string role, HttpStatusCode expected)`
- [Line 725](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L725): `public async Task TeachersArchive(string role, HttpStatusCode expected)`
- [Line 745](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L745): `public async Task TeachersRestore(string role, HttpStatusCode expected)`
- [Line 765](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L765): `public async Task GroupsList(string role, HttpStatusCode expected)`
- [Line 782](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L782): `public async Task GroupsGetById(string role, HttpStatusCode expected)`
- [Line 802](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L802): `public async Task GroupsCreate(string role, HttpStatusCode expected)`
- [Line 825](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L825): `public async Task GroupsRename(string role, HttpStatusCode expected)`
- [Line 846](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L846): `public async Task GroupsChangeCapacity(string role, HttpStatusCode expected)`
- [Line 867](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L867): `public async Task GroupsAssignTeacher(string role, HttpStatusCode expected)`
- [Line 889](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L889): `public async Task GroupsArchive(string role, HttpStatusCode expected)`
- [Line 910](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L910): `public async Task GroupsRestore(string role, HttpStatusCode expected)`
- [Line 931](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L931): `public async Task EnrollmentsEnroll(string role, HttpStatusCode expected)`
- [Line 953](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L953): `public async Task EnrollmentsRoster(string role, HttpStatusCode expected)`
- [Line 973](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L973): `public async Task EnrollmentsListForStudent(string role, HttpStatusCode expected)`
- [Line 991](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L991): `public async Task EnrollmentsWithdraw(string role, HttpStatusCode expected)`
- [Line 1013](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1013): `public async Task EnrollmentsTransfer(string role, HttpStatusCode expected)`
- [Line 1037](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1037): `public async Task ScheduleListRules(string role, HttpStatusCode expected)`
- [Line 1058](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1058): `public async Task ScheduleAddRule(string role, HttpStatusCode expected)`
- [Line 1080](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1080): `public async Task ScheduleEndRule(string role, HttpStatusCode expected)`
- [Line 1103](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1103): `public async Task ScheduleExtend(string role, HttpStatusCode expected)`
- [Line 1125](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1125): `public async Task ScheduleTimetable(string role, HttpStatusCode expected)`
- [Line 1143](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1143): `public async Task ScheduleCancelSession(string role, HttpStatusCode expected)`
- [Line 1168](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1168): `private void FixScheduleClock() => factory.Time.FixAt(new DateTimeOffset(2027, 4, 20, 8, 0, 0, TimeSpan.Zero));`
- [Line 1170](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1170): `private async Task<Guid> AddRuleToTeachersOwnGroupAsOwnerAsync()`
- [Line 1180](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1180): `private Task<string> SnapshotScheduleAsync() =>`
- [Line 1197](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1197): `private async Task<Guid> EnrollAsOwnerAsync(Guid groupId, Guid studentId)`
- [Line 1220](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1220): `private Task<uint> GetEnrollmentVersionAsync(Guid enrollmentId) =>`
- [Line 1223](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1223): `private Task<string> SnapshotEnrollmentsAsync() =>`
- [Line 1227](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1227): `private Task<Guid> GetTwoCentreTeachersOwnNileGroupIdAsync() =>`
- [Line 1263](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1263): `private Task<string> SnapshotGroupAsync(Guid id) =>`
- [Line 1267](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1267): `private Task<long> CountGroupsAsync() =>`
- [Line 1275](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1275): `private static string UniquePhone(string prefix) => prefix + Interlocked.Increment(ref _nextPhoneSuffix).ToString(CultureInfo.InvariantCulture);`
- [Line 1277](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1277): `private Task<long> CountStudentsAsync() =>`
- [Line 1306](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1306): `private async Task<Guid> RegisterSecondParentAsOwnerAsync()`
- [Line 1334](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1334): `private Task<string> SnapshotStudentAsync(Guid id) =>`
- [Line 1337](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1337): `private Task<long> CountParentsAsync() =>`
- [Line 1352](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1352): `private Task<string> SnapshotParentAsync(Guid id) =>`
- [Line 1355](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1355): `private Task<string> SnapshotNileCentreAsync() =>`
- [Line 1358](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1358): `private async Task<uint> GetNileCentreVersionAsync()`
- [Line 1366](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1366): `private Task<SignedInClient> SignInAsync(string role) =>`
- [Line 1371](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1371): `private static async Task AssertPermissionDeniedAsync(HttpResponseMessage response)`
- [Line 1428](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1428): `private Task<string> SnapshotSubjectAsync(Guid id) =>`
- [Line 1431](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1431): `private Task<string> SnapshotMembershipAsync(Guid membershipId) =>`
- [Line 1434](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1434): `private async Task<int> CountSubjectsAsync()`
- [Line 1466](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1466): `private async Task<Guid> CreateEligibleTeacherUserAsOwnerAsync(string email)`
- [Line 1475](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1475): `private Task<string> SnapshotTeacherAsync(Guid id) =>`
- [Line 1478](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1478): `private Task<long> CountTeachersAsync() =>`
- [Line 1481](../../../../../tests/TutoringCentre.Api.Tests/Authorization/PermissionMatrixTests.cs#L1481): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Authorization/EndpointInventoryTests.cs:43](../../../../../tests/TutoringCentre.Api.Tests/Authorization/EndpointInventoryTests.cs#L43)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Globalization;
using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Authorization;

/// <summary>
/// Task 30.8 (extended Task 32.11, Task 43.14, Task 45.9, Task 49.9, Task 53.4): one table of role x endpoint ->
/// expected status, executed over real HTTP against Nile's three roles, now including the seven teacher endpoints
/// (21 more literal cases), the eight group endpoints (24 more), the five enrollment endpoints (15 more) and the six
/// schedule endpoints (18 more). The matrix records the permission decision only: Teacher is allowed on the two
/// group reads, the two enrollment reads and the two schedule reads (and reads a group it owns here), while what a
/// teacher may see is ownership, proven separately by GroupTeacherScopeTests, EnrollmentTeacherScopeTests and
/// ScheduleTeacherScopeTests. The schedule cases run on a fixed application clock, so no case depends on today.
/// Allowed cases assert a non-403 success status;
/// denied cases assert 403 auth.permission_denied and an unchanged database, compared against the exact
/// pre-request state, not a snapshot taken after the request already ran. Values are written literally here,
/// never derived from <c>RolePermissions</c> — that would prove the matrix agrees with itself, not with the
/// contract.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class PermissionMatrixTests(ApiFactory factory) : IAsyncLifetime
{
    /// <summary>The fourteen routes this matrix classifies, in the exact raw pattern <see cref="EndpointInventoryTests"/> reads off the running app.</summary>
    internal static readonly string[] MatrixRoutes =
    [
        "GET /api/subjects",
        "GET /api/subjects/{id:guid}",
        "POST /api/subjects",
        "PUT /api/subjects/{id:guid}",
        "POST /api/subjects/{id:guid}/archive",
        "POST /api/subjects/{id:guid}/restore",
        "GET /api/staff",
        "POST /api/staff",
        "PUT /api/staff/{membershipId:guid}/role",
        "POST /api/staff/{membershipId:guid}/deactivate",
        "POST /api/staff/{membershipId:guid}/reactivate",
        "GET /api/audit",
        "GET /api/centre/settings",
        "PUT /api/centre/settings",
        "POST /api/parents",
        "GET /api/parents/{id:guid}",
        "PUT /api/parents/{id:guid}",
        "POST /api/parents/lookup",
        "GET /api/students",
        "GET /api/students/{id:guid}",
        "POST /api/students",
        "PUT /api/students/{id:guid}",
        "POST /api/students/{id:guid}/archive",
        "POST /api/students/{id:guid}/restore",
        "POST /api/students/{id:guid}/parents",
        "POST /api/students/{id:guid}/parents/{parentId:guid}/unlink",
        "POST /api/students/{id:guid}/parents/{parentId:guid}/make-primary",
        "GET /api/teachers",
        "GET /api/teachers/{id:guid}",
        "POST /api/teachers",
        "PUT /api/teachers/{id:guid}",
        "PUT /api/teachers/{id:guid}/account",
        "POST /api/teachers/{id:guid}/archive",
        "POST /api/teachers/{id:guid}/restore",
        "GET /api/groups",
        "GET /api/groups/{id:guid}",
        "POST /api/groups",
        "PUT /api/groups/{id:guid}",
        "PUT /api/groups/{id:guid}/capacity",
        "PUT /api/groups/{id:guid}/teacher",
        "POST /api/groups/{id:guid}/archive",
        "POST /api/groups/{id:guid}/restore",
        "POST /api/groups/{groupId:guid}/enrollments",
        "GET /api/groups/{groupId:guid}/roster",
        "GET /api/students/{studentId:guid}/enrollments",
        "POST /api/enrollments/{id:guid}/withdraw",
        "POST /api/enrollments/{id:guid}/transfer",
        "GET /api/groups/{groupId:guid}/schedule-rules",
        "POST /api/groups/{groupId:guid}/schedule-rules",
        "POST /api/schedule-rules/{id:guid}/end",
        "POST /api/schedule/extend",
        "GET /api/timetable",
        "POST /api/sessions/{id:guid}/cancel",
    ];

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    // The schedule cases fix the clock; every other case runs on real time, as before.
```

The excerpt stops at line 90; use the original file for the remaining 1396 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
