using System.Globalization;
using System.Text.Json;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;
namespace TutoringCentre.Api.Tests.Billing;

[Collection(ApiCollection.Name)]
public sealed class SeedLogPrivacyTests(ApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.ResetAsync();
    public Task DisposeAsync() => Task.CompletedTask;
    [Fact]
    public async Task SeedAndRepeatedSeedNeverLogNamesPhonesOrPasswords()
    {
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services)); Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
        var json = await factory.ScalarAsync<string>("""
            select json_agg(value)::text from (
              select full_name value from academics.students union select full_name from academics.parents
              union select full_name from academics.teachers union select email from identity.users
              union select phone from academics.parents where phone is not null
            ) person_data
            """);
        var patterns = JsonSerializer.Deserialize<string[]>(json)!; Assert.NotEmpty(patterns);
        var events = factory.AttendanceLogs.Snapshot(); Assert.NotEmpty(events);
        var rendered = string.Join('\n', events.Select(e => e.RenderMessage(CultureInfo.InvariantCulture) + e.ToString()));
        Assert.False(patterns.Append(SharedApiFactory.TestSeedPassword).Any(value => rendered.Contains(value, StringComparison.Ordinal)), "security.seed_log_person_data");
    }
}
