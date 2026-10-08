using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.ReadServices;

/// <summary>
/// Read side of Subject: flat, no-tracking projections straight to <see cref="SubjectDto"/>. No tenant
/// predicate written by hand — the EF query filter and row-level security (Week 1) scope these the same as any
/// other query. Ordering and the 500-row cap run in SQL, not in memory.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class SubjectReadService(AppDbContext db) : ISubjectReadService
{
    private const int ListCap = 500;

    public async Task<IReadOnlyList<SubjectDto>> ListAsync(bool includeArchived, CancellationToken ct) =>
        await Query()
            .Where(subject => includeArchived || subject.Status == SubjectStatus.Active)
            .OrderBy(subject => subject.Name)
            .Take(ListCap)
            .ToListAsync(ct);

    public Task<SubjectDto?> GetAsync(Guid id, CancellationToken ct) =>
        Query().SingleOrDefaultAsync(dto => dto.Id == id, ct);

    private IQueryable<SubjectDto> Query() =>
        db.Set<Subject>()
            .AsNoTracking()
            .Select(subject => new SubjectDto(subject.Id, subject.Name, subject.Status, EF.Property<uint>(subject, "Version")));
}
