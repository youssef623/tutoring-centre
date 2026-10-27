using System.Reflection;
using TutoringCentre.Application.Academics;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;

namespace TutoringCentre.Application.Tests.Attendance;

public sealed class TeachingScopeAdapterTests
{
    [Theory]
    [InlineData("owner")]
    [InlineData("system")]
    public async Task UnrestrictedActor_PublishesAll(string actor)
    {
        Assert.True(actor is "owner" or "system");
        var result = await new TeachingScopeAdapter(new FakeTeacherScopeProvider { Scope = TeacherScope.All }).GetAsync(CancellationToken.None);
        Assert.Equal(TeachingScopeKind.All, result.Kind);
        Assert.Null(result.TeacherId);
        Assert.True(result.Allows(Guid.NewGuid()));
    }

    [Fact]
    public async Task LinkedTeacher_PublishesOwn_WithOnlyItsTeacherAllowed()
    {
        var teacher = Guid.NewGuid();
        var result = await new TeachingScopeAdapter(new FakeTeacherScopeProvider { Scope = TeacherScope.Own(teacher) }).GetAsync(CancellationToken.None);
        Assert.Equal(TeachingScopeKind.Own, result.Kind);
        Assert.Equal(teacher, result.TeacherId);
        Assert.True(result.Allows(teacher));
        Assert.False(result.Allows(Guid.NewGuid()));
    }

    [Fact]
    public async Task UnlinkedTeacher_PublishesNone()
    {
        var result = await new TeachingScopeAdapter(new FakeTeacherScopeProvider()).GetAsync(CancellationToken.None);
        Assert.Equal(TeachingScopeKind.None, result.Kind);
        Assert.Null(result.TeacherId);
        Assert.False(result.Allows(Guid.NewGuid()));
    }

    [Fact]
    public async Task Adapter_DoesNotCacheBeyondProvider()
    {
        var provider = new FakeTeacherScopeProvider { Scope = TeacherScope.All };
        var adapter = new TeachingScopeAdapter(provider);
        Assert.Equal(TeachingScope.All, await adapter.GetAsync(CancellationToken.None));
        provider.Scope = TeacherScope.None;
        Assert.Equal(TeachingScope.None, await adapter.GetAsync(CancellationToken.None));
    }

    [Fact]
    public async Task UnrecognisedProviderKind_FailsClosed()
    {
        var constructor = typeof(TeacherScope).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)
            .Single(candidate => candidate.GetParameters().Length == 2);
        var unknown = (TeacherScope)constructor.Invoke([(TeacherScopeKind)99, null]);
        var result = await new TeachingScopeAdapter(new FakeTeacherScopeProvider { Scope = unknown }).GetAsync(CancellationToken.None);
        Assert.Equal(TeachingScope.None, result);
    }

    [Fact]
    public async Task EmptyOwnTeacherId_FailsClosed()
    {
        var result = await new TeachingScopeAdapter(new FakeTeacherScopeProvider { Scope = TeacherScope.Own(Guid.Empty) }).GetAsync(CancellationToken.None);
        Assert.Equal(TeachingScope.None, result);
        Assert.False(result.Allows(Guid.Empty));
    }
}
