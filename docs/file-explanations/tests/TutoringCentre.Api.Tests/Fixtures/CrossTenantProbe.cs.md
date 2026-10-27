# `tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbe.cs`

## Purpose

Source summary (intent, not independent execution proof): Attacks a victim's resource as a given (attacker) client and checks two things: the response itself, and that the victim's row — read through a caller-supplied snapshot, taken over the owner/superuser connection — is unchanged. Feature-agnostic: knows nothing about subjects or any other module, only HTTP and a snapshot function. A probe that checked only the status would prove nothing; this is why the snapshot is required, not optional.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbe.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 98 lines and 4,026 bytes. Source hash: `54e79155ecc5372ab80c4d8b32b0e7a9040cc8a2141237070fe7991f2a2bb66c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net;`
- Line 2: `using System.Net.Http.Json;`
- Line 3: `using System.Text.Json;`
- Line 4: `using TutoringCentre.Api.Auth;`
- Line 5: `using Xunit;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbe.cs#L16): `internal static class CrossTenantProbe`
- [Line 18](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbe.cs#L18): `public static async Task RunAsync(`
- [Line 39](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbe.cs#L39): `/// one id inside them — a route segment or a body field — names the VICTIM's record instead. An ordinary`
- [Line 45](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbe.cs#L45): `public static async Task RunReferenceAsync(`
- [Line 67](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbe.cs#L67): `private static async Task SendAndAssertAsync(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [db/bootstrap-roles.sh:36](../../../../../db/bootstrap-roles.sh#L36)
- [frontend/src/features/enrollments/SeatMeter.test.tsx:21](../../../../../frontend/src/features/enrollments/SeatMeter.test.tsx#L21)
- [frontend/src/routes/_authenticated/staff.test.tsx:147](../../../../../frontend/src/routes/_authenticated/staff.test.tsx#L147)
- [frontend/src/routes/change-password.test.tsx:131](../../../../../frontend/src/routes/change-password.test.tsx#L131)
- [frontend/src/test/render.tsx:18](../../../../../frontend/src/test/render.tsx#L18)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs:40](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L40)
- [tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs:40](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L40)
- [tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs:49](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L49)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs:46](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L46)
- [tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs:52](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L52)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs:67](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L67)
- [tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs:70](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L70)
- [tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbeSelfTests.cs:31](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/CrossTenantProbeSelfTests.cs#L31)
- [tests/TutoringCentre.Api.Tests/Staff/StaffCrossTenantProbeTests.cs:43](../../../../../tests/TutoringCentre.Api.Tests/Staff/StaffCrossTenantProbeTests.cs#L43)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using TutoringCentre.Api.Auth;
using Xunit;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>
/// Attacks a victim's resource as a given (attacker) client and checks two things: the response itself, and
/// that the victim's row — read through a caller-supplied snapshot, taken over the owner/superuser connection —
/// is unchanged. Feature-agnostic: knows nothing about subjects or any other module, only HTTP and a snapshot
/// function. A probe that checked only the status would prove nothing; this is why the snapshot is required,
/// not optional.
/// </summary>
internal static class CrossTenantProbe
{
    public static async Task RunAsync(
        HttpClient attackerClient,
        HttpMethod method,
        string url,
        object? body,
        HttpStatusCode expectedStatus,
        string? expectedCode,
        Func<Task<string>> snapshotVictimRowAsync)
    {
        ArgumentNullException.ThrowIfNull(snapshotVictimRowAsync);

        var before = await snapshotVictimRowAsync();

        await SendAndAssertAsync(attackerClient, method, url, body, expectedStatus, expectedCode);

        var after = await snapshotVictimRowAsync();
        Assert.Equal(before, after);
    }

    /// <summary>
    /// A reference-attack probe: the URL and body are the ATTACKER's own (a resource they genuinely own), but
    /// one id inside them — a route segment or a body field — names the VICTIM's record instead. An ordinary
    /// <see cref="RunAsync"/> cannot express this: that helper attacks the victim's URL directly and would
    /// always see 404 regardless of whether the reference itself was checked. Both rows are snapshotted and
    /// re-checked, because a reference attack could just as easily corrupt the attacker's own row (e.g. by
    /// linking it to someone else's child record) as the victim's.
    /// </summary>
    public static async Task RunReferenceAsync(
        HttpClient attackerClient,
        HttpMethod method,
        string url,
        object? body,
        HttpStatusCode expectedStatus,
        string? expectedCode,
        Func<Task<string>> snapshotAttackerRowAsync,
        Func<Task<string>> snapshotVictimRowAsync)
    {
        ArgumentNullException.ThrowIfNull(snapshotAttackerRowAsync);
        ArgumentNullException.ThrowIfNull(snapshotVictimRowAsync);

        var attackerBefore = await snapshotAttackerRowAsync();
        var victimBefore = await snapshotVictimRowAsync();

        await SendAndAssertAsync(attackerClient, method, url, body, expectedStatus, expectedCode);

        Assert.Equal(attackerBefore, await snapshotAttackerRowAsync());
        Assert.Equal(victimBefore, await snapshotVictimRowAsync());
    }

    private static async Task SendAndAssertAsync(
        HttpClient attackerClient, HttpMethod method, string url, object? body, HttpStatusCode expectedStatus, string? expectedCode)
    {
        ArgumentNullException.ThrowIfNull(attackerClient);
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(url);

        using var request = new HttpRequestMessage(method, url);
        if (body is not null)
        {
            request.Content = JsonContent.Create(body);
        }

        // Safe methods (GET) need no CSRF token; the real AntiforgeryEndpointFilter only checks unsafe ones.
        if (method != HttpMethod.Get)
        {
            var token = await AntiforgeryTestHelper.GetCsrfTokenAsync(attackerClient);
            request.Headers.Add(AntiforgerySetup.HeaderName, token);
        }

        using var response = await attackerClient.SendAsync(request);
        var text = await response.Content.ReadAsStringAsync();

        Assert.Equal(expectedStatus, response.StatusCode);
```

The excerpt stops at line 90; use the original file for the remaining 8 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
