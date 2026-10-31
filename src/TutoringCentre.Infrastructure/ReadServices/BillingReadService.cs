using TutoringCentre.Domain.Common;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Billing;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Billing;
using TutoringCentre.Infrastructure.Persistence;
namespace TutoringCentre.Infrastructure.ReadServices;
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class BillingReadService(AppDbContext db, ICurrentActor actor, IBillingRosterSource roster) : IBillingReadService
{
    public async Task<StudentChargesPage> GetAsync(Guid studentId, string? cursor, int pageSize, CancellationToken ct)
    {
        var centre = actor.Actor.CentreId ?? throw new InvalidOperationException("A centre is required.");
        var query = db.Set<Charge>().AsNoTracking().Where(x => x.CentreId == centre && x.StudentId == studentId);
        if (cursor is not null && ChargeCursor.TryDecode(cursor, out var after))
            query = query.Where(x => x.BillingMonth < after.Month || (x.BillingMonth == after.Month && x.Id.CompareTo(after.Id) < 0));
        var rows = await query.OrderByDescending(x => x.BillingMonth).ThenByDescending(x => x.Id).Take(pageSize + 1).ToListAsync(ct);
        var more = rows.Count > pageSize; var page = rows.Take(pageSize).ToArray();
        var names = await roster.GetGroupNamesAsync(page.Select(x => x.GroupId).Distinct().ToArray(), ct);
        var items = page.Select(x => new StudentChargeDto(x.Id, x.BillingMonth.ToString(), x.Kind.ToString().ToLowerInvariant(),
            names[x.GroupId].SubjectName, names[x.GroupId].GroupName, x.Amount.ToString(), x.DueDate)).ToArray();
        return new(items, more ? ChargeCursor.Encode(page[^1].BillingMonth, page[^1].Id) : null);
    }
}
