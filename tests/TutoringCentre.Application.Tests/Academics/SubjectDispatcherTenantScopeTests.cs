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
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Task 24.9: the layer 1 proof for this feature — through the real dispatcher, a centre-less staff actor is
/// refused for every one of the six Subject requests, and the handler (and therefore the fake ports behind it)
/// is never invoked.
/// </summary>
public sealed class SubjectDispatcherTenantScopeTests
{
    private static readonly StaffActor CentrelessActor = new(Guid.NewGuid(), CentreId: null, Role: null);

    [Fact]
    public async Task SendAsync_CreateSubject_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.SendAsync<CreateSubjectCommand, CreateSubjectResult>(
            new CreateSubjectCommand("Mathematics"), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
        Assert.Empty(fixture.SubjectRepository.Added);
    }

    [Fact]
    public async Task SendAsync_RenameSubject_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.SendAsync<RenameSubjectCommand, Unit>(
            new RenameSubjectCommand(Guid.CreateVersion7(), "Applied Mathematics", 0), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    [Fact]
    public async Task SendAsync_ArchiveSubject_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.SendAsync<ArchiveSubjectCommand, Unit>(
            new ArchiveSubjectCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    [Fact]
    public async Task SendAsync_RestoreSubject_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.SendAsync<RestoreSubjectCommand, Unit>(
            new RestoreSubjectCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_ListSubjects_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.QueryAsync<ListSubjectsQuery, IReadOnlyList<SubjectDto>>(
            new ListSubjectsQuery(false), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_GetSubject_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.QueryAsync<GetSubjectQuery, SubjectDto>(
            new GetSubjectQuery(Guid.CreateVersion7()), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    private static void AssertTenantNotSelected<T>(Result<T> result, Fixture fixture)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("tenant.not_selected", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    private static Fixture CreateSut()
    {
        var unitOfWork = new FakeUnitOfWork();
        var currentActor = new FakeCurrentActor { Actor = CentrelessActor };
        var subjectRepository = new FakeSubjectRepository();
        var subjectReadService = new FakeSubjectReadService();

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
            new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance), unitOfWork, subjectRepository, provider);
    }

    private sealed class Fixture(
        Dispatcher dispatcher, FakeUnitOfWork unitOfWork, FakeSubjectRepository subjectRepository, ServiceProvider provider)
        : IDisposable
    {
        public Dispatcher Dispatcher { get; } = dispatcher;

        public FakeUnitOfWork UnitOfWork { get; } = unitOfWork;

        public FakeSubjectRepository SubjectRepository { get; } = subjectRepository;

        public void Dispose() => provider.Dispose();
    }
}
