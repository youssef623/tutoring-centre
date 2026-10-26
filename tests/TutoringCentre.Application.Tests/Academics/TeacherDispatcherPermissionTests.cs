using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: only Owner holds teachers.manage, so the five commands succeed for Owner and
/// are denied for both Secretary and Teacher. Both Owner and Secretary hold teachers.view, so the two queries
/// succeed for both and are denied only for Teacher.
/// </summary>
public sealed class TeacherDispatcherPermissionTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Fact]
    public async Task SendAsync_CreateTeacher_SucceedsForOwner()
    {
        using var fixture = CreateSut(StaffRole.Owner);

        var result = await fixture.Dispatcher.SendAsync<CreateTeacherCommand, CreateTeacherResult>(
            new CreateTeacherCommand("Mona Said", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_CreateTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<CreateTeacherCommand, CreateTeacherResult>(
            new CreateTeacherCommand("Mona Said", null), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
        Assert.Empty(fixture.TeacherRepository.Added);
    }

    [Fact]
    public async Task SendAsync_UpdateTeacher_SucceedsForOwner()
    {
        using var fixture = CreateSut(StaffRole.Owner);

        var result = await fixture.Dispatcher.SendAsync<UpdateTeacherCommand, Unit>(
            new UpdateTeacherCommand(fixture.ActiveTeacher.Id, "Mona Said", null, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_UpdateTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<UpdateTeacherCommand, Unit>(
            new UpdateTeacherCommand(fixture.ActiveTeacher.Id, "Mona Said", null, 0), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Fact]
    public async Task SendAsync_SetTeacherAccount_SucceedsForOwner()
    {
        using var fixture = CreateSut(StaffRole.Owner);

        var result = await fixture.Dispatcher.SendAsync<SetTeacherAccountCommand, Unit>(
            new SetTeacherAccountCommand(fixture.ActiveTeacher.Id, null, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_SetTeacherAccount_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<SetTeacherAccountCommand, Unit>(
            new SetTeacherAccountCommand(fixture.ActiveTeacher.Id, null, 0), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Fact]
    public async Task SendAsync_ArchiveTeacher_SucceedsForOwner()
    {
        using var fixture = CreateSut(StaffRole.Owner);

        var result = await fixture.Dispatcher.SendAsync<ArchiveTeacherCommand, Unit>(
            new ArchiveTeacherCommand(fixture.ActiveTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_ArchiveTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<ArchiveTeacherCommand, Unit>(
            new ArchiveTeacherCommand(fixture.ActiveTeacher.Id, 0), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Fact]
    public async Task SendAsync_RestoreTeacher_SucceedsForOwner()
    {
        using var fixture = CreateSut(StaffRole.Owner);

        var result = await fixture.Dispatcher.SendAsync<RestoreTeacherCommand, Unit>(
            new RestoreTeacherCommand(fixture.ArchivedTeacher.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_RestoreTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<RestoreTeacherCommand, Unit>(
            new RestoreTeacherCommand(fixture.ArchivedTeacher.Id, 0), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task QueryAsync_ListTeachers_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.QueryAsync<ListTeachersQuery, IReadOnlyList<TeacherDto>>(
            new ListTeachersQuery(false), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task QueryAsync_ListTeachers_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.QueryAsync<ListTeachersQuery, IReadOnlyList<TeacherDto>>(
            new ListTeachersQuery(false), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task QueryAsync_GetTeacher_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.QueryAsync<GetTeacherQuery, TeacherDto>(
            new GetTeacherQuery(fixture.ActiveTeacher.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task QueryAsync_GetTeacher_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.QueryAsync<GetTeacherQuery, TeacherDto>(
            new GetTeacherQuery(fixture.ActiveTeacher.Id), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    private static void AssertPermissionDenied<T>(Result<T> result, Fixture fixture)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("auth.permission_denied", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    private static Fixture CreateSut(StaffRole role)
    {
        var unitOfWork = new FakeUnitOfWork();
        var actor = new StaffActor(Guid.NewGuid(), CentreId, role);
        var currentActor = new FakeCurrentActor { Actor = actor };
        var teacherRepository = new FakeTeacherRepository();
        var teacherReadService = new FakeTeacherReadService();
        var staffDirectory = new FakeStaffDirectory();

        var activeTeacher = Teacher.Create(CentreId, "Amina Hassan", null).Value;
        var archivedTeacher = Teacher.Create(CentreId, "Mona Said", null).Value;
        archivedTeacher.Archive();
        teacherRepository.Existing.Add(activeTeacher);
        teacherRepository.Existing.Add(archivedTeacher);
        teacherReadService.Teachers.Add(new TeacherDto(activeTeacher.Id, activeTeacher.FullName, null, TeacherStatus.Active, null, null, 0, 0));

        var provider = new ServiceCollection()
            .AddSingleton(unitOfWork)
            .AddSingleton<ICurrentActor>(currentActor)
            .AddSingleton<IGroupRepository>(new FakeGroupRepository())
            .AddSingleton<ITeacherRepository>(teacherRepository)
            .AddSingleton<ITeacherReadService>(teacherReadService)
            .AddSingleton<IStaffDirectory>(staffDirectory)
            .AddScoped<ICommandHandler<CreateTeacherCommand, CreateTeacherResult>, CreateTeacherHandler>()
            .AddScoped<ICommandHandler<UpdateTeacherCommand, Unit>, UpdateTeacherHandler>()
            .AddScoped<ICommandHandler<SetTeacherAccountCommand, Unit>, SetTeacherAccountHandler>()
            .AddScoped<ICommandHandler<ArchiveTeacherCommand, Unit>, ArchiveTeacherHandler>()
            .AddScoped<ICommandHandler<RestoreTeacherCommand, Unit>, RestoreTeacherHandler>()
            .AddScoped<IQueryHandler<ListTeachersQuery, IReadOnlyList<TeacherDto>>, ListTeachersHandler>()
            .AddScoped<IQueryHandler<GetTeacherQuery, TeacherDto>, GetTeacherHandler>()
            .AddScoped<IValidator<CreateTeacherCommand>, CreateTeacherValidator>()
            .AddScoped<IValidator<UpdateTeacherCommand>, UpdateTeacherValidator>()
            .AddScoped<IValidator<SetTeacherAccountCommand>, SetTeacherAccountValidator>()
            .AddScoped<IValidator<ArchiveTeacherCommand>, ArchiveTeacherValidator>()
            .AddScoped<IValidator<RestoreTeacherCommand>, RestoreTeacherValidator>()
            .BuildServiceProvider();

        return new Fixture(
            new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance),
            unitOfWork,
            teacherRepository,
            activeTeacher,
            archivedTeacher,
            provider);
    }

    private sealed class Fixture(
        Dispatcher dispatcher,
        FakeUnitOfWork unitOfWork,
        FakeTeacherRepository teacherRepository,
        Teacher activeTeacher,
        Teacher archivedTeacher,
        ServiceProvider provider) : IDisposable
    {
        public Dispatcher Dispatcher { get; } = dispatcher;

        public FakeUnitOfWork UnitOfWork { get; } = unitOfWork;

        public FakeTeacherRepository TeacherRepository { get; } = teacherRepository;

        public Teacher ActiveTeacher { get; } = activeTeacher;

        public Teacher ArchivedTeacher { get; } = archivedTeacher;

        public void Dispose() => provider.Dispose();
    }
}
