using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Enrollments;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Enrollments.Commands.TransferEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Commands.WithdrawEnrollment;
using TutoringCentre.Application.Academics.Enrollments.Queries.GetGroupRoster;
using TutoringCentre.Application.Academics.Enrollments.Queries.ListStudentEnrollments;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: tenancy and permission. Owner and Secretary hold enrollments.manage; Teacher holds
/// only enrollments.view, so the three commands are denied for Teacher before any handler or transaction runs,
/// while both queries are allowed (ownership narrows them later, in the read). An actor with no selected centre
/// is refused every enrollment request as tenant.not_selected.
/// </summary>
public sealed class EnrollmentDispatcherPermissionTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    public static TheoryData<string> CommandNames => new()
    {
        nameof(EnrollStudentCommand),
        nameof(WithdrawEnrollmentCommand),
        nameof(TransferEnrollmentCommand),
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
        Assert.Empty(fixture.Enrollments.Added);
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
    public async Task SendAsync_Enroll_SucceedsForOwnerAndSecretary(StaffRole role)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, role));

        var error = await SendAsync(fixture, nameof(EnrollStudentCommand));

        Assert.Null(error);
        Assert.Single(fixture.Enrollments.Added);
    }

    [Fact]
    public async Task QueryAsync_RosterAndStudentEnrollments_AreAllowedForTeacher()
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, StaffRole.Teacher));

        var roster = await fixture.Dispatcher.QueryAsync<GetGroupRosterQuery, GroupRosterDto>(
            new GetGroupRosterQuery(Guid.CreateVersion7()), CancellationToken.None);
        var list = await fixture.Dispatcher.QueryAsync<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>(
            new ListStudentEnrollmentsQuery(fixture.Student.Id), CancellationToken.None);

        // Past the permission step: the read itself answers (not found for an unknown group, a list for a known student).
        Assert.Equal("group.not_found", roster.Error!.Code);
        Assert.True(list.IsSuccess, list.Error?.Message);
    }

    [Fact]
    public async Task QueryAsync_RosterAndStudentEnrollments_ReturnTenantNotSelectedForACentrelessActor()
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), null, null));

        var roster = await fixture.Dispatcher.QueryAsync<GetGroupRosterQuery, GroupRosterDto>(
            new GetGroupRosterQuery(Guid.CreateVersion7()), CancellationToken.None);
        var list = await fixture.Dispatcher.QueryAsync<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>(
            new ListStudentEnrollmentsQuery(fixture.Student.Id), CancellationToken.None);

        Assert.Equal("tenant.not_selected", roster.Error!.Code);
        Assert.Equal("tenant.not_selected", list.Error!.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    private static async Task<Error?> SendAsync(Fixture fixture, string commandName)
    {
        var dispatcher = fixture.Dispatcher;
        Result result = commandName switch
        {
            nameof(EnrollStudentCommand) => await dispatcher.SendAsync<EnrollStudentCommand, EnrollStudentResult>(
                new EnrollStudentCommand(fixture.Target.Id, fixture.Student.Id), CancellationToken.None),
            nameof(WithdrawEnrollmentCommand) => await dispatcher.SendAsync<WithdrawEnrollmentCommand, Unit>(
                new WithdrawEnrollmentCommand(fixture.Current.Id, 0), CancellationToken.None),
            nameof(TransferEnrollmentCommand) => await dispatcher.SendAsync<TransferEnrollmentCommand, TransferEnrollmentResult>(
                new TransferEnrollmentCommand(fixture.Current.Id, fixture.Target.Id, 0), CancellationToken.None),
            _ => throw new ArgumentOutOfRangeException(nameof(commandName), commandName, null),
        };

        return result.Error;
    }

    private static Fixture CreateSut(Actor actor)
    {
        var unitOfWork = new FakeUnitOfWork();
        var groups = new FakeGroupRepository();
        var students = new FakeStudentRepository();
        var enrollments = new FakeEnrollmentRepository();
        var reads = new FakeEnrollmentReadService();

        var subjectId = Guid.CreateVersion7();
        var teacherId = Guid.CreateVersion7();
        var source = Group.Create(CentreId, "Grade 10 A", subjectId, teacherId, 20).Value;
        var target = Group.Create(CentreId, "Grade 10 B", subjectId, teacherId, 20).Value;
        var student = Student.Register(CentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        var current = Enrollment.Start(CentreId, Guid.CreateVersion7(), source.Id, DateTimeOffset.UtcNow.AddDays(-7)).Value;
        groups.Existing.Add(source);
        groups.Existing.Add(target);
        students.Existing.Add(student);
        enrollments.Existing.Add(current);
        reads.StudentIds.Add(student.Id);

        var provider = new ServiceCollection()
            .AddSingleton(unitOfWork)
            .AddSingleton<ICurrentActor>(new FakeCurrentActor { Actor = actor })
            .AddSingleton<IClock>(new FakeClock())
            .AddSingleton<IGroupRepository>(groups)
            .AddSingleton<IStudentRepository>(students)
            .AddSingleton<IEnrollmentRepository>(enrollments)
            .AddSingleton<IEnrollmentReadService>(reads)
            .AddSingleton<ITeacherScopeProvider>(new FakeTeacherScopeProvider { Scope = TeacherScope.None })
            .AddScoped<ICommandHandler<EnrollStudentCommand, EnrollStudentResult>, EnrollStudentHandler>()
            .AddScoped<ICommandHandler<WithdrawEnrollmentCommand, Unit>, WithdrawEnrollmentHandler>()
            .AddScoped<ICommandHandler<TransferEnrollmentCommand, TransferEnrollmentResult>, TransferEnrollmentHandler>()
            .AddScoped<IQueryHandler<GetGroupRosterQuery, GroupRosterDto>, GetGroupRosterHandler>()
            .AddScoped<IQueryHandler<ListStudentEnrollmentsQuery, IReadOnlyList<StudentEnrollmentDto>>, ListStudentEnrollmentsHandler>()
            .AddScoped<IValidator<EnrollStudentCommand>, EnrollStudentValidator>()
            .AddScoped<IValidator<WithdrawEnrollmentCommand>, WithdrawEnrollmentValidator>()
            .AddScoped<IValidator<TransferEnrollmentCommand>, TransferEnrollmentValidator>()
            .BuildServiceProvider();

        return new Fixture(
            new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance), unitOfWork, enrollments, target, student, current, provider);
    }

    private sealed class Fixture(
        Dispatcher dispatcher,
        FakeUnitOfWork unitOfWork,
        FakeEnrollmentRepository enrollments,
        Group target,
        Student student,
        Enrollment current,
        ServiceProvider provider) : IDisposable
    {
        public Dispatcher Dispatcher { get; } = dispatcher;

        public FakeUnitOfWork UnitOfWork { get; } = unitOfWork;

        public FakeEnrollmentRepository Enrollments { get; } = enrollments;

        public Group Target { get; } = target;

        public Student Student { get; } = student;

        public Enrollment Current { get; } = current;

        public void Dispose() => provider.Dispose();
    }
}
