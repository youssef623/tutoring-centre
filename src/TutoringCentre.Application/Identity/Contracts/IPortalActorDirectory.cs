namespace TutoringCentre.Application.Identity.Contracts;
/// <summary>Live account and subject linkage, scoped to the current centre.</summary>
public interface IPortalActorDirectory
{
    Task<bool> IsActiveAsync(Guid userId, Guid subjectId, bool isStudent, CancellationToken ct);
}
