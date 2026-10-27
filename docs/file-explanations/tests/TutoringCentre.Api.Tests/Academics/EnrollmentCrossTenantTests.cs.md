# `tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 49.7: tenant isolation for enrollment at the HTTP boundary, every case in both directions. Enrollment joins three resources (group, student, enrollment), so each of the three is the foreign one in at least one case, and mixed cases put one foreign ID inside an otherwise own request — the "everything foreign" case alone would be stopped by the group lookup and prove nothing about the student or target-group lookups. Each case answers the 404 of the first foreign thing the handler meets (group before student; enrollment before target group), and the rows of both centres are compared before and after through the superuser connection.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 254 lines and 15,323 bytes. Source hash: `032a80f535d13587c41a945ceda0661460b77fedc417cd61ad2784a317684be0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Globalization;`
- Line 2: `using System.Net;`
- Line 3: `using System.Text.Json;`
- Line 4: `using TutoringCentre.Api.Cli;`
- Line 5: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 18](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L18): `public sealed class EnrollmentCrossTenantTests(ApiFactory factory) : IAsyncLifetime`
- [Line 22](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L22): `public async Task InitializeAsync()`
- [Line 28](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L28): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 33](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L33): `public async Task Enroll_OwnGroupWithTheOtherCentresStudent_Returns404StudentNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)`
- [Line 49](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L49): `public async Task Enroll_TheOtherCentresGroupWithOwnStudent_Returns404GroupNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)`
- [Line 65](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L65): `public async Task Enroll_TheOtherCentresGroupWithThatCentresOwnStudent_Returns404GroupNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)`
- [Line 81](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L81): `public async Task Withdraw_TheOtherCentresEnrollmentWithItsRealVersion_Returns404EnrollmentNotFoundAndLeavesItActive(string attackerEmail, string victimEmail)`
- [Line 98](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L98): `public async Task Transfer_TheOtherCentresEnrollmentIntoOwnGroup_Returns404EnrollmentNotFoundAndChangesNeitherSide(string attackerEmail, string victimEmail)`
- [Line 116](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L116): `public async Task Transfer_TheOtherCentresEnrollmentWithinThatCentre_Returns404EnrollmentNotFoundAndChangesNothing(string attackerEmail, string victimEmail)`
- [Line 133](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L133): `public async Task Transfer_OwnEnrollmentIntoTheOtherCentresGroup_Returns404GroupNotFoundAndTheEnrollmentStaysActive(string attackerEmail, string victimEmail)`
- [Line 152](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L152): `public async Task Roster_OfTheOtherCentresGroup_Returns404GroupNotFound(string attackerEmail, string victimEmail)`
- [Line 167](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L167): `public async Task StudentEnrollments_OfTheOtherCentresStudent_Returns404StudentNotFound(string attackerEmail, string victimEmail)`
- [Line 180](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L180): `private static async Task<Guid> CreateGroupAsync(HttpClient client, string prefix)`
- [Line 196](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L196): `private static Task<Guid> RegisterStudentAsync(HttpClient client, string fullName) =>`
- [Line 205](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L205): `private static Task<Guid> EnrollAsync(HttpClient client, Guid groupId, Guid studentId) =>`
- [Line 208](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L208): `private static async Task<Guid> CreateAsync(HttpClient client, string url, object body)`
- [Line 215](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L215): `private static async Task<uint> GetRosterVersionAsync(HttpClient client, Guid groupId, Guid enrollmentId)`
- [Line 225](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L225): `private Task<string> SnapshotGroupAsync(Guid groupId) =>`
- [Line 235](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L235): `private Task<string> SnapshotStudentAsync(Guid studentId) =>`
- [Line 244](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L244): `private Task<string> EnrollmentStatusAsync(Guid enrollmentId) =>`
- [Line 247](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L247): `private static string UniquePhone() => "015" + Interlocked.Increment(ref _nextPhoneSuffix).ToString(CultureInfo.InvariantCulture);`
- [Line 249](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L249): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

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
/// Task 49.7: tenant isolation for enrollment at the HTTP boundary, every case in both directions. Enrollment joins
/// three resources (group, student, enrollment), so each of the three is the foreign one in at least one case,
/// and mixed cases put one foreign ID inside an otherwise own request — the "everything foreign" case alone would
/// be stopped by the group lookup and prove nothing about the student or target-group lookups. Each case answers
/// the 404 of the first foreign thing the handler meets (group before student; enrollment before target group),
/// and the rows of both centres are compared before and after through the superuser connection.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class EnrollmentCrossTenantTests(ApiFactory factory) : IAsyncLifetime
{
    private static int _nextPhoneSuffix = 40000000;

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Enroll_OwnGroupWithTheOtherCentresStudent_Returns404StudentNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerGroupId = await CreateGroupAsync(attacker.Client, "Attacker");
        var victimStudentId = await RegisterStudentAsync(victim.Client, "Victim Student");

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, $"/api/groups/{attackerGroupId}/enrollments", new { studentId = victimStudentId },
            HttpStatusCode.NotFound, "student.not_found",
            () => SnapshotGroupAsync(attackerGroupId), () => SnapshotStudentAsync(victimStudentId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Enroll_TheOtherCentresGroupWithOwnStudent_Returns404GroupNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var attackerStudentId = await RegisterStudentAsync(attacker.Client, "Attacker Student");
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim");

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, $"/api/groups/{victimGroupId}/enrollments", new { studentId = attackerStudentId },
            HttpStatusCode.NotFound, "group.not_found",
            () => SnapshotStudentAsync(attackerStudentId), () => SnapshotGroupAsync(victimGroupId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Enroll_TheOtherCentresGroupWithThatCentresOwnStudent_Returns404GroupNotFoundAndWritesNoRow(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim");
        var victimStudentId = await RegisterStudentAsync(victim.Client, "Victim Student");

        await CrossTenantProbe.RunReferenceAsync(
            attacker.Client, HttpMethod.Post, $"/api/groups/{victimGroupId}/enrollments", new { studentId = victimStudentId },
            HttpStatusCode.NotFound, "group.not_found",
            () => SnapshotGroupAsync(victimGroupId), () => SnapshotStudentAsync(victimStudentId));
    }

    [Theory]
    [InlineData("owner@nile.test", "owner@maadi.test")]
    [InlineData("owner@maadi.test", "owner@nile.test")]
    public async Task Withdraw_TheOtherCentresEnrollmentWithItsRealVersion_Returns404EnrollmentNotFoundAndLeavesItActive(string attackerEmail, string victimEmail)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);
        var victimGroupId = await CreateGroupAsync(victim.Client, "Victim");
        var victimEnrollmentId = await EnrollAsync(victim.Client, victimGroupId, await RegisterStudentAsync(victim.Client, "Victim Student"));
        var version = await GetRosterVersionAsync(victim.Client, victimGroupId, victimEnrollmentId);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/enrollments/{victimEnrollmentId}/withdraw", new { version },
```

The excerpt stops at line 90; use the original file for the remaining 164 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
