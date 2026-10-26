using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Staff;

/// <summary>Task 30.7: POST /api/auth/change-password and the Day 30 first-login gate, through the full HTTP pipeline.</summary>
[Collection(ApiCollection.Name)]
public sealed class ChangePasswordHttpTests(ApiFactory factory) : IAsyncLifetime
{
    private static readonly Uri StaffUri = new("/api/staff", UriKind.Relative);
    private static readonly Uri ChangePasswordUri = new("/api/auth/change-password", UriKind.Relative);
    private static readonly Uri MeUri = new("/api/me", UriKind.Relative);
    private static readonly Uri SelectCentreUri = new("/api/session/centre", UriKind.Relative);
    private static readonly Uri SubjectsUri = new("/api/subjects", UriKind.Relative);
    private static readonly Uri LoginUri = new("/api/auth/login", UriKind.Relative);

    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
    }

    public Task DisposeAsync() => Task.CompletedTask;

    [Fact]
    public async Task WrongCurrentPassword_Returns422()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, ChangePasswordUri, new { currentPassword = "totally-wrong", newPassword = "a-decent-new-password" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.Equal("auth.current_password_invalid", body.RootElement.GetProperty("code").GetString());
    }

    [Fact]
    public async Task WeakNewPassword_Returns400WithErrorsNewPassword()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, ChangePasswordUri, new { currentPassword = ApiFactory.TestSeedPassword, newPassword = "short" });
        var body = await ReadJsonAsync(response);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("auth.password_too_weak", body.RootElement.GetProperty("code").GetString());
        Assert.True(body.RootElement.GetProperty("errors").GetProperty("newPassword").GetArrayLength() > 0);
    }

    [Fact]
    public async Task Success_ReissuesTheCookieAndTheOldCookiesNextRequestIs401()
    {
        // Two independent sessions for the same user — "two devices" — both signed in before the change.
        using var deviceA = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var deviceB = await SignInHelper.SignInAsNileOwnerAsync(factory);

        using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
            deviceA.Client, ChangePasswordUri, new { currentPassword = ApiFactory.TestSeedPassword, newPassword = "a-brand-new-password-1" });
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Contains(response.Headers.GetValues("Set-Cookie"), c => c.StartsWith("__Host-tcm.session=", StringComparison.Ordinal));

        using var meOnDeviceA = await deviceA.Client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.OK, meOnDeviceA.StatusCode);

        // SessionValidation:CacheDuration is zero in tests, so deviceB's very next request already re-checks.
        using var meOnDeviceB = await deviceB.Client.GetAsync(MeUri);
        Assert.Equal(HttpStatusCode.Unauthorized, meOnDeviceB.StatusCode);
    }

    [Fact]
    public async Task SixthAttemptInAMinute_Returns429()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        const string partition = "change-password-rate-limit";

        for (var attempt = 1; attempt <= 5; attempt++)
        {
            using var response = await AntiforgeryTestHelper.PostAsJsonAsync(
                owner.Client, ChangePasswordUri, new { currentPassword = "wrong-on-purpose", newPassword = "a-decent-new-password" }, partition);
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        using var sixth = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, ChangePasswordUri, new { currentPassword = "wrong-on-purpose", newPassword = "a-decent-new-password" }, partition);
        var body = await ReadJsonAsync(sixth);

        Assert.Equal(HttpStatusCode.TooManyRequests, sixth.StatusCode);
        Assert.Equal("auth.rate_limited", body.RootElement.GetProperty("code").GetString());
        Assert.True(sixth.Headers.TryGetValues("Retry-After", out _));
    }

    [Fact]
    public async Task FirstLoginGateSequence_FromCreateToCentreSelection()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, StaffUri, new { email = "gate-http@nile.test", displayName = "Gate Http", role = "teacher", preferredLocale = "en" });
        var temporaryPassword = (await ReadJsonAsync(createResponse)).RootElement.GetProperty("temporaryPassword").GetString()!;

        using var gated = factory.CreateClient(new Microsoft.AspNetCore.Mvc.Testing.WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost"),
        });
        using var login = await AntiforgeryTestHelper.PostAsJsonAsync(
            gated, LoginUri, new { email = "gate-http@nile.test", password = temporaryPassword });
        var loginBody = await ReadJsonAsync(login);
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        Assert.True(loginBody.RootElement.GetProperty("mustChangePassword").GetBoolean());
        Assert.Equal(JsonValueKind.Null, loginBody.RootElement.GetProperty("activeCentreId").ValueKind);
        var nileCentreId = loginBody.RootElement.GetProperty("memberships")[0].GetProperty("centreId").GetGuid();

        using var selectAttempt = await AntiforgeryTestHelper.PostAsJsonAsync(gated, SelectCentreUri, new { centreId = nileCentreId });
        var selectBody = await ReadJsonAsync(selectAttempt);
        Assert.Equal(HttpStatusCode.Forbidden, selectAttempt.StatusCode);
        Assert.Equal("auth.password_change_required", selectBody.RootElement.GetProperty("code").GetString());

        using var subjectsAttempt = await gated.GetAsync(SubjectsUri);
        var subjectsBody = await ReadJsonAsync(subjectsAttempt);
        Assert.Equal(HttpStatusCode.Forbidden, subjectsAttempt.StatusCode);
        Assert.Equal("tenant.not_selected", subjectsBody.RootElement.GetProperty("code").GetString());

        using var changePassword = await AntiforgeryTestHelper.PostAsJsonAsync(
            gated, ChangePasswordUri, new { currentPassword = temporaryPassword, newPassword = "a-genuinely-new-password" });
        Assert.Equal(HttpStatusCode.NoContent, changePassword.StatusCode);

        using var selectAfterChange = await AntiforgeryTestHelper.PostAsJsonAsync(gated, SelectCentreUri, new { centreId = nileCentreId });
        Assert.Equal(HttpStatusCode.NoContent, selectAfterChange.StatusCode);

        using var oldPasswordLogin = await AntiforgeryTestHelper.PostAsJsonAsync(
            gated, LoginUri, new { email = "gate-http@nile.test", password = temporaryPassword });
        Assert.Equal(HttpStatusCode.Unauthorized, oldPasswordLogin.StatusCode);
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
