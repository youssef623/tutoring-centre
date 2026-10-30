namespace TutoringCentre.Application.Academics.Contracts;

public interface IPortalDirectory
{
    Task<IReadOnlyList<string>> ListStudentNamesAsync(CancellationToken ct);
    Task<PortalSubjectFacts?> GetParentAsync(Guid parentId, CancellationToken ct);
    Task<PortalSubjectFacts?> GetStudentAsync(Guid studentId, CancellationToken ct);
    Task<IReadOnlyList<PortalChild>> GetChildrenAsync(Guid parentId, CancellationToken ct);
}
