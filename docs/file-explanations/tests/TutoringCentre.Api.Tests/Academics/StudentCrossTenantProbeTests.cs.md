# `tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 41.5: owner A cannot see or touch B's students, proven in both directions with CrossTenantProbe, using a real victim student and its real current version so a 409 could never mask a leak. Reference probes cover the second kind of attack: the attacker's own, legitimate URL with the victim's id planted inside it (a body field or a route segment), which an ordinary cross-tenant probe — always aimed at the victim's own URL — cannot exercise.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 158 lines and 9,181 bytes. Source hash: `a3438d46b3f84ae671dfa33f7270c48912b1e7f6fe207275aaf90fafd52c8555`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L16): `public sealed class StudentCrossTenantProbeTests(ApiFactory factory) : IAsyncLifetime`
- [Line 18](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L18): `public async Task InitializeAsync()`
- [Line 24](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L24): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 27](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L27): `public Task NileOwner_CannotSeeOrTouchMaadisStudents() =>`
- [Line 31](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L31): `public Task MaadiOwner_CannotSeeOrTouchNilesStudents() =>`
- [Line 35](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L35): `public Task NileOwner_ReferenceAttacks_CannotReachMaadisParentOrStudent() =>`
- [Line 39](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L39): `public Task MaadiOwner_ReferenceAttacks_CannotReachNilesParentOrStudent() =>`
- [Line 42](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L42): `private async Task RunDirectionAsync(string attackerEmail, string victimEmail, string victimParentPhone, string namePrefix)`
- [Line 92](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L92): `private async Task RunReferenceDirectionAsync(string attackerEmail, string victimEmail, string victimParentPhone)`
- [Line 153](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L153): `private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)`

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

/// <summary>
/// Task 41.5: owner A cannot see or touch B's students, proven in both directions with CrossTenantProbe, using
/// a real victim student and its real current version so a 409 could never mask a leak. Reference probes
/// cover the second kind of attack: the attacker's own, legitimate URL with the victim's id planted inside it
/// (a body field or a route segment), which an ordinary cross-tenant probe — always aimed at the victim's own
/// URL — cannot exercise.
/// </summary>
[Collection(ApiCollection.Name)]
public sealed class StudentCrossTenantProbeTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public Task NileOwner_CannotSeeOrTouchMaadisStudents() =>
        RunDirectionAsync(attackerEmail: "owner@nile.test", victimEmail: "owner@maadi.test", victimParentPhone: "01212340010", namePrefix: "Maadi Victim");

    [Fact]
    public Task MaadiOwner_CannotSeeOrTouchNilesStudents() =>
        RunDirectionAsync(attackerEmail: "owner@maadi.test", victimEmail: "owner@nile.test", victimParentPhone: "01512340011", namePrefix: "Nile Victim");

    [Fact]
    public Task NileOwner_ReferenceAttacks_CannotReachMaadisParentOrStudent() =>
        RunReferenceDirectionAsync(attackerEmail: "owner@nile.test", victimEmail: "owner@maadi.test", victimParentPhone: "01212340020");

    [Fact]
    public Task MaadiOwner_ReferenceAttacks_CannotReachNilesParentOrStudent() =>
        RunReferenceDirectionAsync(attackerEmail: "owner@maadi.test", victimEmail: "owner@nile.test", victimParentPhone: "01512340021");

    private async Task RunDirectionAsync(string attackerEmail, string victimEmail, string victimParentPhone, string namePrefix)
    {
        using var attacker = await SignInHelper.SignInAsync(factory, attackerEmail);
        using var victim = await SignInHelper.SignInAsync(factory, victimEmail);

        var (victimStudentId, _, victimVersion) = await RegisterAsync(victim.Client, $"{namePrefix} Student", $"{namePrefix} Parent", victimParentPhone);

        Task<string> SnapshotAsync() =>
            factory.ScalarAsync<string>($"select full_name || '|' || status || '|' || xmin from academics.students where id = '{victimStudentId}'");

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Get, $"/api/students/{victimStudentId}", body: null,
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Put, $"/api/students/{victimStudentId}",
            new { fullName = "Hacked Name", phone = (string?)null, schoolYear = (string?)null, version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/students/{victimStudentId}/archive",
            new { version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/students/{victimStudentId}/restore",
            new { version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/students/{victimStudentId}/parents",
            new { parentId = Guid.NewGuid(), relationship = "mother", version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/students/{victimStudentId}/parents/{Guid.NewGuid()}/unlink",
            new { version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        await CrossTenantProbe.RunAsync(
            attacker.Client, HttpMethod.Post, $"/api/students/{victimStudentId}/parents/{Guid.NewGuid()}/make-primary",
            new { version = victimVersion },
            HttpStatusCode.NotFound, "student.not_found", SnapshotAsync);

        using var listResponse = await attacker.Client.GetAsync(new Uri("/api/students?pageSize=100", UriKind.Relative));
        var listBody = await ReadJsonAsync(listResponse);
        var names = listBody.RootElement.GetProperty("items").EnumerateArray().Select(item => item.GetProperty("fullName").GetString()).ToList();
        Assert.DoesNotContain($"{namePrefix} Student", names);
    }
```

The excerpt stops at line 90; use the original file for the remaining 68 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
