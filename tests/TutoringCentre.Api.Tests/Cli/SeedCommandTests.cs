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

    [Theory]
    [InlineData("Development")]
    [InlineData("Testing")]
    public void SeedGate_OutsideProduction_IncludesTheSetupCentre(string environmentName) =>
        Assert.True(SeedGate.IncludesSetupCentre(FakeEnvironment(environmentName)));

    [Fact]
    public void SeedGate_Production_NeverIncludesTheSetupCentre_EvenOnADemoInstance()
    {
        Assert.True(SeedGate.IsAllowed(FakeEnvironment("Production"), new DemoOptions { Enabled = true }));
        Assert.False(SeedGate.IncludesSetupCentre(FakeEnvironment("Production")));
    }

    private static FakeHostEnvironment FakeEnvironment(string environmentName) => new(environmentName);

    private sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "TutoringCentre.Api.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheTwoDemoCentresAndTheEmptySetupCentre()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);

        Assert.Equal(2, await factory.ScalarAsync<long>("select count(*) from conversations.channel_accounts"));
        Assert.Equal("100000000000001,100000000000002", await factory.ScalarAsync<string>(
            "select string_agg(external_account_id, ',' order by external_account_id) from conversations.channel_accounts"));

        // The tests run as Testing, so the setup centre (Development and Testing only) is here too.
        Assert.Equal(3, await factory.ScalarAsync<long>("select count(*) from platform.centres"));
        Assert.Equal(
            "giza-new,maadi-hub,nile-centre",
            await factory.ScalarAsync<string>("select string_agg(slug, ',' order by slug) from platform.centres"));
    }

    [Fact]
    public async Task ChannelsRegister_DuplicateInOtherCentre_ReturnsFailureWithoutExtraAccount()
    {
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));
        Assert.Equal(1, await ChannelsCommand.RunAsync(factory.Services,
            ["register", "--centre", "maadi-hub", "--provider", "whatsapp", "--account-id", "100000000000001", "--display-phone", "+201000000093"]));
        Assert.Equal(2, await factory.ScalarAsync<long>("select count(*) from conversations.channel_accounts"));
        Assert.Equal(0, await ChannelsCommand.RunAsync(factory.Services,
            ["register", "--centre", "maadi-hub", "--provider", "whatsapp", "--account-id", "100000000000003", "--display-phone", "+201000000093"]));
        Assert.Equal(3, await factory.ScalarAsync<long>("select count(*) from conversations.channel_accounts"));
    }

    [Fact]
    public async Task RunAsync_LeavesTheSetupCentreEmpty_ExceptForItsOneOwner()
    {
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));

        Assert.Equal(
            "owner@giza.test:owner:active",
            await factory.ScalarAsync<string>(
                """
                select string_agg(u.email || ':' || m.role || ':' || m.status, ',') from identity.memberships m
                join identity.users u on u.id = m.user_id
                join platform.centres c on c.id = m.centre_id
                where c.slug = 'giza-new'
                """));
        Assert.Equal(
            0,
            await factory.ScalarAsync<long>(
                """
                select (select count(*) from academics.subjects t join platform.centres c on c.id = t.centre_id where c.slug = 'giza-new')
                     + (select count(*) from academics.teachers t join platform.centres c on c.id = t.centre_id where c.slug = 'giza-new')
                     + (select count(*) from academics.groups t join platform.centres c on c.id = t.centre_id where c.slug = 'giza-new')
                     + (select count(*) from academics.students t join platform.centres c on c.id = t.centre_id where c.slug = 'giza-new')
                     + (select count(*) from academics.parents t join platform.centres c on c.id = t.centre_id where c.slug = 'giza-new')
                     + (select count(*) from academics.schedule_rules t join platform.centres c on c.id = t.centre_id where c.slug = 'giza-new')
                """));
    }

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheStaffSetIdempotently()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        // The five demo users and their six memberships, plus the setup centre's one owner (Testing).
        Assert.Equal(6, await factory.ScalarAsync<long>("select count(*) from identity.users"));
        Assert.Equal(7, await factory.ScalarAsync<long>("select count(*) from identity.memberships"));
        Assert.Equal(
            5,
            await factory.ScalarAsync<long>(
                """
                select count(distinct m.user_id) from identity.memberships m
                join platform.centres c on c.id = m.centre_id where c.slug in ('nile-centre', 'maadi-hub')
                """));
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

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoEnrollmentsIdempotently_LeavingOneSeatInTheTwoSeatGroup()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var firstCounts = await EnrollmentCountsAsync();
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(firstCounts, await EnrollmentCountsAsync());
        Assert.Equal("maadi-hub:6,nile-centre:25", firstCounts);
        Assert.Equal(31, await factory.ScalarAsync<long>("select count(*) from academics.enrollments where status = 'active'"));
        Assert.Equal(0, await factory.ScalarAsync<long>("select count(*) from academics.enrollments where status != 'active'"));

        // Every Nile group has students, and the two-seat demo group has exactly one seat free.
        Assert.Equal(
            "1/2,6/15,8/20,10/20",
            await factory.ScalarAsync<string>(
                """
                select string_agg(n || '/' || capacity, ',' order by capacity, n) from (
                    select g.capacity, count(e.id) as n from academics.groups g
                    join platform.centres c on c.id = g.centre_id
                    left join academics.enrollments e on e.group_id = g.id and e.status = 'active'
                    where c.slug = 'nile-centre'
                    group by g.id, g.capacity) as seats
                """));

        // Commands only: every seeded enrollment was audited like an enrollment made over HTTP.
        Assert.Equal(31, await factory.ScalarAsync<long>("select count(*) from audit.audit_entries where entity_type = 'enrollment'"));
    }

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoSchedulesIdempotently_FromTheSeedDay()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var firstCounts = await DatasetCountsAsync();
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);
        Assert.Equal(firstCounts, await DatasetCountsAsync());
        Assert.Equal("maadi-hub:2,nile-centre:6", await PerCentreCountsAsync("academics.schedule_rules"));

        // A four-week horizon holds each weekday exactly four times, whatever day the seed runs.
        Assert.Equal("maadi-hub:8,nile-centre:24", await PerCentreCountsAsync("academics.class_sessions"));
        Assert.Equal(0, await factory.ScalarAsync<long>("select count(*) from academics.class_sessions where status != 'scheduled'"));

        // Relative dates only: every rule starts on the seed day in the centre's own zone, and its first session
        // is within the week that starts then.
        var seedDay = DateOnly.FromDateTime(
            TimeZoneInfo.ConvertTime(factory.Time.GetUtcNow(), TimeZoneInfo.FindSystemTimeZoneById("Africa/Cairo")).DateTime);
        Assert.Equal(
            seedDay.ToString("yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture),
            await factory.ScalarAsync<string>(
                "select string_agg(distinct effective_from::text, ',') from academics.schedule_rules where effective_to is null"));
        Assert.Equal(
            8,
            await factory.ScalarAsync<long>(
                """
                select count(distinct r.id) from academics.schedule_rules r
                join academics.class_sessions s on s.schedule_rule_id = r.id
                where s.local_date between r.effective_from and r.effective_from + 6
                """));

        // Every rule is the same centre as its group, and every session the same centre and teacher as its group.
        Assert.Equal(
            0,
            await factory.ScalarAsync<long>(
                """
                select count(*) from academics.class_sessions s
                join academics.schedule_rules r on r.id = s.schedule_rule_id
                join academics.groups g on g.id = r.group_id
                where s.centre_id != r.centre_id or r.centre_id != g.centre_id or s.teacher_id != g.teacher_id
                """));

        // Commands only: every seeded rule was audited like one added over HTTP.
        Assert.Equal(8, await factory.ScalarAsync<long>("select count(*) from audit.audit_entries where entity_type = 'schedule_rule'"));
    }

    private Task<string> PerCentreCountsAsync(string table) =>
        factory.ScalarAsync<string>(
            $"""
            select string_agg(slug || ':' || n, ',' order by slug) from (
                select c.slug, count(*) as n from {table} t
                join platform.centres c on c.id = t.centre_id
                group by c.slug) as counts
            """);

    // Every table the seed writes, so a second run that changed anything at all shows up as a different string.
    private Task<string> DatasetCountsAsync() =>
        factory.ScalarAsync<string>(
            """
            select concat_ws(',',
                (select count(*) from platform.centres),
                (select count(*) from identity.users),
                (select count(*) from identity.memberships),
                (select count(*) from academics.subjects),
                (select count(*) from academics.teachers),
                (select count(*) from academics.groups),
                (select count(*) from academics.students),
                (select count(*) from academics.parents),
                (select count(*) from academics.student_parents),
                (select count(*) from academics.enrollments),
                (select count(*) from academics.schedule_rules),
                (select count(*) from academics.class_sessions),
                (select count(*) from audit.audit_entries))
            """);

    private Task<string> EnrollmentCountsAsync() =>
        factory.ScalarAsync<string>(
            """
            select string_agg(slug || ':' || n, ',' order by slug) from (
                select c.slug, count(*) as n from academics.enrollments e
                join platform.centres c on c.id = e.centre_id
                group by c.slug) as counts
            """);
}
