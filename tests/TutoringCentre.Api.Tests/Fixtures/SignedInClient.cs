using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TutoringCentre.Api.Tests.Fixtures;

/// <summary>
/// An HTTP client signed in as a specific seeded user through the real login endpoint — its own cookies, its
/// own session. Feature-agnostic: knows nothing about subjects or any other module. Never shared between tests
/// or users; each sign-in gets its own client.
/// </summary>
internal sealed class SignedInClient(HttpClient client, Guid? centreId) : IDisposable
{
    private static readonly Uri MeUri = new("/api/me", UriKind.Relative);
    private static readonly Uri SelectCentreUri = new("/api/session/centre", UriKind.Relative);

    public HttpClient Client { get; } = client;

    /// <summary>The actor's active centre after sign-in (and, if requested, centre selection) — null if none is active.</summary>
    public Guid? CentreId { get; private set; } = centreId;

    /// <summary>Switches the active centre on this same session (same cookies, same client) — proves a later request on this client reflects the switch, not a fresh login.</summary>
    public async Task SwitchCentreAsync(string centreSlug)
    {
        using var meResponse = await Client.GetAsync(MeUri);
        var text = await meResponse.Content.ReadAsStringAsync();
        using var me = JsonDocument.Parse(text);
        var targetCentreId = SignInHelper.FindMembershipCentreId(me, centreSlug)
            ?? throw new InvalidOperationException($"No membership in centre '{centreSlug}'.");

        using var selectResponse = await AntiforgeryTestHelper.PostAsJsonAsync(Client, SelectCentreUri, new { centreId = targetCentreId });
        if (selectResponse.StatusCode != HttpStatusCode.NoContent)
        {
            throw new InvalidOperationException($"Switching to centre '{centreSlug}' failed with {selectResponse.StatusCode}.");
        }

        CentreId = targetCentreId;
    }

    public void Dispose() => Client.Dispose();
}

/// <summary>Signs a fresh client in as a seeded user via the real login (and, when asked, centre-selection) endpoints.</summary>
internal static class SignInHelper
{
    private static readonly Uri LoginUri = new("/api/auth/login", UriKind.Relative);
    private static readonly Uri SelectCentreUri = new("/api/session/centre", UriKind.Relative);
    private static readonly Uri ChangePasswordUri = new("/api/auth/change-password", UriKind.Relative);

    public static Task<SignedInClient> SignInAsNileOwnerAsync(ApiFactory factory) =>
        SignInAsync(factory, "owner@nile.test");

    public static Task<SignedInClient> SignInAsMaadiOwnerAsync(ApiFactory factory) =>
        SignInAsync(factory, "owner@maadi.test");

    public static Task<SignedInClient> SignInAsNileSecretaryAsync(ApiFactory factory) =>
        SignInAsync(factory, "secretary@nile.test");

    /// <summary>The two-centre teacher has no auto-selected centre after login; <paramref name="centreSlug"/> picks one explicitly.</summary>
    public static Task<SignedInClient> SignInAsTwoCentreTeacherAsync(ApiFactory factory, string centreSlug) =>
        SignInAsync(factory, "teacher@both.test", centreSlug);

    /// <summary>Signs in as any seeded user; optionally selects a centre by slug when the account has more than one membership.</summary>
    public static async Task<SignedInClient> SignInAsync(ApiFactory factory, string email, string? selectCentreSlug = null)
    {
        ArgumentNullException.ThrowIfNull(factory);

        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        using var loginResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, LoginUri, new { email, password = ApiFactory.TestSeedPassword });
        if (loginResponse.StatusCode != HttpStatusCode.OK)
        {
            client.Dispose();
            throw new InvalidOperationException($"Sign-in as '{email}' failed with {loginResponse.StatusCode}.");
        }

        using var me = await ReadJsonAsync(loginResponse);
        Guid? centreId = me.RootElement.GetProperty("activeCentreId").ValueKind == JsonValueKind.String
            ? me.RootElement.GetProperty("activeCentreId").GetGuid()
            : null;

        if (selectCentreSlug is not null)
        {
            centreId = FindMembershipCentreId(me, selectCentreSlug)
                ?? throw new InvalidOperationException($"'{email}' has no membership in centre '{selectCentreSlug}'.");

            using var selectResponse = await AntiforgeryTestHelper.PostAsJsonAsync(client, SelectCentreUri, new { centreId });
            if (selectResponse.StatusCode != HttpStatusCode.NoContent)
            {
                client.Dispose();
                throw new InvalidOperationException(
                    $"Selecting centre '{selectCentreSlug}' for '{email}' failed with {selectResponse.StatusCode}.");
            }
        }

        return new SignedInClient(client, centreId);
    }

    /// <summary>
    /// Creates a brand-new Teacher-role member of the owner's centre through the real POST /api/staff, then signs
    /// that account in with its temporary password, completes the forced password change and selects the centre
    /// — the same path a real new teacher takes. Returns the signed-in client and the new user's ID.
    /// </summary>
    public static async Task<(SignedInClient Client, Guid UserId)> CreateAndSignInNewTeacherAsync(
        ApiFactory factory, SignedInClient owner, string email)
    {
        ArgumentNullException.ThrowIfNull(factory);
        ArgumentNullException.ThrowIfNull(owner);

        using var createResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            owner.Client, new Uri("/api/staff", UriKind.Relative), new { email, displayName = "New Teacher", role = "teacher", preferredLocale = "en" });
        if (createResponse.StatusCode != HttpStatusCode.Created)
        {
            throw new InvalidOperationException($"Creating staff member '{email}' failed with {createResponse.StatusCode}.");
        }

        using var created = await ReadJsonAsync(createResponse);
        var userId = created.RootElement.GetProperty("userId").GetGuid();
        var temporaryPassword = created.RootElement.GetProperty("temporaryPassword").GetString()!;

        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { BaseAddress = new Uri("https://localhost") });
        using var loginResponse = await AntiforgeryTestHelper.PostAsJsonAsync(client, LoginUri, new { email, password = temporaryPassword });
        using var changeResponse = await AntiforgeryTestHelper.PostAsJsonAsync(
            client, ChangePasswordUri, new { currentPassword = temporaryPassword, newPassword = $"{Guid.NewGuid():N}-New-1!" });
        using var selectResponse = await AntiforgeryTestHelper.PostAsJsonAsync(client, SelectCentreUri, new { centreId = owner.CentreId });
        if (loginResponse.StatusCode != HttpStatusCode.OK
            || changeResponse.StatusCode != HttpStatusCode.NoContent
            || selectResponse.StatusCode != HttpStatusCode.NoContent)
        {
            client.Dispose();
            throw new InvalidOperationException(
                $"Signing in new teacher '{email}' failed: login {loginResponse.StatusCode}, change password {changeResponse.StatusCode}, "
                + $"select centre {selectResponse.StatusCode}.");
        }

        return (new SignedInClient(client, owner.CentreId), userId);
    }

    internal static Guid? FindMembershipCentreId(JsonDocument me, string centreSlug)
    {
        foreach (var membership in me.RootElement.GetProperty("memberships").EnumerateArray())
        {
            if (membership.GetProperty("centreSlug").GetString() == centreSlug)
            {
                return membership.GetProperty("centreId").GetGuid();
            }
        }

        return null;
    }

    private static async Task<JsonDocument> ReadJsonAsync(HttpResponseMessage response)
    {
        var text = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(text);
    }
}
