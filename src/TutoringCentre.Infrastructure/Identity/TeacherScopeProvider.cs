using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Infrastructure.Identity;

/// <summary>
/// Resolves the ownership guard for group-returning requests. A system actor or a role not limited to its
/// own groups (<see cref="RoleScopes.IsLimitedToOwnGroups"/>) gets <see cref="TeacherScope.All"/>; a limited
/// role resolves through <see cref="ITeacherReadService.FindIdByUserAsync"/> to its own teacher record, or
/// <see cref="TeacherScope.None"/> when none is linked or the linked record is archived (that lookup only
/// ever returns an active teacher's ID). Any other actor — anonymous, or staff with no selected role —
/// also gets <see cref="TeacherScope.None"/>: fails closed, never defaults to All on an unknown case.
/// Registered scoped, so the result is computed at most once per request.
/// </summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class TeacherScopeProvider(ICurrentActor currentActor, ITeacherReadService teachers) : ITeacherScopeProvider
{
    private TeacherScope? _cached;

    public async Task<TeacherScope> GetAsync(CancellationToken ct)
    {
        if (_cached is { } cached)
        {
            return cached;
        }

        var resolved = await ResolveAsync(ct);
        _cached = resolved;
        return resolved;
    }

    private async Task<TeacherScope> ResolveAsync(CancellationToken ct)
    {
        if (currentActor.Actor is not StaffActor staffActor || staffActor.Role is not { } role)
        {
            return currentActor.Actor is SystemActor ? TeacherScope.All : TeacherScope.None;
        }

        if (!RoleScopes.IsLimitedToOwnGroups(role))
        {
            return TeacherScope.All;
        }

        var teacherId = await teachers.FindIdByUserAsync(staffActor.UserId, ct);
        return teacherId is { } id ? TeacherScope.Own(id) : TeacherScope.None;
    }
}
