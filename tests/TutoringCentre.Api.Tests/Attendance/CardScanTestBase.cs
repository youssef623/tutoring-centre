using System.Net;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
using static TutoringCentre.Api.Tests.Fixtures.AttendanceHttp;

namespace TutoringCentre.Api.Tests.Attendance;

public abstract class CardScanTestBase(ApiFactory factory) : IAsyncLifetime
{
    protected ApiFactory Factory { get; } = factory;
    public async Task InitializeAsync()
    {
        await Factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(Factory.Services));
        Factory.Time.FixAt(BeforeSession);
    }
    public Task DisposeAsync() { Factory.Time.UseRealTime(); return Task.CompletedTask; }
    private protected static async Task<JsonElement> Issue(HttpClient client, Guid studentId)
    {
        using var response = await AntiforgeryTestHelper.PostAsync(client, new($"/api/students/{studentId}/card", UriKind.Relative), null);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal($"/api/students/{studentId}/card", response.Headers.Location!.OriginalString);
        Assert.True(response.Headers.CacheControl!.NoStore);
        return await Json(response);
    }
    private protected static Task<HttpResponseMessage> Scan(HttpClient client, Guid sessionId, string token) =>
        Post(client, $"/api/sessions/{sessionId}/scans", new { token });
}
