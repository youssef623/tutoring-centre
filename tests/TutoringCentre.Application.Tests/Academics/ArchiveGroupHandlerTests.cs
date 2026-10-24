using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveGroupHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var handler = new ArchiveGroupHandler(new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveGroupCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ActiveGroup_Archives()
    {
        var group = NewGroup();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = new ArchiveGroupHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(GroupStatus.Archived, group.Status);
    }

    [Fact]
    public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()
    {
        var group = NewGroup();
        group.Archive();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = new ArchiveGroupHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.already_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_GroupWithActiveEnrollments_ReturnsHasActiveEnrollmentsAndStaysActive()
    {
        var group = NewGroup();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        repository.ActiveEnrollmentCounts[group.Id] = 1;
        var handler = new ArchiveGroupHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.has_active_enrollments", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(GroupStatus.Active, group.Status);
    }

    [Fact]
    public async Task HandleAsync_StaleVersion_ReturnsConcurrencyStaleAndStaysActive()
    {
        var group = NewGroup();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        repository.Versions[group.Id] = 3;
        var handler = new ArchiveGroupHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 2), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("concurrency.stale", result.Error!.Code);
        Assert.Equal(GroupStatus.Active, group.Status);
    }

    private static Group NewGroup() =>
        Group.Create(NileCentreId, "Grade 10 A", Guid.CreateVersion7(), Guid.CreateVersion7(), 20).Value;
}
