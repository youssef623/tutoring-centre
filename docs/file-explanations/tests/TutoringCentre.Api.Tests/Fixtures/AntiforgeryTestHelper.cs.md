# `tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs`

## Purpose

Source summary (intent, not independent execution proof): Fetches and attaches an antiforgery token so tests exercise the real CSRF protection, never bypass it.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 51 lines and 2,796 bytes. Source hash: `80bbff828a0b271dfa91a31ca05ce14825e887a733515569e9aaef58bd54eaab`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Net.Http.Json;`
- Line 2: `using System.Text.Json;`
- Line 3: `using TutoringCentre.Api.Auth;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs#L8): `internal static class AntiforgeryTestHelper`
- [Line 12](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs#L12): `public static async Task<string> GetCsrfTokenAsync(HttpClient client)`
- [Line 27](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs#L27): `public static async Task<HttpResponseMessage> PostAsync(`
- [Line 37](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs#L37): `public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(`
- [Line 42](../../../../../tests/TutoringCentre.Api.Tests/Fixtures/AntiforgeryTestHelper.cs#L42): `public static async Task<HttpResponseMessage> SendAsJsonAsync<TValue>(`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs:210](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentCrossTenantTests.cs#L210)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs:739](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentEndpointTests.cs#L739)
- [tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs:92](../../../../../tests/TutoringCentre.Api.Tests/Academics/EnrollmentTeacherScopeTests.cs#L92)
- [tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs:52](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupCrossTenantTests.cs#L52)
- [tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs:39](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupEndpointTests.cs#L39)
- [tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs:97](../../../../../tests/TutoringCentre.Api.Tests/Academics/GroupTeacherScopeTests.cs#L97)
- [tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs:87](../../../../../tests/TutoringCentre.Api.Tests/Academics/LastSeatHttpTests.cs#L87)
- [tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs:37](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentCrossTenantProbeTests.cs#L37)
- [tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs:28](../../../../../tests/TutoringCentre.Api.Tests/Academics/ParentEndpointTests.cs#L28)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs:187](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleCrossTenantTests.cs#L187)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs:55](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleDoubleBookingHttpTests.cs#L55)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs:616](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleEndpointTests.cs#L616)
- [tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs:114](../../../../../tests/TutoringCentre.Api.Tests/Academics/ScheduleTeacherScopeTests.cs#L114)
- [tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs:127](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentCrossTenantProbeTests.cs#L127)
- [tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs:27](../../../../../tests/TutoringCentre.Api.Tests/Academics/StudentEndpointTests.cs#L27)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs:96](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectCrossTenantProbeTests.cs#L96)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs:43](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectEndpointTests.cs#L43)
- [tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs:55](../../../../../tests/TutoringCentre.Api.Tests/Academics/SubjectPermissionHttpTests.cs#L55)
- [tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs:46](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherCrossTenantTests.cs#L46)
- [tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs:27](../../../../../tests/TutoringCentre.Api.Tests/Academics/TeacherEndpointTests.cs#L27)
- 11 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Net.Http.Json;
using System.Text.Json;
using TutoringCentre.Api.Auth;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>Fetches and attaches an antiforgery token so tests exercise the real CSRF protection, never bypass it.</summary>
internal static class AntiforgeryTestHelper
{
    private static readonly Uri AntiforgeryUri = new("/api/auth/antiforgery", UriKind.Relative);

    public static async Task<string> GetCsrfTokenAsync(HttpClient client)
    {
        using var response = await client.GetAsync(AntiforgeryUri);
        var text = await response.Content.ReadAsStringAsync();
        using var body = JsonDocument.Parse(text);
        return body.RootElement.GetProperty("token").GetString()!;
    }

    /// <summary>
    /// POSTs with a freshly fetched token attached. Pass <paramref name="content"/> null for a bodyless POST
    /// (e.g. logout). Each call gets its own random rate-limit partition (see
    /// <see cref="LoginRateLimiting.TestPartitionHeaderName"/>) unless <paramref name="rateLimitPartition"/> is
    /// given explicitly — so ordinary tests never share a login rate-limit window with each other, and only a
    /// test that deliberately wants to trigger the limit passes the same partition value on every call.
    /// </summary>
    public static async Task<HttpResponseMessage> PostAsync(
        HttpClient client, Uri uri, HttpContent? content, string? rateLimitPartition = null)
    {
        var token = await GetCsrfTokenAsync(client);
        using var request = new HttpRequestMessage(HttpMethod.Post, uri) { Content = content };
        request.Headers.Add(AntiforgerySetup.HeaderName, token);
        request.Headers.Add(LoginRateLimiting.TestPartitionHeaderName, rateLimitPartition ?? Guid.NewGuid().ToString());
        return await client.SendAsync(request);
    }

    public static Task<HttpResponseMessage> PostAsJsonAsync<TValue>(
        HttpClient client, Uri uri, TValue value, string? rateLimitPartition = null) =>
        PostAsync(client, uri, JsonContent.Create(value), rateLimitPartition);

    /// <summary>Same as <see cref="PostAsync"/> but for any unsafe method (e.g. PUT) that needs the antiforgery header.</summary>
    public static async Task<HttpResponseMessage> SendAsJsonAsync<TValue>(
        HttpClient client, HttpMethod method, Uri uri, TValue value, string? rateLimitPartition = null)
    {
        var token = await GetCsrfTokenAsync(client);
        using var request = new HttpRequestMessage(method, uri) { Content = JsonContent.Create(value) };
        request.Headers.Add(AntiforgerySetup.HeaderName, token);
        request.Headers.Add(LoginRateLimiting.TestPartitionHeaderName, rateLimitPartition ?? Guid.NewGuid().ToString());
        return await client.SendAsync(request);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
