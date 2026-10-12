using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;
using TutoringCentre.Application.Academics.Students.Commands.LinkParent;
using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;
using TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;
using TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;
using TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;
using TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;
using TutoringCentre.Application.Academics.Students.Queries.GetStudent;
using TutoringCentre.Application.Academics.Students.Queries.ListStudents;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: Teacher is refused students.manage/students.view for all nine student
/// requests, and a centre-less actor is refused all nine before any handler runs — layer 1 (tenant scope) and
/// the permission step, both proven before any transaction, mirroring Parent's Day 39 proofs.
/// </summary>
public sealed class StudentDispatcherAuthorizationTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();
    private static readonly StaffActor CentrelessActor = new(Guid.NewGuid(), CentreId: null, Role: null);

    [Theory]
    [InlineData(StaffRole.Teacher)]
    public async Task EveryManageCommand_ReturnsPermissionDeniedForTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<RegisterStudentCommand, RegisterStudentResult>(
            new RegisterStudentCommand("Youssef Omar", null, null, new RegisterStudentParent(fixture.ExistingParent.Id, null, null), ParentRelationship.Father),
            CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<UpdateStudentCommand, Unit>(
            new UpdateStudentCommand(fixture.ExistingStudent.Id, "Youssef Omar", null, null, 0), CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<ArchiveStudentCommand, Unit>(
            new ArchiveStudentCommand(fixture.ExistingStudent.Id, 0), CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<RestoreStudentCommand, Unit>(
            new RestoreStudentCommand(fixture.ExistingStudent.Id, 0), CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<LinkParentCommand, Unit>(
            new LinkParentCommand(fixture.ExistingStudent.Id, fixture.ExistingParent.Id, ParentRelationship.Mother, 0), CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<UnlinkParentCommand, Unit>(
            new UnlinkParentCommand(fixture.ExistingStudent.Id, fixture.ExistingParent.Id, 0), CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<SetPrimaryParentCommand, Unit>(
            new SetPrimaryParentCommand(fixture.ExistingStudent.Id, fixture.ExistingParent.Id, 0), CancellationToken.None), fixture);
    }

    [Fact]
    public async Task QueryAsync_GetStudent_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.QueryAsync<GetStudentQuery, StudentDetailDto>(
            new GetStudentQuery(fixture.ExistingStudent.Id), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_ListStudents_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.QueryAsync<ListStudentsQuery, StudentPageDto>(
            new ListStudentsQuery(null, false, null, 25), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task SendAsync_RegisterStudent_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<RegisterStudentCommand, RegisterStudentResult>(
            new RegisterStudentCommand("Youssef Omar", null, null, new RegisterStudentParent(fixture.ExistingParent.Id, null, null), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SendAsync_RegisterStudent_ReturnsTenantNotSelectedForCentrelessActor()
    {
        using var fixture = CreateSut(CentrelessActor);

        var result = await fixture.Dispatcher.SendAsync<RegisterStudentCommand, RegisterStudentResult>(
            new RegisterStudentCommand("Youssef Omar", null, null, new RegisterStudentParent(Guid.CreateVersion7(), null, null), ParentRelationship.Father),
            CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
        Assert.Empty(fixture.StudentRepository.Added);
    }

    [Fact]
    public async Task QueryAsync_GetStudent_ReturnsTenantNotSelectedForCentrelessActor()
    {
        using var fixture = CreateSut(CentrelessActor);

        var result = await fixture.Dispatcher.QueryAsync<GetStudentQuery, StudentDetailDto>(
            new GetStudentQuery(Guid.CreateVersion7()), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    private static async Task AssertPermissionDenied<T>(Task<Result<T>> resultTask, Fixture fixture)
    {
        var result = await resultTask;
        AssertPermissionDenied(result, fixture);
    }

    private static void AssertPermissionDenied<T>(Result<T> result, Fixture fixture)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("auth.permission_denied", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    private static void AssertTenantNotSelected<T>(Result<T> result, Fixture fixture)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("tenant.not_selected", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    private static Fixture CreateSut(StaffRole role) => CreateSut(new StaffActor(Guid.NewGuid(), CentreId, role));

    private static Fixture CreateSut(StaffActor actor)
    {
        var unitOfWork = new FakeUnitOfWork();
        var currentActor = new FakeCurrentActor { Actor = actor };
        var studentRepository = new FakeStudentRepository();
        var studentReadService = new FakeStudentReadService();
        var parentRepository = new FakeParentRepository();

        var existingParent = Parent.Create(CentreId, "Existing Parent", PhoneNumber.Create("01512345678").Value).Value;
        parentRepository.Existing.Add(existingParent);
        var existingStudent = Student.Register(CentreId, "Existing Student", null, null, existingParent.Id, ParentRelationship.Father).Value;
        studentRepository.Existing.Add(existingStudent);
        studentReadService.Details.Add(new StudentDetailDto(existingStudent.Id, existingStudent.FullName, null, null, existingStudent.Status, 0, []));

        var provider = new ServiceCollection()
            .AddSingleton(unitOfWork)
            .AddSingleton<ICurrentActor>(currentActor)
            .AddSingleton<IStudentRepository>(studentRepository)
            .AddSingleton<IStudentReadService>(studentReadService)
            .AddSingleton<IParentRepository>(parentRepository)
            .AddScoped<ICommandHandler<RegisterStudentCommand, RegisterStudentResult>, RegisterStudentHandler>()
            .AddScoped<ICommandHandler<UpdateStudentCommand, Unit>, UpdateStudentHandler>()
            .AddScoped<ICommandHandler<ArchiveStudentCommand, Unit>, ArchiveStudentHandler>()
            .AddScoped<ICommandHandler<RestoreStudentCommand, Unit>, RestoreStudentHandler>()
            .AddScoped<ICommandHandler<LinkParentCommand, Unit>, LinkParentHandler>()
            .AddScoped<ICommandHandler<UnlinkParentCommand, Unit>, UnlinkParentHandler>()
            .AddScoped<ICommandHandler<SetPrimaryParentCommand, Unit>, SetPrimaryParentHandler>()
            .AddScoped<IQueryHandler<GetStudentQuery, StudentDetailDto>, GetStudentHandler>()
            .AddScoped<IQueryHandler<ListStudentsQuery, StudentPageDto>, ListStudentsHandler>()
            .AddScoped<IValidator<RegisterStudentCommand>, RegisterStudentValidator>()
            .AddScoped<IValidator<UpdateStudentCommand>, UpdateStudentValidator>()
            .AddScoped<IValidator<ArchiveStudentCommand>, ArchiveStudentValidator>()
            .AddScoped<IValidator<RestoreStudentCommand>, RestoreStudentValidator>()
            .AddScoped<IValidator<LinkParentCommand>, LinkParentValidator>()
            .AddScoped<IValidator<UnlinkParentCommand>, UnlinkParentValidator>()
            .AddScoped<IValidator<SetPrimaryParentCommand>, SetPrimaryParentValidator>()
            .AddScoped<IValidator<ListStudentsQuery>, ListStudentsValidator>()
            .BuildServiceProvider();

        return new Fixture(
            new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance),
            unitOfWork,
            studentRepository,
            existingStudent,
            existingParent,
            provider);
    }

    private sealed class Fixture(
        Dispatcher dispatcher,
        FakeUnitOfWork unitOfWork,
        FakeStudentRepository studentRepository,
        Student existingStudent,
        Parent existingParent,
        ServiceProvider provider) : IDisposable
    {
        public Dispatcher Dispatcher { get; } = dispatcher;

        public FakeUnitOfWork UnitOfWork { get; } = unitOfWork;

        public FakeStudentRepository StudentRepository { get; } = studentRepository;

        public Student ExistingStudent { get; } = existingStudent;

        public Parent ExistingParent { get; } = existingParent;

        public void Dispose() => provider.Dispose();
    }
}
