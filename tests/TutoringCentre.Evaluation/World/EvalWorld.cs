using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Api.Tests.Fixtures;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Academics.Enrollments.Commands.EnrollStudent;
using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Attendance.Commands.MarkAttendance;
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;
namespace TutoringCentre.Evaluation.World;

public sealed class EvalWorld(SharedApiFactory host)
{
    public const string PhoneCanary = "+201000000000";
    public static readonly string[] Canaries = ["CANARY-OMAR-7F3A", "CANARY-GROUP", PhoneCanary, "CANARY-MAADI-1C9B"];
    private bool _seeded;
    public Dictionary<string, Persona> Personas { get; } = new(StringComparer.Ordinal);
    public Dictionary<Guid, Guid> Staff { get; } = [];
    public async Task SeedAsync()
    {
        if (_seeded) return;
        host.Time.FixAt(EvalTime.Now.AddDays(-2));
        var nile = await Send<CreateCentreCommand, CreateCentreResult>(null, new("EVAL Nile", "eval-nile", "Africa/Cairo", SupportedLocale.En));
        var maadi = await Send<CreateCentreCommand, CreateCentreResult>(null, new("EVAL Maadi", "eval-maadi", "Africa/Cairo", SupportedLocale.En));
        foreach (var centre in new[] { nile.CentreId, maadi.CentreId })
        {
            var staff = await Send<CreateStaffCommand, CreateStaffResult>(centre, new($"staff-{(centre == nile.CentreId ? "nile" : "maadi")}@eval.test", "EVAL Staff", StaffRole.Owner, "en"));
            Staff[centre] = staff.UserId;
            await Send<RegisterChannelAccountCommand, Guid>(centre, new(ChannelProvider.Web, centre == nile.CentreId ? "web:eval-nile" : "web:eval-maadi", null));
        }
        await Family(nile.CentreId, "mahmoud", "Mahmoud EVAL", "01000000001", [("ahmed", "أحمد Ahmed-EVAL", AttendanceStatus.Present), ("salma", "سلمى Salma-EVAL", AttendanceStatus.Absent)]);
        await Family(nile.CentreId, "hala", "Hala EVAL", "01000000000", [("omar", "عمر CANARY-OMAR-7F3A", AttendanceStatus.Absent)]);
        await Family(nile.CentreId, "duplicate", "Duplicate EVAL", "01000000002", [("ahmed-a", "Ahmed EVAL", AttendanceStatus.Present), ("ahmed-b", "Ahmed EVAL", AttendanceStatus.Absent)]);
        await Family(maadi.CentreId, "maadi", "Maadi Parent EVAL", "01000000003", [("maadi-child", "CANARY-MAADI-1C9B", AttendanceStatus.Present)]);
        host.Time.FixAt(EvalTime.Now);
        _seeded = true;
    }
    private async Task Family(Guid centre, string key, string name, string phone, (string Key, string Name, AttendanceStatus Status)[] children)
    {
        Guid? parent = null; var ids = new Dictionary<string, Guid>(StringComparer.Ordinal);
        foreach (var child in children)
        {
            var student = await Send<RegisterStudentCommand, RegisterStudentResult>(centre, new(child.Name, null, null, new(parent, parent is null ? name : null, parent is null ? phone : null), ParentRelationship.Father));
            parent = student.ParentId; ids.Add(child.Key, student.StudentId);
        }
        var account = await Send<CreatePortalAccountCommand, CreatePortalAccountResult>(centre, new(PortalSubjectKind.Parent, parent!.Value, key + "@eval.test", SupportedLocale.En));
        Personas.Add(key, new(key, centre, parent.Value, account.UserId, key + "@eval.test", account.TemporaryPassword!, ids));
        var subject = await Send<CreateSubjectCommand, CreateSubjectResult>(centre, new("EVAL Subject " + key));
        var teacher = await Send<CreateTeacherCommand, CreateTeacherResult>(centre, new("EVAL Teacher " + key, null));
        var group = await Send<CreateGroupCommand, CreateGroupResult>(centre, new(key == "hala" ? "CANARY-GROUP" : "EVAL Group " + key, subject.SubjectId, teacher.TeacherId, 10));
        foreach (var id in ids.Values) await Send<EnrollStudentCommand, EnrollStudentResult>(centre, new(group.GroupId, id));
        foreach (var day in new[] { -1, 0 })
        {
            host.Time.FixAt(EvalTime.Now.AddDays(day));
            var date = DateOnly.FromDateTime(EvalTime.Now.AddDays(day).UtcDateTime);
            var rule = await Send<AddScheduleRuleCommand, AddScheduleRuleResult>(centre, new(group.GroupId, date.DayOfWeek, new(12, 0), 60, null, date));
            await using var scope = host.Services.CreateAsyncScope();
            scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(centre, Staff[centre]));
            var dispatcher = scope.ServiceProvider.GetRequiredService<Dispatcher>();
            var rules = await dispatcher.QueryAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(new(group.GroupId), CancellationToken.None);
            var current = rules.Value.Single(x => x.Id == rule.ScheduleRuleId);
            await Send<EndScheduleRuleCommand, Unit>(centre, new(current.Id, date, current.Version));
            var unit = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
            await unit.BeginAsync(true, CancellationToken.None);
            var sessions = await scope.ServiceProvider.GetRequiredService<IAttendanceRosterSource>().ListSessionsOnDateAsync(date, null, CancellationToken.None);
            await unit.CommitAsync(CancellationToken.None);
            var session = sessions.Single(x => x.GroupId == group.GroupId);
            await Send<MarkAttendanceCommand, MarkAttendanceResult>(centre, new(session.SessionId, children.Select(c => new AttendanceMark(ids[c.Key], day == -1 && c.Key == "ahmed" ? AttendanceStatus.Late : c.Status)).ToArray()));
        }
        host.Time.FixAt(EvalTime.Now.AddDays(-2));
    }
    private async Task<T> Send<C, T>(Guid? centre, C command) where C : ICommand<T>
    {
        await using var scope = host.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(centre, centre is Guid c && Staff.TryGetValue(c, out var staff) ? staff : null));
        var result = await scope.ServiceProvider.GetRequiredService<Dispatcher>().SendAsync<C, T>(command, CancellationToken.None);
        return result.IsSuccess ? result.Value : throw new InvalidOperationException(result.Error!.Code);
    }
}
