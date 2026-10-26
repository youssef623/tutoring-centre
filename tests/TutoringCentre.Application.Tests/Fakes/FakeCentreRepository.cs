using TutoringCentre.Application.Centres;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory centre repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add calls.</summary>
public sealed class FakeCentreRepository : ICentreRepository
{
    public List<Centre> Existing { get; } = [];

    public List<Centre> Added { get; } = [];

    public Task<bool> ExistsBySlugAsync(string slug, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).Any(centre => centre.Slug == slug));

    public Task<Centre?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).SingleOrDefault(centre => centre.Id == id));

    public void Add(Centre centre) => Added.Add(centre);
}
