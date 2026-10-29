using System.Net;
using TutoringCentre.Api.Tests.Fixtures;
using static TutoringCentre.Api.Tests.Fixtures.AttendanceHttp;

namespace TutoringCentre.Api.Tests.Attendance;

[Collection(ApiCollection.Name)]
public sealed class ScanRateLimitTests(ApiFactory factory) : CardScanTestBase(factory)
{
    [Fact]
    public async Task SixtyPerUser_SixtyFirst429_OtherUserSucceeds_SameIp()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(Factory);
        var scenario = await Arrange(owner.Client);
        var card = await Issue(owner.Client, scenario.Students[0]);
        using var first = await NewSecretary(owner);
        using var other = await NewSecretary(owner);
        Factory.Time.FixAt(InSession);
        var token = card.GetProperty("token").GetString()!;
        // Pre-fetch CSRF once, and send all 61 within one real limiter window.
        var csrf = await AntiforgeryTestHelper.GetCsrfTokenAsync(first.Client);
        async Task<HttpResponseMessage> Send()
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, $"/api/sessions/{scenario.SessionId}/scans")
            { Content = System.Net.Http.Json.JsonContent.Create(new { token }) };
            request.Headers.Add("X-XSRF-TOKEN", csrf);
            return await first.Client.SendAsync(request);
        }
        var responses = await Task.WhenAll(Enumerable.Range(0, 60).Select(_ => Send()));
        foreach (var response in responses)
        {
            using (response) Assert.True(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.Conflict,
                $"Unexpected {response.StatusCode}: {await response.Content.ReadAsStringAsync()}");
        }
        using var limited = await Send();
        await Problem(limited, HttpStatusCode.TooManyRequests, "auth.rate_limited");
        Assert.NotNull(limited.Headers.RetryAfter);
        using var independent = await Scan(other.Client, scenario.SessionId, token);
        Assert.Equal(HttpStatusCode.OK, independent.StatusCode);
    }

    private async Task<SignedInClient> NewSecretary(SignedInClient owner)
    {
        var email = $"scan-{Guid.NewGuid():N}@example.test";
        using var created = await Post(owner.Client, "/api/staff", new { email, displayName = "Scan operator", role = "secretary", preferredLocale = "en" });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);
        var password = (await Json(created)).GetProperty("temporaryPassword").GetString()!;
        var client = Factory.CreateClient(new() { BaseAddress = new Uri("https://localhost") });
        using var login = await Post(client, "/api/auth/login", new { email, password });
        Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        using var changed = await Post(client, "/api/auth/change-password", new { currentPassword = password, newPassword = $"{Guid.NewGuid():N}-Changed-1!" });
        Assert.Equal(HttpStatusCode.NoContent, changed.StatusCode);
        using var selected = await Post(client, "/api/session/centre", new { centreId = owner.CentreId });
        Assert.Equal(HttpStatusCode.NoContent, selected.StatusCode);
        return new SignedInClient(client, owner.CentreId);
    }
}
