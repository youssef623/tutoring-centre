using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Cli;

[Collection(ApiCollection.Name)]
public sealed class SeedCommandTests(ApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData("Development")]
    [InlineData("Testing")]
    public void SeedGate_OutsideProduction_AlwaysAllowed(string environmentName) =>
        Assert.True(SeedGate.IsAllowed(FakeEnvironment(environmentName), new DemoOptions { Enabled = false }));

    [Fact]
    public void SeedGate_Production_WithDemoDisabled_IsRefused() =>
        Assert.False(SeedGate.IsAllowed(FakeEnvironment("Production"), new DemoOptions { Enabled = false }));

    [Fact]
    public void SeedGate_Production_WithDemoEnabled_IsAllowed() =>
        Assert.True(SeedGate.IsAllowed(FakeEnvironment("Production"), new DemoOptions { Enabled = true }));

    private static FakeHostEnvironment FakeEnvironment(string environmentName) => new(environmentName);

    private sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "TutoringCentre.Api.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheTwoDemoCentres()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(2, await factory.ScalarAsync<long>("select count(*) from platform.centres"));
        Assert.Equal(
            "maadi-hub,nile-centre",
            await factory.ScalarAsync<string>("select string_agg(slug, ',' order by slug) from platform.centres"));
    }

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheStaffSetIdempotently()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(5, await factory.ScalarAsync<long>("select count(*) from identity.users"));
        Assert.Equal(6, await factory.ScalarAsync<long>("select count(*) from identity.memberships"));
        Assert.Equal(
            1,
            await factory.ScalarAsync<long>("select count(*) from identity.memberships where status = 'inactive'"));
    }

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoSubjectsIdempotently()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(5, await factory.ScalarAsync<long>("select count(*) from academics.subjects"));
        Assert.Equal(
            3,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.subjects s
                join platform.centres c on c.id = s.centre_id
                where c.slug = 'nile-centre'
                """));
        Assert.Equal(
            2,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.subjects s
                join platform.centres c on c.id = s.centre_id
                where c.slug = 'maadi-hub'
                """));
    }

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoStudentsAndParentsIdempotently()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(18, await factory.ScalarAsync<long>("select count(*) from academics.students"));
        Assert.Equal(15, await factory.ScalarAsync<long>("select count(*) from academics.parents"));
        Assert.Equal(
            12,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.students s
                join platform.centres c on c.id = s.centre_id
                where c.slug = 'nile-centre'
                """));
        Assert.Equal(
            9,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.parents p
                join platform.centres c on c.id = p.centre_id
                where c.slug = 'nile-centre'
                """));
        Assert.Equal(
            6,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.students s
                join platform.centres c on c.id = s.centre_id
                where c.slug = 'maadi-hub'
                """));
        Assert.Equal(
            6,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.parents p
                join platform.centres c on c.id = p.centre_id
                where c.slug = 'maadi-hub'
                """));

        // Every student-parent link's own centre_id matches both the student's and the parent's — never a row
        // that could only exist by crossing the tenant boundary the composite foreign keys enforce.
        Assert.Equal(
            0,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.student_parents sp
                join academics.students s on s.id = sp.student_id
                join academics.parents p on p.id = sp.parent_id
                where sp.centre_id != s.centre_id or sp.centre_id != p.centre_id
                """));

        // Every seeded phone is a synthetic, reserved-looking value, never one a real person could hold.
        Assert.Equal(
            0,
            await factory.ScalarAsync<long>(
                "select count(*) from academics.parents where phone !~ '^\\+2010000000[0-9]{2}$'"));
    }
}
