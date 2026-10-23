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

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoTeachersAndGroupsIdempotently()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(3, await factory.ScalarAsync<long>("select count(*) from academics.teachers"));
        Assert.Equal(6, await factory.ScalarAsync<long>("select count(*) from academics.groups"));
        Assert.Equal(
            "maadi-hub:1,nile-centre:2",
            await factory.ScalarAsync<string>(
                """
                select string_agg(slug || ':' || n, ',' order by slug) from (
                    select c.slug, count(*) as n from academics.teachers t
                    join platform.centres c on c.id = t.centre_id group by c.slug) per_centre
                """));
        Assert.Equal(
            "maadi-hub:2,nile-centre:4",
            await factory.ScalarAsync<string>(
                """
                select string_agg(slug || ':' || n, ',' order by slug) from (
                    select c.slug, count(*) as n from academics.groups g
                    join platform.centres c on c.id = g.centre_id group by c.slug) per_centre
                """));

        // Nile's four groups span all three seeded subjects, with the capacities the demo needs (two seats for
        // the last-seat demo). Every group is active and in the same centre as its subject and its teacher.
        Assert.Equal(
            "2,15,20,20",
            await factory.ScalarAsync<string>(
                """
                select string_agg(g.capacity::text, ',' order by g.capacity) from academics.groups g
                join platform.centres c on c.id = g.centre_id where c.slug = 'nile-centre'
                """));
        Assert.Equal(
            3,
            await factory.ScalarAsync<long>(
                """
                select count(distinct g.subject_id) from academics.groups g
                join platform.centres c on c.id = g.centre_id where c.slug = 'nile-centre'
                """));
        Assert.Equal(0, await factory.ScalarAsync<long>("select count(*) from academics.groups where status != 'active'"));
        Assert.Equal(
            0,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.groups g
                join academics.subjects s on s.id = g.subject_id
                join academics.teachers t on t.id = g.teacher_id
                where g.centre_id != s.centre_id or g.centre_id != t.centre_id
                """));

        // The two-centre teacher account is linked to exactly one teacher record per centre, and in Nile that
        // record owns exactly one group; the unlinked Nile teacher owns the rest.
        Assert.Equal(
            "maadi-hub,nile-centre",
            await factory.ScalarAsync<string>(
                """
                select string_agg(c.slug, ',' order by c.slug) from academics.teachers t
                join platform.centres c on c.id = t.centre_id
                join identity.users u on u.id = t.user_id
                where u.email = 'teacher@both.test'
                """));
        Assert.Equal(
            1,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.groups g
                join academics.teachers t on t.id = g.teacher_id
                join platform.centres c on c.id = g.centre_id
                where c.slug = 'nile-centre' and t.user_id is not null
                """));
        Assert.Equal(
            1,
            await factory.ScalarAsync<long>(
                "select count(*) from academics.teachers where user_id is null"));
    }
}
