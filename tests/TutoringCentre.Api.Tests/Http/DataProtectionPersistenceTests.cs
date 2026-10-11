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
