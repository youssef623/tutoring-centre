using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;
using TutoringCentre.Application.Identity.Portal.Commands.DeactivatePortalAccount;
using TutoringCentre.Application.Identity.Portal.Commands.ReactivatePortalAccount;
using TutoringCentre.Application.Identity.Portal.Queries.ListPortalAccounts;
using TutoringCentre.Application.Identity.Queries.GetActiveMembership;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Application.Staff.Commands.ReactivateStaff;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Repositories;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

public sealed class PortalAccountTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    private StaffActor _owner = null!;
    private Guid _parent;
    private Guid _student;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _owner = new(await Fixture.SeedUserAsync("Acting Owner"), NileCentreId, StaffRole.Owner);
        _parent = await Fixture.SeedParentAsync(NileCentreId, "Nile Parent", "+201000000061");
        _student = await Fixture.SeedStudentAsync(NileCentreId, "Nile Student");
    }

    private CreatePortalAccountCommand Create(PortalSubjectKind kind = PortalSubjectKind.Parent, string email = "portal@nile.test") =>
        new(kind, kind == PortalSubjectKind.Parent ? _parent : _student, email, SupportedLocale.En);

    private Task<Result<CreatePortalAccountResult>> CreateAsync(CreatePortalAccountCommand? command = null, Actor? actor = null) =>
        Fixture.SendAsAsync<CreatePortalAccountCommand, CreatePortalAccountResult>(actor ?? _owner, command ?? Create());

    private Task<Result<IReadOnlyList<PortalAccountDto>>> ListAsync(PortalSubjectKind kind = PortalSubjectKind.Parent, Actor? actor = null) =>
        Fixture.QueryAsAsync<ListPortalAccountsQuery, IReadOnlyList<PortalAccountDto>>(actor ?? _owner,
            new(kind, kind == PortalSubjectKind.Parent ? _parent : _student));

    [Theory]
    [InlineData(PortalSubjectKind.Parent, StaffRole.Owner)]
    [InlineData(PortalSubjectKind.Student, StaffRole.Secretary)]
    public async Task CreateAndListPersistMembershipLinkUserAndCreationAudit(PortalSubjectKind kind, StaffRole role)
    {
        var created = await CreateAsync(Create(kind), new StaffActor(_owner.UserId, NileCentreId, role));
        Assert.True(created.IsSuccess, created.Error?.Message);
        Assert.NotNull(created.Value.TemporaryPassword);
        var listed = await ListAsync(kind);
        var account = Assert.Single(listed.Value);
        Assert.Equal(created.Value.LinkId, account.LinkId);
        Assert.Equal(created.Value.MembershipId, account.MembershipId);
        Assert.Equal("portal@nile.test", account.Email);
        Assert.Equal(MembershipStatus.Active, account.Status);
        Assert.True(account.Version > 0);
        Assert.NotEqual(default, account.CreatedAt);
        Assert.Equal(kind == PortalSubjectKind.Parent ? "Nile Parent" : "Nile Student",
            await Fixture.ScalarAsync<string>($"select display_name from identity.users where id='{created.Value.UserId}'"));
        Assert.Equal(1L, await Fixture.ScalarAsync<long>("select count(*) from identity.portal_links"));
        Assert.Equal(1L, await Fixture.ScalarAsync<long>($"select count(*) from audit.audit_entries where entity_type='portal_link' and entity_id='{account.LinkId}' and action='created'"));
        var changes = await Fixture.ScalarAsync<string>($"select changes::text from audit.audit_entries where entity_type='portal_link' and entity_id='{account.LinkId}'");
        Assert.Contains("roleKey", changes, StringComparison.Ordinal);
        Assert.DoesNotContain("email", changes, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain(created.Value.TemporaryPassword!, changes, StringComparison.Ordinal);
        Assert.Empty((await ListAsync(kind, new StaffActor(_owner.UserId, MaadiCentreId, StaffRole.Owner))).Value);
    }

    [Fact]
    public async Task ForeignOrInactiveSubjectCreatesNothing()
    {
        var foreign = await Fixture.SeedParentAsync(MaadiCentreId, "Maadi Parent", "+201000000062");
        Assert.Equal("portal.subject_not_found", (await CreateAsync(Create() with { SubjectId = foreign })).Error!.Code);
        Assert.Equal("portal.subject_not_found", (await CreateAsync(Create() with { SubjectId = Guid.NewGuid() })).Error!.Code);
        var archived = await Fixture.SeedStudentAsync(NileCentreId, "Archived Student", "archived");
        Assert.Equal("portal.subject_not_found", (await CreateAsync(Create(PortalSubjectKind.Student) with { SubjectId = archived })).Error!.Code);
        Assert.Equal(1L, await Fixture.ScalarAsync<long>("select count(*) from identity.users"));
        Assert.Equal(0L, await Fixture.ScalarAsync<long>("select count(*) from identity.memberships"));
        Assert.Equal(0L, await Fixture.ScalarAsync<long>("select count(*) from identity.portal_links"));
    }

    [Fact]
    public async Task LinkedSubjectIsRefusedBeforeAnotherUserIsCreated()
    {
        Assert.True((await CreateAsync()).IsSuccess);
        Assert.Equal("portal.subject_already_linked", (await CreateAsync(Create(email: "second@nile.test"))).Error!.Code);
        Assert.Equal(0L, await Fixture.ScalarAsync<long>("select count(*) from identity.users where email='second@nile.test'"));
    }

    [Fact]
    public async Task ExistingEmailInAnotherCentreReturnsNoPasswordAndKeepsExistingProfile()
    {
        var staff = await CreateStaffAsync(MaadiCentreId, "existing@maadi.test", StaffRole.Teacher, "Existing Name");
        var created = await CreateAsync(Create(email: "existing@maadi.test"));
        Assert.True(created.IsSuccess);
        Assert.Equal(staff.UserId, created.Value.UserId);
        Assert.Null(created.Value.TemporaryPassword);
        Assert.Equal("Existing Name", await Fixture.ScalarAsync<string>($"select display_name from identity.users where id='{staff.UserId}'"));
    }

    [Fact]
    public async Task ExistingMembershipInSameCentreIsRefused()
    {
        await CreateStaffAsync(NileCentreId, "existing@nile.test", StaffRole.Teacher);
        Assert.Equal("portal.user_already_linked", (await CreateAsync(Create(email: "existing@nile.test"))).Error!.Code);
        Assert.Equal(0L, await Fixture.ScalarAsync<long>("select count(*) from identity.portal_links"));
    }

    [Fact]
    public async Task LifecyclePreservesLinkEnforcesVersionsAndRevokesOnlyMembership()
    {
        var created = (await CreateAsync()).Value;
        var initial = Assert.Single((await ListAsync()).Value);
        Assert.True((await Fixture.SendAsAsync<DeactivatePortalAccountCommand, Unit>(_owner, new(created.LinkId, initial.Version))).IsSuccess);
        var inactive = Assert.Single((await ListAsync()).Value);
        Assert.Equal(MembershipStatus.Inactive, inactive.Status);
        Assert.Equal("membership.already_inactive", (await Fixture.SendAsAsync<DeactivatePortalAccountCommand, Unit>(_owner, new(created.LinkId, inactive.Version))).Error!.Code);
        Assert.Equal("concurrency.stale", (await Fixture.SendAsAsync<ReactivatePortalAccountCommand, Unit>(_owner, new(created.LinkId, initial.Version))).Error!.Code);
        Assert.Equal(MembershipStatus.Inactive, Assert.Single((await ListAsync()).Value).Status);
        // The real login would clear this flag through password change; isolate membership validation here.
        await ExecuteAsync($"update identity.users set must_change_password=false where id='{created.UserId}'");
        var gate = await Fixture.QueryAsAsync<GetActiveMembershipQuery, ActiveMembershipDto>(new ParentActor(created.UserId, NileCentreId, _parent), new(NileCentreId));
        Assert.Equal("tenant.no_membership", gate.Error!.Code);
        Assert.True((await Fixture.SendAsAsync<ReactivatePortalAccountCommand, Unit>(_owner, new(created.LinkId, inactive.Version))).IsSuccess);
        var active = Assert.Single((await ListAsync()).Value);
        Assert.Equal("membership.already_active", (await Fixture.SendAsAsync<ReactivatePortalAccountCommand, Unit>(_owner, new(created.LinkId, active.Version))).Error!.Code);
        Assert.Equal(created.LinkId, active.LinkId);
        Assert.Equal(1L, await Fixture.ScalarAsync<long>($"select count(*) from identity.users where id='{created.UserId}'"));
    }

    [Fact]
    public async Task ForeignLinkCannotBeDeactivatedOrReactivated()
    {
        var created = (await CreateAsync()).Value;
        var version = Assert.Single((await ListAsync()).Value).Version;
        var maadi = new StaffActor(_owner.UserId, MaadiCentreId, StaffRole.Owner);
        Assert.Equal("portal.not_found", (await Fixture.SendAsAsync<DeactivatePortalAccountCommand, Unit>(maadi, new(created.LinkId, version))).Error!.Code);
        Assert.Equal("portal.not_found", (await Fixture.SendAsAsync<ReactivatePortalAccountCommand, Unit>(maadi, new(created.LinkId, version))).Error!.Code);
        Assert.Equal(MembershipStatus.Active, Assert.Single((await ListAsync()).Value).Status);
    }

    [Fact]
    public async Task TeacherCannotDispatchAnyPortalAccountRequest()
    {
        var teacher = new StaffActor(_owner.UserId, NileCentreId, StaffRole.Teacher);
        Assert.Equal("auth.permission_denied", (await CreateAsync(actor: teacher)).Error!.Code);
        Assert.Equal("auth.permission_denied", (await ListAsync(actor: teacher)).Error!.Code);
        Assert.Equal("auth.permission_denied", (await Fixture.SendAsAsync<DeactivatePortalAccountCommand, Unit>(teacher, new(Guid.NewGuid(), 1))).Error!.Code);
        Assert.Equal("auth.permission_denied", (await Fixture.SendAsAsync<ReactivatePortalAccountCommand, Unit>(teacher, new(Guid.NewGuid(), 1))).Error!.Code);
        Assert.Equal(0L, await Fixture.ScalarAsync<long>("select count(*) from identity.portal_links"));
    }

    [Fact]
    public async Task StaffListCommandsDirectoryAndOwnerCountExcludePortalMemberships()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner@nile.test", StaffRole.Owner);
        var created = (await CreateAsync()).Value;
        var version = Assert.Single((await ListAsync()).Value).Version;
        var staff = await Fixture.QueryAsAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(owner.Actor, new());
        Assert.Equal(owner.MembershipId, Assert.Single(staff.Value).MembershipId);
        Assert.Equal("staff.not_found", (await Fixture.SendAsAsync<ChangeStaffRoleCommand, Unit>(owner.Actor, new(created.MembershipId, StaffRole.Teacher, version))).Error!.Code);
        Assert.Equal("staff.not_found", (await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(owner.Actor, new(created.MembershipId, version))).Error!.Code);
        Assert.Equal("staff.not_found", (await Fixture.SendAsAsync<ReactivateStaffCommand, Unit>(owner.Actor, new(created.MembershipId, version))).Error!.Code);
        Assert.Equal("staff.last_owner", (await Fixture.SendAsAsync<DeactivateStaffCommand, Unit>(_owner, new(owner.MembershipId, await ReadVersionAsync(owner, owner.MembershipId)))).Error!.Code);
        Assert.Equal(MembershipStatus.Active, Assert.Single((await ListAsync()).Value).Status);
    }

    [Fact]
    public async Task SessionSubjectReadUsesValidatedMembershipCentreAndRestoresAmbientTenant()
    {
        var created = (await CreateAsync()).Value;
        await Fixture.ReadAsAsync(new SystemActor(MaadiCentreId), async services =>
        {
            var state = await services.GetRequiredService<IMembershipReadService>()
                .GetSessionStateAsync(created.UserId, NileCentreId, CancellationToken.None);
            Assert.Equal(new PortalSubject(PortalSubjectKind.Parent, _parent), state!.PortalSubject);
            var setting = await services.GetRequiredService<AppDbContext>().Database
                .SqlQueryRaw<string>("SELECT current_setting('app.current_centre', true) AS \"Value\"").SingleAsync();
            Assert.Equal(MaadiCentreId.ToString(), setting);
            return true;
        });
    }

    [Fact]
    public async Task DispatcherAllowsPortalCapabilityUsingActorInterface()
    {
        await using var provider = Fixture.CreateServiceProvider(services =>
            services.AddScoped<IQueryHandler<PortalPermissionProbe, string>, PortalPermissionProbeHandler>());
        foreach (Actor actor in new Actor[] { new ParentActor(_owner.UserId, NileCentreId, _parent), new StudentActor(_owner.UserId, NileCentreId, _student) })
        {
            var result = await provider.QueryAsAsync<PortalPermissionProbe, string>(actor, new());
            Assert.True(result.IsSuccess, result.Error?.Message);
            Assert.Equal(((IUserActor)actor).RoleKey, result.Value);
        }
    }

    [Fact]
    public async Task LinkFailureRollsBackPreviouslyCreatedIdentityUser()
    {
        await using var provider = Fixture.CreateServiceProvider(services => services.AddScoped<IPortalLinkRepository>(sp =>
            new ControlledLinks(new PortalLinkRepository(sp.GetRequiredService<AppDbContext>()), failOnAdd: true)));
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => SendAsync(provider, Create(email: "rollback@nile.test")));
        Assert.Equal("simulated_link_failure", error.Message);
        Assert.Equal(0L, await Fixture.ScalarAsync<long>("select count(*) from identity.users where email='rollback@nile.test'"));
        Assert.Equal(0L, await Fixture.ScalarAsync<long>("select count(*) from identity.memberships"));
        Assert.Equal(0L, await Fixture.ScalarAsync<long>("select count(*) from identity.portal_links"));
    }

    [Fact]
    public async Task ParallelCreatesForOneSubjectYieldOneLinkAndStableSubjectConflict()
    {
        var gate = new TwoCallerGate();
        await using var provider = Fixture.CreateServiceProvider(services => services.AddScoped<IPortalLinkRepository>(sp =>
            new ControlledLinks(new PortalLinkRepository(sp.GetRequiredService<AppDbContext>()), gate)));
        var results = await Task.WhenAll(SendAsync(provider, Create(email: "race1@nile.test")), SendAsync(provider, Create(email: "race2@nile.test")));
        Assert.Single(results, result => result.IsSuccess);
        Assert.Equal("portal.subject_already_linked", Assert.Single(results, result => result.IsFailure).Error!.Code);
        Assert.Equal(1L, await Fixture.ScalarAsync<long>("select count(*) from identity.portal_links"));
        Assert.Equal(1L, await Fixture.ScalarAsync<long>("select count(*) from identity.memberships"));
        Assert.Equal(1L, await Fixture.ScalarAsync<long>("select count(*) from identity.users where email in ('race1@nile.test','race2@nile.test')"));
    }

    [Fact]
    public async Task ParallelCreatesForOneExistingLoginYieldStableUserConflict()
    {
        await CreateStaffAsync(MaadiCentreId, "shared@maadi.test", StaffRole.Teacher);
        var gate = new TwoCallerGate();
        await using var provider = Fixture.CreateServiceProvider(services => services.AddScoped<IPortalLinkRepository>(sp =>
            new ControlledLinks(new PortalLinkRepository(sp.GetRequiredService<AppDbContext>()), gate)));
        var results = await Task.WhenAll(SendAsync(provider, Create(email: "shared@maadi.test")),
            SendAsync(provider, Create(PortalSubjectKind.Student, "shared@maadi.test")));
        Assert.Single(results, result => result.IsSuccess);
        Assert.Equal("portal.user_already_linked", Assert.Single(results, result => result.IsFailure).Error!.Code);
        Assert.Equal(1L, await Fixture.ScalarAsync<long>("select count(*) from identity.portal_links"));
    }

    private async Task<Result<CreatePortalAccountResult>> SendAsync(ServiceProvider provider, CreatePortalAccountCommand command)
    {
        await using var scope = provider.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(_owner);
        return await scope.ServiceProvider.GetRequiredService<Dispatcher>().SendAsync<CreatePortalAccountCommand, CreatePortalAccountResult>(command, CancellationToken.None);
    }

    private async Task ExecuteAsync(string sql)
    {
        await using var connection = new NpgsqlConnection(Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(sql, connection);
        await command.ExecuteNonQueryAsync();
    }

    private sealed class TwoCallerGate
    {
        private readonly TaskCompletionSource _ready = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _arrivals;
        public Task ArriveAsync(CancellationToken ct)
        {
            if (Interlocked.Increment(ref _arrivals) == 2) _ready.TrySetResult();
            return _ready.Task.WaitAsync(TimeSpan.FromSeconds(15), ct);
        }
    }

    private sealed record PortalPermissionProbe : IQuery<string>, ITenantScoped, IRequirePermission
    {
        public string RequiredPermission => Permissions.PortalChat;
    }

    private sealed class PortalPermissionProbeHandler(ICurrentActor actor) : IQueryHandler<PortalPermissionProbe, string>
    {
        public Task<Result<string>> HandleAsync(PortalPermissionProbe query, CancellationToken cancellationToken) =>
            Task.FromResult(Result<string>.Success(actor.RoleKey!));
    }

    private sealed class ControlledLinks(IPortalLinkRepository inner, TwoCallerGate? gate = null, bool failOnAdd = false) : IPortalLinkRepository
    {
        public async Task<bool> SubjectExistsAsync(Guid centreId, PortalSubjectKind kind, Guid subjectId, CancellationToken ct)
        {
            var exists = await inner.SubjectExistsAsync(centreId, kind, subjectId, ct);
            if (gate is not null) await gate.ArriveAsync(ct);
            return exists;
        }
        public Task<PortalLink?> GetAsync(Guid linkId, Guid centreId, CancellationToken ct) => inner.GetAsync(linkId, centreId, ct);
        public Task<Membership?> GetMembershipForUpdateAsync(PortalLink link, uint expectedVersion, CancellationToken ct) => inner.GetMembershipForUpdateAsync(link, expectedVersion, ct);
        public void Add(PortalLink link)
        {
            if (failOnAdd) throw new InvalidOperationException("simulated_link_failure");
            inner.Add(link);
        }
    }
}
