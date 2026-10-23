using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>A settable stand-in for <see cref="ITeacherScopeProvider"/>; starts at None, failing closed like the real one.</summary>
public sealed class FakeTeacherScopeProvider : ITeacherScopeProvider
{
    public TeacherScope Scope { get; set; } = TeacherScope.None;

    public Task<TeacherScope> GetAsync(CancellationToken ct) => Task.FromResult(Scope);
}
