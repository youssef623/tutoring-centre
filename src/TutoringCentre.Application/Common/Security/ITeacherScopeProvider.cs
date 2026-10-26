namespace TutoringCentre.Application.Common.Security;

/// <summary>
/// Resolves the acting actor's <see cref="TeacherScope"/> for a group-returning request. The scope derives
/// only from the server-side actor and the database, never from the request. Registered scoped: the result is
/// cached for the lifetime of one request.
/// </summary>
public interface ITeacherScopeProvider
{
    Task<TeacherScope> GetAsync(CancellationToken ct);
}
