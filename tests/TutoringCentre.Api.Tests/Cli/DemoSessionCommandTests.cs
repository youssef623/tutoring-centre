using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
using static TutoringCentre.Api.Tests.Fixtures.AttendanceHttp;

namespace TutoringCentre.Api.Tests.Cli;

[Collection(ApiCollection.Name)]
public sealed class DemoSessionCommandTests(ApiFactory factory) : IAsyncLifetime
{
    public async Task InitializeAsync()
    {
        await factory.ResetAsync();
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
        factory.Time.FixAt(BeforeSession);
    }
    public Task DisposeAsync() { factory.Time.UseRealTime(); return Task.CompletedTask; }

    [Fact]
    public async Task Development_CreatesTodaysOpenSessionThroughCommands_AndEndsRuleToday()
    {
        using var owner = await SignInHelper.SignInAsNileOwnerAsync(factory);
        var group = (await Get(owner.Client, "/api/groups")).GetProperty("items")[0];
        using var output = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
        var code = await DemoSessionCommand.RunAsync(factory.Services, new Environment("Development"),
            ["--centre", "nile-centre", "--group", group.GetProperty("name").GetString()!], output);
        Assert.Equal(0, code);
        var sessions = (await Get(owner.Client, "/api/attendance/sessions")).GetProperty("items");
        var session = Assert.Single(sessions.EnumerateArray());
        Assert.Equal("open", session.GetProperty("window").GetString());
        Assert.Contains(session.GetProperty("sessionId").GetGuid().ToString(), output.ToString(), StringComparison.Ordinal);
        var rules = (await Get(owner.Client, $"/api/groups/{group.GetProperty("id").GetGuid()}/schedule-rules")).GetProperty("items");
        var rule = Assert.Single(rules.EnumerateArray(), row => row.GetProperty("effectiveFrom").GetString() == Date);
        Assert.Equal(Date, rule.GetProperty("effectiveTo").GetString());
    }

    [Theory]
    [InlineData("Production")]
    [InlineData("Testing")]
    [InlineData("Staging")]
    public async Task OutsideDevelopment_RefusesBeforeDatabaseWork(string environment)
    {
        using var output = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(1, await DemoSessionCommand.RunAsync(factory.Services, new Environment(environment), [], output));
        Assert.Contains("only runs in Development", output.ToString(), StringComparison.Ordinal);
    }

    [Fact]
    public async Task LateNight_Refuses()
    {
        factory.Time.FixAt(new(2027, 4, 20, 21, 15, 0, TimeSpan.Zero));
        using var output = new StringWriter(System.Globalization.CultureInfo.InvariantCulture);
        Assert.Equal(1, await DemoSessionCommand.RunAsync(factory.Services, new Environment("Development"),
            ["--centre", "nile-centre", "--group", "unused"], output));
        Assert.Contains("23:00", output.ToString(), StringComparison.Ordinal);
    }

    private sealed class Environment(string name) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = name;
        public string ApplicationName { get; set; } = "Tests";
        public string ContentRootPath { get; set; } = string.Empty;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }
}
