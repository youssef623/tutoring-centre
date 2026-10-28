using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class StudentCardReadService(AppDbContext db) : IStudentCardReadService
{
    public async Task<StudentCardDto?> GetMostRecentAsync(Guid studentId, CancellationToken ct)
    {
        var row = await (from card in db.Set<StudentCard>().AsNoTracking()
                         join user in db.Set<ApplicationUser>().AsNoTracking() on card.IssuedByUserId equals user.Id
                         where card.StudentId == studentId
                         orderby card.IssuedAt descending, card.Id descending
                         select new { card.Id, card.Status, card.IssuedAt, user.DisplayName,
                             card.RevocationReason, card.RevokedAt, Version = EF.Property<uint>(card, "Version") }).FirstOrDefaultAsync(ct);
        return row is null ? null : new(row.Id, row.Status.ToString().ToLowerInvariant(), row.IssuedAt, row.DisplayName,
            row.RevocationReason?.ToString().ToLowerInvariant(), row.RevokedAt, row.Version);
    }
}
