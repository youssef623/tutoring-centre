using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Groups.Queries.GetGroup;
using TutoringCentre.Application.Academics.Groups.Queries.ListGroups;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ListGroupsHandlerTests
{
    [Fact]
    public async Task HandleAsync_PassesTheProvidersScopeAndTheFiltersToTheReadService()
    {
        var teacherId = Guid.CreateVersion7();
        var subjectId = Guid.CreateVersion7();
        var scopeProvider = new FakeTeacherScopeProvider { Scope = TeacherScope.Own(teacherId) };
        var readService = new FakeGroupReadService();
        var handler = new ListGroupsHandler(scopeProvider, readService);

        var result = await handler.HandleAsync(new ListGroupsQuery(subjectId, teacherId, true), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(TeacherScope.Own(teacherId), readService.LastScope);
        Assert.Equal(new GroupListCriteria(subjectId, teacherId, true), readService.LastCriteria);
    }

    [Fact]
    public async Task HandleAsync_OwnScope_ReturnsOnlyThatTeachersGroups()
    {
        var ownTeacherId = Guid.CreateVersion7();
        var readService = new FakeGroupReadService();
        readService.Groups.Add(GroupDtos.For(ownTeacherId));
        readService.Groups.Add(GroupDtos.For(Guid.CreateVersion7()));
        var handler = new ListGroupsHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.Own(ownTeacherId) }, readService);

        var result = await handler.HandleAsync(new ListGroupsQuery(null, null, false), CancellationToken.None);

        var only = Assert.Single(result.Value);
        Assert.Equal(ownTeacherId, only.TeacherId);
    }
}

public sealed class GetGroupHandlerTests
{
    [Fact]
    public async Task HandleAsync_InScope_ReturnsTheGroup()
    {
        var group = GroupDtos.For(Guid.CreateVersion7());
        var readService = new FakeGroupReadService();
        readService.Groups.Add(group);
        var handler = new GetGroupHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.All }, readService);

        var result = await handler.HandleAsync(new GetGroupQuery(group.Id), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(group, result.Value);
    }

    [Fact]
    public async Task HandleAsync_OutOfScope_ReturnsNotFound()
    {
        var group = GroupDtos.For(Guid.CreateVersion7());
        var readService = new FakeGroupReadService();
        readService.Groups.Add(group);
        var handler = new GetGroupHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.Own(Guid.CreateVersion7()) }, readService);

        var result = await handler.HandleAsync(new GetGroupQuery(group.Id), CancellationToken.None);

        AssertNotFound(result);
    }

    [Fact]
    public async Task HandleAsync_NoneScope_ReturnsNotFound()
    {
        var group = GroupDtos.For(Guid.CreateVersion7());
        var readService = new FakeGroupReadService();
        readService.Groups.Add(group);
        var handler = new GetGroupHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.None }, readService);

        var result = await handler.HandleAsync(new GetGroupQuery(group.Id), CancellationToken.None);

        AssertNotFound(result);
    }

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var handler = new GetGroupHandler(new FakeTeacherScopeProvider { Scope = TeacherScope.All }, new FakeGroupReadService());

        var result = await handler.HandleAsync(new GetGroupQuery(Guid.CreateVersion7()), CancellationToken.None);

        AssertNotFound(result);
    }

    private static void AssertNotFound(Result<GroupDto> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }
}

internal static class GroupDtos
{
    public static GroupDto For(Guid teacherId) => new(
        Guid.CreateVersion7(), "Grade 10 A", Guid.CreateVersion7(), "Mathematics", teacherId, "Amina Hassan", 20, 0, GroupStatus.Active, 0);
}
