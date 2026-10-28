using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Academics.Scheduling;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class AttendanceRosterSource(AppDbContext db, IClock clock) : IAttendanceRosterSource
{
    public Task<CardStudent?> GetCardStudentAsync(Guid studentId, CancellationToken ct) =>
        db.Set<Student>().AsNoTracking().Where(student => student.Id == studentId)
            .Select(student => new CardStudent(student.Id, student.FullName, student.Status == StudentStatus.Archived))
            .SingleOrDefaultAsync(ct);

    public async Task<CardGroup?> GetCardGroupAsync(Guid groupId, CancellationToken ct)
    {
        var group = await db.Set<Group>().AsNoTracking().Where(group => group.Id == groupId)
            .Select(group => new { group.Id, group.Name }).SingleOrDefaultAsync(ct);
        if (group is null) { return null; }
        var now = clock.UtcNow;
        var students = await (from enrollment in db.Set<Enrollment>().AsNoTracking()
                              join student in db.Set<Student>().AsNoTracking() on enrollment.StudentId equals student.Id
                              where enrollment.GroupId == groupId && enrollment.EnrolledAt <= now
                                  && (enrollment.EndedAt == null || enrollment.EndedAt > now)
                                  && student.Status == StudentStatus.Active
                              orderby student.SearchName, student.Id
                              select new CardGroupStudent(student.Id, student.FullName)).ToListAsync(ct);
        return new(group.Id, group.Name, students);
    }

    public Task<AttendanceSessionFacts?> GetSessionAsync(Guid sessionId, CancellationToken ct) =>
        SessionFacts(sessionId, null, null).SingleOrDefaultAsync(ct);

    public async Task<IReadOnlyList<ExpectedStudent>> GetExpectedStudentsAsync(Guid sessionId, CancellationToken ct) =>
        await (from session in db.Set<ClassSession>().AsNoTracking()
               join enrollment in db.Set<Enrollment>().AsNoTracking() on session.GroupId equals enrollment.GroupId
               join student in db.Set<Student>().AsNoTracking() on enrollment.StudentId equals student.Id
               where session.Id == sessionId && enrollment.EnrolledAt <= session.StartsAt
                   && (enrollment.EndedAt == null || enrollment.EndedAt > session.StartsAt)
               orderby student.SearchName, student.Id
               select new ExpectedStudent(student.Id, enrollment.Id, student.FullName)).ToListAsync(ct);

    public async Task<IReadOnlyList<SessionOnDate>> ListSessionsOnDateAsync(DateOnly localDate, Guid? teacherId, CancellationToken ct)
    {
        var facts = await SessionFacts(null, localDate, teacherId).ToListAsync(ct);
        return facts.OrderBy(session => session.StartsAt).ThenBy(session => session.SessionId)
            .Select(session => new SessionOnDate(session.SessionId, session.GroupId, session.GroupName, session.SubjectName,
                session.TeacherId, session.TeacherName, session.StartsAt, session.EndsAt, session.LocalDate, session.Room, session.IsCancelled))
            .ToArray();
    }

    private IQueryable<AttendanceSessionFacts> SessionFacts(Guid? sessionId, DateOnly? localDate, Guid? teacherId)
    {
        var sessions = db.Set<ClassSession>().AsNoTracking().AsQueryable();
        if (sessionId is { } id)
        {
            sessions = sessions.Where(session => session.Id == id);
        }

        if (localDate is { } date)
        {
            sessions = sessions.Where(session => session.LocalDate == date);
        }

        if (teacherId is { } selectedTeacher)
        {
            sessions = sessions.Where(session => session.TeacherId == selectedTeacher);
        }

        return from session in sessions
               join studyGroup in db.Set<Group>().AsNoTracking() on session.GroupId equals studyGroup.Id
               join subject in db.Set<Subject>().AsNoTracking() on studyGroup.SubjectId equals subject.Id
               join teacher in db.Set<Teacher>().AsNoTracking() on session.TeacherId equals teacher.Id
               select new AttendanceSessionFacts(session.Id, studyGroup.Id, studyGroup.Name, subject.Name, teacher.Id, teacher.FullName,
                   session.StartsAt, session.EndsAt, session.LocalDate, session.Room, session.Status == ClassSessionStatus.Cancelled);
    }
}
