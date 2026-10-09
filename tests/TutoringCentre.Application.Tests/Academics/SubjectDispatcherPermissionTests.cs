using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Task 28.6: through the real dispatcher, Teacher is read-only at the use-case level for all six Subject
/// requests, while Secretary and Owner can do everything. Closes the Day 24 authorization gap.
/// </summary>
public sealed class SubjectDispatcherPermissionTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task SendAsync_CreateSubject_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<CreateSubjectCommand, CreateSubjectResult>(
            new CreateSubjectCommand("Biology"), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SendAsync_CreateSubject_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.SendAsync<CreateSubjectCommand, CreateSubjectResult>(
            new CreateSubjectCommand("Biology"), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
        Assert.Empty(fixture.SubjectRepository.Added);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task SendAsync_RenameSubject_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<RenameSubjectCommand, Unit>(
            new RenameSubjectCommand(fixture.ActiveSubject.Id, "Applied Mathematics", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SendAsync_RenameSubject_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.SendAsync<RenameSubjectCommand, Unit>(
            new RenameSubjectCommand(fixture.ActiveSubject.Id, "Applied Mathematics", 0), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task SendAsync_ArchiveSubject_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<ArchiveSubjectCommand, Unit>(
            new ArchiveSubjectCommand(fixture.ActiveSubject.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SendAsync_ArchiveSubject_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.SendAsync<ArchiveSubjectCommand, Unit>(
            new ArchiveSubjectCommand(fixture.ActiveSubject.Id, 0), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task SendAsync_RestoreSubject_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<RestoreSubjectCommand, Unit>(
            new RestoreSubjectCommand(fixture.ArchivedSubject.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SendAsync_RestoreSubject_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.SendAsync<RestoreSubjectCommand, Unit>(
            new RestoreSubjectCommand(fixture.ArchivedSubject.Id, 0), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Theory]
    [InlineData(StaffRole.Teacher)]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task QueryAsync_ListSubjects_SucceedsForEveryRole(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.QueryAsync<ListSubjectsQuery, IReadOnlyList<SubjectDto>>(
            new ListSubjectsQuery(false), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(StaffRole.Teacher)]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task QueryAsync_GetSubject_SucceedsForEveryRole(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.QueryAsync<GetSubjectQuery, SubjectDto>(
            new GetSubjectQuery(fixture.ActiveSubject.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
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
        var subjectRepository = new FakeSubjectRepository();
        var subjectReadService = new FakeSubjectReadService();

        var activeSubject = Subject.Create(CentreId, "Mathematics").Value;
        var archivedSubject = Subject.Create(CentreId, "Chemistry").Value;
        archivedSubject.Archive();
        subjectRepository.Existing.Add(activeSubject);
        subjectRepository.Existing.Add(archivedSubject);
        subjectReadService.Subjects.Add(new SubjectDto(activeSubject.Id, activeSubject.Name, SubjectStatus.Active, 0));

        var provider = new ServiceCollection()
            .AddSingleton(unitOfWork)
            .AddSingleton<ICurrentActor>(currentActor)
            .AddSingleton<ISubjectRepository>(subjectRepository)
            .AddSingleton<ISubjectReadService>(subjectReadService)
            .AddScoped<ICommandHandler<CreateSubjectCommand, CreateSubjectResult>, CreateSubjectHandler>()
            .AddScoped<ICommandHandler<RenameSubjectCommand, Unit>, RenameSubjectHandler>()
            .AddScoped<ICommandHandler<ArchiveSubjectCommand, Unit>, ArchiveSubjectHandler>()
            .AddScoped<ICommandHandler<RestoreSubjectCommand, Unit>, RestoreSubjectHandler>()
            .AddScoped<IQueryHandler<ListSubjectsQuery, IReadOnlyList<SubjectDto>>, ListSubjectsHandler>()
            .AddScoped<IQueryHandler<GetSubjectQuery, SubjectDto>, GetSubjectHandler>()
            .AddScoped<IValidator<CreateSubjectCommand>, CreateSubjectValidator>()
            .AddScoped<IValidator<RenameSubjectCommand>, RenameSubjectValidator>()
            .AddScoped<IValidator<ArchiveSubjectCommand>, ArchiveSubjectValidator>()
            .AddScoped<IValidator<RestoreSubjectCommand>, RestoreSubjectValidator>()
            .BuildServiceProvider();

        return new Fixture(
            new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance),
            unitOfWork,
            subjectRepository,
            activeSubject,
            archivedSubject,
            provider);
    }

    private sealed class Fixture(
        Dispatcher dispatcher,
        FakeUnitOfWork unitOfWork,
        FakeSubjectRepository subjectRepository,
        Subject activeSubject,
        Subject archivedSubject,
        ServiceProvider provider) : IDisposable
    {
        public Dispatcher Dispatcher { get; } = dispatcher;

        public FakeUnitOfWork UnitOfWork { get; } = unitOfWork;

        public FakeSubjectRepository SubjectRepository { get; } = subjectRepository;

        public Subject ActiveSubject { get; } = activeSubject;

        public Subject ArchivedSubject { get; } = archivedSubject;

        public void Dispose() => provider.Dispose();
    }
}
