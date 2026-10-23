using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Academics.Groups.Commands.AssignGroupTeacher;
using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;
using TutoringCentre.Application.Academics.Groups.Commands.RestoreGroup;
using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;
using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: tenancy and permission, the first two of the three checks. Owner and Secretary
/// hold groups.manage; Teacher holds only groups.view, so all six commands are denied for Teacher before any
/// handler or transaction runs, while both queries are allowed (ownership narrows them later, in the read).
/// An actor with no selected centre is refused every group request as tenant.not_selected.
/// </summary>
public sealed class GroupDispatcherPermissionTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    public static TheoryData<string> CommandNames => new()
    {
        nameof(CreateGroupCommand),
        nameof(RenameGroupCommand),
        nameof(ChangeGroupCapacityCommand),
        nameof(AssignGroupTeacherCommand),
        nameof(ArchiveGroupCommand),
        nameof(RestoreGroupCommand),
    };

    [Theory]
    [MemberData(nameof(CommandNames))]
    public async Task SendAsync_EveryCommand_ReturnsPermissionDeniedForTeacher(string commandName)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, StaffRole.Teacher));

        var error = await SendAsync(fixture, commandName);

        Assert.NotNull(error);
        Assert.Equal("auth.permission_denied", error.Code);
        Assert.Equal(ErrorKind.Forbidden, error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
        Assert.Empty(fixture.Groups.Added);
    }

    [Theory]
    [MemberData(nameof(CommandNames))]
    public async Task SendAsync_EveryCommand_ReturnsTenantNotSelectedForACentrelessActor(string commandName)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), null, null));

        var error = await SendAsync(fixture, commandName);

        Assert.NotNull(error);
        Assert.Equal("tenant.not_selected", error.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Theory]
    [InlineData(StaffRole.Owner)]
    [InlineData(StaffRole.Secretary)]
    public async Task SendAsync_CreateGroup_SucceedsForOwnerAndSecretary(StaffRole role)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, role));

        var error = await SendAsync(fixture, nameof(CreateGroupCommand));

        Assert.Null(error);
        Assert.Single(fixture.Groups.Added);
    }

    [Fact]
    public async Task QueryAsync_ListAndGet_AreAllowedForTeacher()
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, StaffRole.Teacher));

        var list = await fixture.Dispatcher.QueryAsync<ListGroupsQuery, IReadOnlyList<GroupDto>>(
            new ListGroupsQuery(null, null, false), CancellationToken.None);
        var get = await fixture.Dispatcher.QueryAsync<GetGroupQuery, GroupDto>(
            new GetGroupQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.True(list.IsSuccess);
        Assert.Equal("group.not_found", get.Error!.Code);
    }

    [Fact]
    public async Task QueryAsync_ListAndGet_ReturnTenantNotSelectedForACentrelessActor()
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), null, null));

        var list = await fixture.Dispatcher.QueryAsync<ListGroupsQuery, IReadOnlyList<GroupDto>>(
            new ListGroupsQuery(null, null, false), CancellationToken.None);
        var get = await fixture.Dispatcher.QueryAsync<GetGroupQuery, GroupDto>(
            new GetGroupQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.Equal("tenant.not_selected", list.Error!.Code);
        Assert.Equal("tenant.not_selected", get.Error!.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    private static async Task<Error?> SendAsync(Fixture fixture, string commandName)
    {
        var dispatcher = fixture.Dispatcher;
        var group = fixture.ActiveGroup;
        Result result = commandName switch
        {
            nameof(CreateGroupCommand) => await dispatcher.SendAsync<CreateGroupCommand, CreateGroupResult>(
                new CreateGroupCommand("Grade 11 A", fixture.Subject.Id, fixture.Teacher.Id, 20), CancellationToken.None),
            nameof(RenameGroupCommand) => await dispatcher.SendAsync<RenameGroupCommand, Unit>(
                new RenameGroupCommand(group.Id, "Grade 10 B", 0), CancellationToken.None),
            nameof(ChangeGroupCapacityCommand) => await dispatcher.SendAsync<ChangeGroupCapacityCommand, Unit>(
                new ChangeGroupCapacityCommand(group.Id, 30, 0), CancellationToken.None),
            nameof(AssignGroupTeacherCommand) => await dispatcher.SendAsync<AssignGroupTeacherCommand, Unit>(
                new AssignGroupTeacherCommand(group.Id, fixture.Teacher.Id, 0), CancellationToken.None),
            nameof(ArchiveGroupCommand) => await dispatcher.SendAsync<ArchiveGroupCommand, Unit>(
                new ArchiveGroupCommand(group.Id, 0), CancellationToken.None),
            nameof(RestoreGroupCommand) => await dispatcher.SendAsync<RestoreGroupCommand, Unit>(
                new RestoreGroupCommand(group.Id, 0), CancellationToken.None),
            _ => throw new ArgumentOutOfRangeException(nameof(commandName), commandName, null),
        };

        return result.Error;
    }

    private static Fixture CreateSut(Actor actor)
    {
        var unitOfWork = new FakeUnitOfWork();
        var subjects = new FakeSubjectRepository();
        var teachers = new FakeTeacherRepository();
        var groups = new FakeGroupRepository();

        var subject = Subject.Create(CentreId, "Mathematics").Value;
        var teacher = Teacher.Create(CentreId, "Amina Hassan", null).Value;
        var activeGroup = Group.Create(CentreId, "Grade 10 A", subject.Id, teacher.Id, 20).Value;
        subjects.Existing.Add(subject);
        teachers.Existing.Add(teacher);
        groups.Existing.Add(activeGroup);

        var provider = new ServiceCollection()
            .AddSingleton(unitOfWork)
            .AddSingleton<ICurrentActor>(new FakeCurrentActor { Actor = actor })
            .AddSingleton<ISubjectRepository>(subjects)
            .AddSingleton<ITeacherRepository>(teachers)
            .AddSingleton<IGroupRepository>(groups)
            .AddSingleton<IGroupReadService>(new FakeGroupReadService())
            .AddSingleton<ITeacherScopeProvider>(new FakeTeacherScopeProvider { Scope = TeacherScope.All })
            .AddScoped<ICommandHandler<CreateGroupCommand, CreateGroupResult>, CreateGroupHandler>()
            .AddScoped<ICommandHandler<RenameGroupCommand, Unit>, RenameGroupHandler>()
            .AddScoped<ICommandHandler<ChangeGroupCapacityCommand, Unit>, ChangeGroupCapacityHandler>()
            .AddScoped<ICommandHandler<AssignGroupTeacherCommand, Unit>, AssignGroupTeacherHandler>()
            .AddScoped<ICommandHandler<ArchiveGroupCommand, Unit>, ArchiveGroupHandler>()
            .AddScoped<ICommandHandler<RestoreGroupCommand, Unit>, RestoreGroupHandler>()
            .AddScoped<IQueryHandler<ListGroupsQuery, IReadOnlyList<GroupDto>>, ListGroupsHandler>()
            .AddScoped<IQueryHandler<GetGroupQuery, GroupDto>, GetGroupHandler>()
            .AddScoped<IValidator<CreateGroupCommand>, CreateGroupValidator>()
            .AddScoped<IValidator<RenameGroupCommand>, RenameGroupValidator>()
            .AddScoped<IValidator<ChangeGroupCapacityCommand>, ChangeGroupCapacityValidator>()
            .AddScoped<IValidator<AssignGroupTeacherCommand>, AssignGroupTeacherValidator>()
            .AddScoped<IValidator<ArchiveGroupCommand>, ArchiveGroupValidator>()
            .AddScoped<IValidator<RestoreGroupCommand>, RestoreGroupValidator>()
            .BuildServiceProvider();

        return new Fixture(
            new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance), unitOfWork, groups, subject, teacher, activeGroup, provider);
    }

    private sealed class Fixture(
        Dispatcher dispatcher,
        FakeUnitOfWork unitOfWork,
        FakeGroupRepository groups,
        Subject subject,
        Teacher teacher,
        Group activeGroup,
        ServiceProvider provider) : IDisposable
    {
        public Dispatcher Dispatcher { get; } = dispatcher;

        public FakeUnitOfWork UnitOfWork { get; } = unitOfWork;

        public FakeGroupRepository Groups { get; } = groups;

        public Subject Subject { get; } = subject;

        public Teacher Teacher { get; } = teacher;

        public Group ActiveGroup { get; } = activeGroup;

        public void Dispose() => provider.Dispose();
    }
}
