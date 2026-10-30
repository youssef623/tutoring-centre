using TutoringCentre.Application.Academics.Contracts;

namespace TutoringCentre.Application.Tests.Fakes;

public sealed class FakePortalDirectory : IPortalDirectory
{
    public Dictionary<Guid, PortalSubjectFacts> Parents { get; } = [];
    public Dictionary<Guid, PortalSubjectFacts> Students { get; } = [];
    public IReadOnlyList<PortalChild> Children { get; set; } = [];
    public int ChildrenReads { get; private set; }
    public Task<PortalSubjectFacts?> GetParentAsync(Guid parentId, CancellationToken ct) => Task.FromResult(Parents.GetValueOrDefault(parentId));
    public Task<PortalSubjectFacts?> GetStudentAsync(Guid studentId, CancellationToken ct) => Task.FromResult(Students.GetValueOrDefault(studentId));
    public Task<IReadOnlyList<PortalChild>> GetChildrenAsync(Guid parentId, CancellationToken ct)
    {
        ChildrenReads++;
        return Task.FromResult(Children);
    }
}
