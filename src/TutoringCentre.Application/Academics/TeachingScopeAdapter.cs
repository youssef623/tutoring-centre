using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Academics;

internal sealed class TeachingScopeAdapter(ITeacherScopeProvider provider) : ITeachingScope
{
    public async Task<TeachingScope> GetAsync(CancellationToken ct) => await provider.GetAsync(ct) switch
    {
        { Kind: TeacherScopeKind.All } => TeachingScope.All,
        { Kind: TeacherScopeKind.Own, TeacherId: { } teacherId } => TeachingScope.Own(teacherId),
        _ => TeachingScope.None,
    };
}
