using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;
using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;
using TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
using static TutoringCentre.Application.Tests.Academics.ScheduleTestData;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: tenancy and permission. Owner and Secretary hold schedule.manage; Teacher holds
/// only schedule.view, so the four commands are denied for Teacher before any handler or transaction runs, while
/// both queries are allowed (ownership narrows them later, in the read). An actor with no selected centre is
/// refused every schedule request as tenant.not_selected.
/// </summary>
public sealed class ScheduleDispatcherPermissionTests
{
    public static TheoryData<string> CommandNames => new()
    {
        nameof(AddScheduleRuleCommand),
        nameof(EndScheduleRuleCommand),
        nameof(ExtendScheduleCommand),
        nameof(CancelSessionCommand),
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
        Assert.Empty(fixture.Rules.Added);
        Assert.Empty(fixture.Sessions.Added);
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
    public async Task SendAsync_AddRule_SucceedsForOwnerAndSecretary(StaffRole role)
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, role));

        var error = await SendAsync(fixture, nameof(AddScheduleRuleCommand));

        Assert.Null(error);
        Assert.Single(fixture.Rules.Added);
    }

    [Fact]
    public async Task QueryAsync_TimetableAndRules_AreAllowedForTeacher()
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), CentreId, StaffRole.Teacher));

        var timetable = await fixture.Dispatcher.QueryAsync<GetTimetableQuery, TimetableDto>(
            new GetTimetableQuery(Today, null, null), CancellationToken.None);
        var rules = await fixture.Dispatcher.QueryAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            new ListGroupScheduleRulesQuery(Guid.CreateVersion7()), CancellationToken.None);

        // Past the permission step: the read itself answers (a timetable, and not found for an unknown group).
        Assert.True(timetable.IsSuccess, timetable.Error?.Message);
        Assert.Equal("group.not_found", rules.Error!.Code);
    }

    [Fact]
    public async Task QueryAsync_TimetableAndRules_ReturnTenantNotSelectedForACentrelessActor()
    {
        using var fixture = CreateSut(new StaffActor(Guid.CreateVersion7(), null, null));

        var timetable = await fixture.Dispatcher.QueryAsync<GetTimetableQuery, TimetableDto>(
            new GetTimetableQuery(Today, null, null), CancellationToken.None);
        var rules = await fixture.Dispatcher.QueryAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            new ListGroupScheduleRulesQuery(Guid.CreateVersion7()), CancellationToken.None);

        Assert.Equal("tenant.not_selected", timetable.Error!.Code);
        Assert.Equal("tenant.not_selected", rules.Error!.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    private static async Task<Error?> SendAsync(Fixture fixture, string commandName)
    {
        var dispatcher = fixture.Dispatcher;
        Result result = commandName switch
        {
            nameof(AddScheduleRuleCommand) => await dispatcher.SendAsync<AddScheduleRuleCommand, AddScheduleRuleResult>(
                new AddScheduleRuleCommand(fixture.Group.Id, DayOfWeek.Saturday, new TimeOnly(18, 0), 90, null, Today), CancellationToken.None),
            nameof(EndScheduleRuleCommand) => await dispatcher.SendAsync<EndScheduleRuleCommand, Unit>(
                new EndScheduleRuleCommand(Guid.CreateVersion7(), Today, 0), CancellationToken.None),
            nameof(ExtendScheduleCommand) => await dispatcher.SendAsync<ExtendScheduleCommand, ExtendScheduleResult>(
                new ExtendScheduleCommand(Today.AddDays(7)), CancellationToken.None),
            nameof(CancelSessionCommand) => await dispatcher.SendAsync<CancelSessionCommand, Unit>(
                new CancelSessionCommand(Guid.CreateVersion7(), 0), CancellationToken.None),
            _ => throw new ArgumentOutOfRangeException(nameof(commandName), commandName, null),
        };

        return result.Error;
    }

    private static Fixture CreateSut(Actor actor)
    {
        var unitOfWork = new FakeUnitOfWork();
        var groups = new FakeGroupRepository();
        var rules = new FakeScheduleRuleRepository();
        var sessions = new FakeClassSessionRepository();
        var group = NewGroup();
        groups.Existing.Add(group);

        var provider = new ServiceCollection()
            .AddSingleton(unitOfWork)
            .AddSingleton<ICurrentActor>(new FakeCurrentActor { Actor = actor })
            .AddSingleton<IClock>(Clock())
            .AddSingleton<ICentreTimeZoneProvider>(new FakeCentreTimeZoneProvider())
            .AddSingleton<IGroupRepository>(groups)
            .AddSingleton<IScheduleRuleRepository>(rules)
            .AddSingleton<IClassSessionRepository>(sessions)
            .AddSingleton<IScheduleReadService>(new FakeScheduleReadService())
            .AddSingleton<ITeacherScopeProvider>(new FakeTeacherScopeProvider { Scope = TeacherScope.None })
            .AddScoped<ICommandHandler<AddScheduleRuleCommand, AddScheduleRuleResult>, AddScheduleRuleHandler>()
            .AddScoped<ICommandHandler<EndScheduleRuleCommand, Unit>, EndScheduleRuleHandler>()
            .AddScoped<ICommandHandler<ExtendScheduleCommand, ExtendScheduleResult>, ExtendScheduleHandler>()
            .AddScoped<ICommandHandler<CancelSessionCommand, Unit>, CancelSessionHandler>()
            .AddScoped<IQueryHandler<GetTimetableQuery, TimetableDto>, GetTimetableHandler>()
            .AddScoped<IQueryHandler<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>, ListGroupScheduleRulesHandler>()
            .AddScoped<IValidator<AddScheduleRuleCommand>, AddScheduleRuleValidator>()
            .AddScoped<IValidator<EndScheduleRuleCommand>, EndScheduleRuleValidator>()
            .AddScoped<IValidator<ExtendScheduleCommand>, ExtendScheduleValidator>()
            .AddScoped<IValidator<CancelSessionCommand>, CancelSessionValidator>()
            .AddScoped<IValidator<GetTimetableQuery>, GetTimetableValidator>()
            .BuildServiceProvider();

        return new Fixture(new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance), unitOfWork, rules, sessions, group, provider);
    }

    private sealed class Fixture(
        Dispatcher dispatcher,
        FakeUnitOfWork unitOfWork,
        FakeScheduleRuleRepository rules,
        FakeClassSessionRepository sessions,
        Group group,
        ServiceProvider provider) : IDisposable
    {
        public Dispatcher Dispatcher { get; } = dispatcher;

        public FakeUnitOfWork UnitOfWork { get; } = unitOfWork;

        public FakeScheduleRuleRepository Rules { get; } = rules;

        public FakeClassSessionRepository Sessions { get; } = sessions;

        public Group Group { get; } = group;

        public void Dispose() => provider.Dispose();
    }
}
