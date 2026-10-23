using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Identity;

/// <summary>
/// Task 44.4: the ownership guard resolved through the real provider, the real teacher read service and real
/// PostgreSQL, inside a dispatched request so row-level security is in force. Fails closed: every case that is
/// not explicitly All or Own must come back None.
/// </summary>
public sealed class TeacherScopeProviderTests(PostgresFixture fixture) : StaffProbeTestBase(fixture)
{
    [Fact]
    public async Task SystemActor_ResolvesAll()
    {
        var scope = await ResolveAsync(new SystemActor(NileCentreId));

        Assert.Equal(TeacherScope.All, scope);
    }

    [Fact]
    public async Task Owner_ResolvesAll()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-scope1@nile.test", StaffRole.Owner);

        var scope = await ResolveAsync(owner.Actor);

        Assert.Equal(TeacherScope.All, scope);
    }

    [Fact]
    public async Task Secretary_ResolvesAll()
    {
        var secretary = await CreateStaffAsync(NileCentreId, "secretary-scope2@nile.test", StaffRole.Secretary);

        var scope = await ResolveAsync(secretary.Actor);

        Assert.Equal(TeacherScope.All, scope);
    }

    [Fact]
    public async Task TeacherLinkedToActiveRecord_ResolvesOwnWithThatId()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-scope3@nile.test", StaffRole.Owner);
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-scope3@nile.test", StaffRole.Teacher);
        var teacherId = await CreateLinkedTeacherAsync(owner, teacherMember.UserId);

        var scope = await ResolveAsync(teacherMember.Actor);

        Assert.Equal(TeacherScope.Own(teacherId), scope);
    }

    [Fact]
    public async Task TeacherWithNoLinkedRecord_ResolvesNone()
    {
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-scope4@nile.test", StaffRole.Teacher);

        var scope = await ResolveAsync(teacherMember.Actor);

        Assert.Equal(TeacherScope.None, scope);
    }

    [Fact]
    public async Task TeacherLinkedToArchivedRecord_ResolvesNone()
    {
        var owner = await CreateStaffAsync(NileCentreId, "owner-scope5@nile.test", StaffRole.Owner);
        var teacherMember = await CreateStaffAsync(NileCentreId, "teacher-scope5@nile.test", StaffRole.Teacher);
        var teacherId = await CreateLinkedTeacherAsync(owner, teacherMember.UserId);
        var version = await ReadTeacherVersionAsync(owner, teacherId);
        var archived = await Fixture.SendAsAsync<ArchiveTeacherCommand, Unit>(owner.Actor, new ArchiveTeacherCommand(teacherId, version));
        Assert.True(archived.IsSuccess, archived.Error?.Message);

        var scope = await ResolveAsync(teacherMember.Actor);

        Assert.Equal(TeacherScope.None, scope);
    }

    [Fact]
    public async Task AnonymousActor_ResolvesNone()
    {
        var scope = await ResolveAsync(new AnonymousActor());

        Assert.Equal(TeacherScope.None, scope);
    }

    private async Task<TeacherScope> ResolveAsync(Actor actor)
    {
        var result = await Fixture.QueryAsAsync<ResolveTeacherScopeQuery, TeacherScope>(actor, new ResolveTeacherScopeQuery());
        Assert.True(result.IsSuccess, result.Error?.Message);
        return result.Value;
    }

    private async Task<Guid> CreateLinkedTeacherAsync(CreatedStaff owner, Guid userId)
    {
        var created = await Fixture.SendAsAsync<CreateTeacherCommand, CreateTeacherResult>(owner.Actor, new CreateTeacherCommand("Amina Hassan", null));
        Assert.True(created.IsSuccess, created.Error?.Message);
        var version = await ReadTeacherVersionAsync(owner, created.Value.TeacherId);
        var linked = await Fixture.SendAsAsync<SetTeacherAccountCommand, Unit>(
            owner.Actor, new SetTeacherAccountCommand(created.Value.TeacherId, userId, version));
        Assert.True(linked.IsSuccess, linked.Error?.Message);
        return created.Value.TeacherId;
    }

    private async Task<uint> ReadTeacherVersionAsync(CreatedStaff owner, Guid teacherId)
    {
        var dto = await Fixture.QueryAsAsync<GetTeacherQuery, TeacherDto>(owner.Actor, new GetTeacherQuery(teacherId));
        Assert.True(dto.IsSuccess, dto.Error?.Message);
        return dto.Value.Version;
    }
}

/// <summary>Test-only: resolves the scope inside a real dispatched read, the way a group query handler will.</summary>
internal sealed record ResolveTeacherScopeQuery : IQuery<TeacherScope>;

internal sealed class ResolveTeacherScopeQueryHandler(ITeacherScopeProvider provider) : IQueryHandler<ResolveTeacherScopeQuery, TeacherScope>
{
    public async Task<Result<TeacherScope>> HandleAsync(ResolveTeacherScopeQuery query, CancellationToken cancellationToken) =>
        Result<TeacherScope>.Success(await provider.GetAsync(cancellationToken));
}

internal static class TeacherScopeProviderTestRegistration
{
    public static IServiceCollection AddTeacherScopeProbe(this IServiceCollection services) =>
        services.AddScoped<IQueryHandler<ResolveTeacherScopeQuery, TeacherScope>, ResolveTeacherScopeQueryHandler>();
}
