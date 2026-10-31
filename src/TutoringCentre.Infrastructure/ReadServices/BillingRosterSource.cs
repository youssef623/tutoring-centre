using TutoringCentre.Domain.Common;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Billing;
using TutoringCentre.Infrastructure.Persistence;
namespace TutoringCentre.Infrastructure.ReadServices;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class BillingRosterSource(AppDbContext db, ICurrentActor actor, ICentreTimeZoneProvider zones) : IBillingRosterSource
{
    public async Task<IReadOnlyList<ChargeableEnrollment>> ListChargeableAsync(BillingMonth month, CancellationToken ct)
    {
        var centre = actor.Actor.CentreId ?? throw new InvalidOperationException("A centre is required.");
        var zone = await zones.GetForCurrentCentreAsync(ct);
        var start = new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(month.FirstDay.ToDateTime(TimeOnly.MinValue), zone));
        var end = new DateTimeOffset(TimeZoneInfo.ConvertTimeToUtc(month.Next().FirstDay.ToDateTime(TimeOnly.MinValue), zone));
        // Enrollment intervals are [enrolled_at, ended_at), intersected with the centre-local month.
        var candidates = from enrollment in db.Set<Enrollment>().AsNoTracking()
                         join groupRow in db.Set<Group>().AsNoTracking() on enrollment.GroupId equals groupRow.Id
                         join student in db.Set<Student>().AsNoTracking() on enrollment.StudentId equals student.Id
                         where enrollment.CentreId == centre && groupRow.CentreId == centre && student.CentreId == centre
                             && enrollment.EnrolledAt < end && (enrollment.EndedAt == null || enrollment.EndedAt > start)
                             && student.Status == StudentStatus.Active
                         select new { enrollment.Id, enrollment.StudentId, enrollment.EnrolledAt, groupRow.SubjectId, groupRow.TeacherId, GroupId = groupRow.Id, groupRow.Status };
        var rows = await candidates.Where(x => !candidates.Any(y => y.StudentId == x.StudentId && y.SubjectId == x.SubjectId
            && (y.EnrolledAt > x.EnrolledAt || (y.EnrolledAt == x.EnrolledAt && y.Id.CompareTo(x.Id) > 0)))).ToListAsync(ct);
        var groupIds = rows.Select(x => x.GroupId).Distinct().ToArray();
        var fees = await db.Set<GroupMonthlyFee>().AsNoTracking().Where(x => x.CentreId == centre
            && groupIds.Contains(x.GroupId) && x.EffectiveMonth <= month).ToListAsync(ct);
        var selectedFees = fees.GroupBy(x => x.GroupId).ToDictionary(x => x.Key, x => x.MaxBy(f => f.EffectiveMonth.FirstDay)!.Fee);
        return rows.Where(x => x.Status == GroupStatus.Active && selectedFees.TryGetValue(x.GroupId, out var fee) && fee.Amount > 0)
            .Select(x => new ChargeableEnrollment(x.Id, x.StudentId, x.GroupId, x.SubjectId, x.TeacherId, selectedFees[x.GroupId])).ToArray();
    }
    public async Task<ChargeableEnrollment?> GetChargeableAsync(Guid enrollmentId, BillingMonth month, CancellationToken ct) =>
        (await ListChargeableAsync(month, ct)).SingleOrDefault(x => x.EnrollmentId == enrollmentId);
    public Task<bool> StudentExistsAsync(Guid studentId, CancellationToken ct) => db.Set<Student>().AsNoTracking()
        .AnyAsync(x => x.CentreId == actor.Actor.CentreId && x.Id == studentId, ct);
    public async Task<IReadOnlyDictionary<Guid, BillingNames>> GetGroupNamesAsync(IReadOnlyCollection<Guid> groupIds, CancellationToken ct) =>
        await (from g in db.Set<Group>().AsNoTracking() join s in db.Set<Subject>().AsNoTracking() on g.SubjectId equals s.Id
               where g.CentreId == actor.Actor.CentreId && s.CentreId == actor.Actor.CentreId && groupIds.Contains(g.Id)
               select new { g.Id, g.Name, SubjectName = s.Name }).ToDictionaryAsync(x => x.Id, x => new BillingNames(x.SubjectName, x.Name), ct);
}
