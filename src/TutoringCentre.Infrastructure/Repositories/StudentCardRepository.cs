using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Repositories;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class StudentCardRepository(AppDbContext db) : IStudentCardRepository
{
    public Task<StudentCard?> GetActiveAsync(Guid studentId, CancellationToken ct) =>
        db.Set<StudentCard>().SingleOrDefaultAsync(card => card.StudentId == studentId && card.Status == StudentCardStatus.Active, ct);

    public async Task<IReadOnlyList<StudentCard>> GetActiveAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken ct) =>
        await db.Set<StudentCard>().Where(card => studentIds.Contains(card.StudentId) && card.Status == StudentCardStatus.Active)
            .OrderBy(card => card.StudentId).ToListAsync(ct);

    public async Task<StudentCard?> GetByIdAsync(Guid cardId, uint expectedVersion, CancellationToken ct)
    {
        var card = await db.Set<StudentCard>().SingleOrDefaultAsync(card => card.Id == cardId, ct);
        if (card is not null) { db.Entry(card).Property("Version").OriginalValue = expectedVersion; }
        return card;
    }

    public Task<StudentCard?> FindByTokenHashAsync(byte[] tokenHash, CancellationToken ct) =>
        db.Set<StudentCard>().SingleOrDefaultAsync(card => card.TokenHash == tokenHash, ct);

    public void Add(StudentCard card) => db.Add(card);
}
