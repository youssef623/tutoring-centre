using Npgsql;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;
using TutoringCentre.Infrastructure.Tests.Tenancy;

namespace TutoringCentre.Infrastructure.Tests.Identity;

public sealed class PortalLinkIntegrityTests(PostgresFixture fixture) : TenantProbeTestBase(fixture)
{
    private Guid _user;
    private Guid _parent;
    private Guid _student;

    public override async Task InitializeAsync()
    {
        await base.InitializeAsync();
        _user = await Fixture.SeedUserAsync();
        _parent = Guid.NewGuid();
        _student = Guid.NewGuid();
        await ExecuteAsync($"""
            insert into academics.parents (id,centre_id,full_name,search_name,phone,created_at)
            values ('{_parent}','{NileCentreId}','Test Parent','test parent','+201000000091',now());
            insert into academics.students (id,centre_id,full_name,search_name,status,links_revision,created_at)
            values ('{_student}','{NileCentreId}','Test Student','test student','active',0,now());
            insert into identity.memberships (id,centre_id,user_id,role,status,created_at)
            values ('{Guid.NewGuid()}','{NileCentreId}','{_user}','parent','active',now());
            """);
    }

    private string Insert(Guid? user = null, Guid? parent = null, string role = "parent", Guid? student = null) =>
        $"""
        insert into identity.portal_links (id,centre_id,user_id,role_key,parent_id,student_id,created_at,created_by)
        values ('{Guid.NewGuid()}','{NileCentreId}','{user ?? _user}','{role}',
        {(parent.HasValue ? $"'{parent}'" : "null")},{(student.HasValue ? $"'{student}'" : "null")},now(),'{_user}')
        """;

    [Fact]
    public async Task OneSaveOrdersMembershipBeforeLinkAndReadsImmutableProperties()
    {
        var otherUser = await Fixture.SeedUserAsync("New Portal Login");
        await using var probe = await CreateProbeScope(new SystemActor(NileCentreId));
        var membership = Membership.CreatePortal(otherUser, NileCentreId, PortalRole.Parent).Value;
        var link = PortalLink.CreateForParent(otherUser, NileCentreId, _parent, _user).Value;
        // Add the dependent first deliberately; relationship metadata must order SQL correctly.
        probe.Context.Add(link);
        probe.Context.Add(membership);
        await probe.Context.SaveChangesAsync();
        probe.Context.ChangeTracker.Clear();
        var stored = await probe.Context.Set<PortalLink>().SingleAsync();
        Assert.Equal(link.Id, stored.Id);
        Assert.Equal("parent", stored.RoleKey);
        Assert.Equal(_parent, stored.ParentId);
        Assert.Equal(_user, stored.CreatedBy);
    }

    [Fact]
    public async Task DuplicateLoginIsRejected()
    {
        await ExecuteAsync(Insert(parent: _parent), true, NileCentreId);
        // Different subject, so the login unique is the only duplicate.
        var otherParent = Guid.NewGuid();
        await ExecuteAsync($"insert into academics.parents (id,centre_id,full_name,search_name,phone,created_at) values ('{otherParent}','{NileCentreId}','Other Parent','other parent','+201000000092',now())");
        await RejectAsync(Insert(parent: otherParent), "23505", "ux_portal_links_centre_user");
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task SubjectCanHaveOnlyOneLogin(bool student)
    {
        if (student) await ExecuteAsync($"update identity.memberships set role='student' where user_id='{_user}'");
        await ExecuteAsync(student ? Insert(role: "student", student: _student) : Insert(parent: _parent), true, NileCentreId);
        var otherUser = await Fixture.SeedUserAsync("Other Login");
        var role = student ? "student" : "parent";
        await ExecuteAsync($"insert into identity.memberships (id,centre_id,user_id,role,status,created_at) values ('{Guid.NewGuid()}','{NileCentreId}','{otherUser}','{role}','active',now())");
        await RejectAsync(student ? Insert(otherUser, role: role, student: _student) : Insert(otherUser, _parent),
            "23505", student ? "ux_portal_links_centre_student" : "ux_portal_links_centre_parent");
    }

    [Fact]
    public async Task CrossTenantSubjectIsRejected()
    {
        var parent = Guid.NewGuid();
        await ExecuteAsync($"insert into academics.parents (id,centre_id,full_name,search_name,phone,created_at) values ('{parent}','{MaadiCentreId}','Other Parent','other parent','+201000000093',now())");
        await RejectAsync(Insert(parent: parent), "23503", "fk_portal_links_parents");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task WrongSubjectShapeIsRejected(bool both) =>
        await RejectAsync(Insert(parent: both ? _parent : null, student: _student), "23514", "ck_portal_links_subject");

    [Fact]
    public async Task RoleMustMatchMembershipAndCannotBePromoted()
    {
        await RejectAsync(Insert(role: "student", student: _student), "23503", "fk_portal_links_memberships");
        await ExecuteAsync(Insert(parent: _parent), true, NileCentreId);
        await RejectAsync($"update identity.memberships set role='owner' where user_id='{_user}'", "23503", "fk_portal_links_memberships");
    }

    [Theory]
    [InlineData("update identity.portal_links set role_key='parent'")]
    [InlineData("delete from identity.portal_links")]
    public async Task LinkCannotBeChangedByApplication(string sql)
    {
        await ExecuteAsync(Insert(parent: _parent), true, NileCentreId);
        var error = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(sql, true, NileCentreId));
        Assert.Equal("42501", error.SqlState);
    }

    [Fact]
    public async Task ForcedRlsHidesLinksAndRefusesForeignInsert()
    {
        await ExecuteAsync(Insert(parent: _parent), true, NileCentreId);
        Assert.Equal(0L, await CountAsync(null));
        Assert.Equal(0L, await CountAsync(MaadiCentreId));
        Assert.Equal(1L, await CountAsync(NileCentreId));
        var error = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(Insert(parent: _parent), true, MaadiCentreId));
        Assert.Equal("42501", error.SqlState);
        Assert.True(await Fixture.ScalarAsync<bool>("select relrowsecurity and relforcerowsecurity from pg_class where oid='identity.portal_links'::regclass"));
    }

    private async Task RejectAsync(string sql, string state, string constraint)
    {
        var error = await Assert.ThrowsAsync<PostgresException>(() => ExecuteAsync(sql, true, NileCentreId));
        Assert.Equal(state, error.SqlState);
        Assert.Equal(constraint, error.ConstraintName);
    }

    private async Task ExecuteAsync(string sql, bool app = false, Guid? centre = null)
    {
        await using var connection = new NpgsqlConnection(app ? Fixture.AppConnectionString : Fixture.SuperuserConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        if (centre.HasValue)
        {
            await using var setting = new NpgsqlCommand("select set_config('app.current_centre', @centre, true)", connection, transaction);
            setting.Parameters.AddWithValue("centre", centre.Value.ToString());
            await setting.ExecuteNonQueryAsync();
        }
        await using var command = new NpgsqlCommand(sql, connection, transaction);
        await command.ExecuteNonQueryAsync();
        await transaction.CommitAsync();
    }

    private async Task<long> CountAsync(Guid? centre)
    {
        await using var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("select set_config('app.current_centre', @centre, false); select count(*) from identity.portal_links", connection);
        command.Parameters.AddWithValue("centre", centre?.ToString() ?? "");
        await using var reader = await command.ExecuteReaderAsync();
        await reader.NextResultAsync();
        Assert.True(await reader.ReadAsync());
        return reader.GetInt64(0);
    }
}
