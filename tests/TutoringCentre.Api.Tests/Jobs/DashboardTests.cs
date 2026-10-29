using System.Net;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Jobs;

[Collection(ApiCollection.Name)]
public sealed class DashboardTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri DashboardUri = new("/ops/jobs", UriKind.Relative);
    private static readonly Uri MeUri = new("/api/me", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }
    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task Dashboard_SignedOut_Returns401()
    {
        using var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        using var response = await client.GetAsync(DashboardUri);
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Theory]
    [InlineData("owner@nile.test")]
    [InlineData("secretary@nile.test")]
    public async Task Dashboard_UnlistedOwnerAndSecretary_Return403_AndMeFalse(string email)
    {
        using var signed = await SignInHelper.SignInAsync(factory, email);
        using var response = await signed.Client.GetAsync(DashboardUri);
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        using var me = await signed.Client.GetAsync(MeUri);
        Assert.False((await AttendanceHttp.Json(me)).GetProperty("isOperator").GetBoolean());
    }

    [Theory]
    [InlineData("owner@nile.test")]
    [InlineData("teacher@both.test")]
    public async Task Dashboard_NormalizedAllowlist_GrantsOperatorRegardlessOfCentreRole(string email)
    {
        await using var listed = WithOperator(email.ToUpperInvariant());
        using var signed = await SignInHelper.SignInAsync(listed, email);
        using var response = await signed.Client.GetAsync(DashboardUri);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Hangfire", await response.Content.ReadAsStringAsync(), StringComparison.Ordinal);
        using var me = await signed.Client.GetAsync(MeUri);
        Assert.True((await AttendanceHttp.Json(me)).GetProperty("isOperator").GetBoolean());
    }

    [Fact]
    public async Task Dashboard_OutsideDevelopment_RefusesOperatorPost()
    {
        await using var listed = WithOperator("owner@nile.test");
        using var signed = await SignInHelper.SignInAsync(listed, "owner@nile.test");
        using var response = await signed.Client.PostAsync(new Uri("/ops/jobs/jobs/enqueued/delete", UriKind.Relative), new StringContent(string.Empty));
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public void Dashboard_TestHost_HasNoHangfireServer()
    {
        Assert.DoesNotContain(factory.Services.GetServices<IHostedService>(), service =>
            service.GetType().Assembly.GetName().Name?.StartsWith("Hangfire", StringComparison.Ordinal) == true);
    }

    private WebApplicationFactory<Program> WithOperator(string email) => factory.WithWebHostBuilder(builder =>
        builder.UseSetting("Jobs:OperatorEmails:0", email));
}
