# `tests/TutoringCentre.Api.Tests/Http/DataProtectionPersistenceTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Proves the key ring is shared, not per-process (Task 34.6): a cookie minted by one application instance is accepted by a second, independent instance that only shares the same database — exactly what two container replicas, or one replica before and after a restart, look like. Without persistence this would 401, because each instance would have generated its own, different key ring in memory.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Http/DataProtectionPersistenceTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 69 lines and 3,341 bytes. Source hash: `1bfc5038d6b6d1c2110b18f9891a79f91fe200e4dbb02f9e7c377fd9c5e6b4a2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using System.Net;`
- Line 3: `using Microsoft.AspNetCore.Hosting;`
- Line 4: `using Microsoft.AspNetCore.Mvc.Testing;`
- Line 5: `using TutoringCentre.Api.Cli;`
- Line 6: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 20](../../../../../tests/TutoringCentre.Api.Tests/Http/DataProtectionPersistenceTests.cs#L20): `public sealed class DataProtectionPersistenceTests : IAsyncLifetime`
- [Line 30](../../../../../tests/TutoringCentre.Api.Tests/Http/DataProtectionPersistenceTests.cs#L30): `public async Task InitializeAsync()`
- [Line 36](../../../../../tests/TutoringCentre.Api.Tests/Http/DataProtectionPersistenceTests.cs#L36): `public async Task DisposeAsync()`
- [Line 43](../../../../../tests/TutoringCentre.Api.Tests/Http/DataProtectionPersistenceTests.cs#L43): `public async Task CookieFromOneInstance_IsAcceptedByASecondInstance_OnTheSameDatabase()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Http;

/// <summary>
/// Proves the key ring is shared, not per-process (Task 34.6): a cookie minted by one application instance is
/// accepted by a second, independent instance that only shares the same database — exactly what two container
/// replicas, or one replica before and after a restart, look like. Without persistence this would 401, because
/// each instance would have generated its own, different key ring in memory.
/// </summary>
[SuppressMessage(
    "Design",
    "CA1001:Types that own disposable fields should be disposable",
    Justification = "xUnit's IAsyncLifetime.DisposeAsync disposes both factories; a synchronous IDisposable here would double-dispose them.")]
public sealed class DataProtectionPersistenceTests : IAsyncLifetime
{
    private static readonly Uri LoginUri = new("/api/auth/login", UriKind.Relative);
    private static readonly Uri MeUri = new("/api/me", UriKind.Relative);

    // Its own instance (not the shared ApiCollection fixture): this test specifically needs two application
    // hosts pointed at one database, which no shared single-factory fixture can give it.
    private readonly ApiFactory _instanceA = new();
    private WebApplicationFactory<Program>? _instanceB;

    public async Task InitializeAsync()
    {
        await ((IAsyncLifetime)_instanceA).InitializeAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(_instanceA.Services));
    }

    public async Task DisposeAsync()
    {
        _instanceB?.Dispose();
        await _instanceA.DisposeAsync();
    }

    [Fact]
    public async Task CookieFromOneInstance_IsAcceptedByASecondInstance_OnTheSameDatabase()
    {
        using var clientA = _instanceA.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        using var loginResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            clientA,
            LoginUri,
            new { email = "owner@nile.test", password = ApiFactory.TestSeedPassword });
        Assert.Equal(HttpStatusCode.OK, loginResponse.StatusCode);
        var sessionCookie = Assert.Single(loginResponse.Headers.GetValues("Set-Cookie")).Split(';')[0];

        _instanceB = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseEnvironment("Testing");
            builder.UseSetting("ConnectionStrings:Postgres", _instanceA.AppConnectionString);
            builder.UseSetting("ConnectionStrings:PostgresMigrations", _instanceA.OwnerConnectionString);
            builder.UseSetting("Seed:Password", ApiFactory.TestSeedPassword);
            builder.UseSetting("SessionValidation:CacheDuration", "00:00:00");
        });
        using var clientB = _instanceB.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        using var meRequest = new HttpRequestMessage(HttpMethod.Get, MeUri);
        meRequest.Headers.Add("Cookie", sessionCookie);

        using var meResponse = await clientB.SendAsync(meRequest);

        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
