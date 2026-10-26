using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory stand-in for group reads. Applies the scope the way the real adapter does (rows outside it are
/// invisible) and records the last criteria and scope it was asked for, so a handler test can assert both were
/// passed through.
/// </summary>
public sealed class FakeGroupReadService : IGroupReadService
{
    public List<GroupDto> Groups { get; } = [];

    public GroupListCriteria? LastCriteria { get; private set; }

    public TeacherScope? LastScope { get; private set; }

    public Task<IReadOnlyList<GroupDto>> ListAsync(GroupListCriteria criteria, TeacherScope scope, CancellationToken ct)
    {
        LastCriteria = criteria;
        LastScope = scope;
        return Task.FromResult<IReadOnlyList<GroupDto>>(Groups.Where(group => scope.Allows(group.TeacherId)).ToList());
    }

    public Task<GroupDto?> GetAsync(Guid id, TeacherScope scope, CancellationToken ct)
    {
        LastScope = scope;
        return Task.FromResult(Groups.FirstOrDefault(group => group.Id == id && scope.Allows(group.TeacherId)));
    }
}
