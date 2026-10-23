using TutoringCentre.Application.Academics.Groups.Commands.ChangeGroupCapacity;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ChangeGroupCapacityHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var handler = new ChangeGroupCapacityHandler(new FakeGroupRepository());

        var result = await handler.HandleAsync(new ChangeGroupCapacityCommand(Guid.CreateVersion7(), 30, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_InRange_ChangesCapacity()
    {
        var (handler, group) = CreateSut();

        var result = await handler.HandleAsync(new ChangeGroupCapacityCommand(group.Id, 200, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(200, group.Capacity);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public async Task HandleAsync_OutOfRange_ReturnsCapacityInvalidAndKeepsCapacity(int capacity)
    {
        var (handler, group) = CreateSut();

        var result = await handler.HandleAsync(new ChangeGroupCapacityCommand(group.Id, capacity, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.capacity_invalid", result.Error!.Code);
        Assert.Contains("capacity", result.Error.Fields!.Keys);
        Assert.Equal(20, group.Capacity);
    }

    [Fact]
    public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()
    {
        var (handler, group) = CreateSut();
        group.Archive();

        var result = await handler.HandleAsync(new ChangeGroupCapacityCommand(group.Id, 30, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.archived_read_only", result.Error!.Code);
        Assert.Equal(20, group.Capacity);
    }

    private static (ChangeGroupCapacityHandler Handler, Group Group) CreateSut()
    {
        var group = Group.Create(NileCentreId, "Grade 10 A", Guid.CreateVersion7(), Guid.CreateVersion7(), 20).Value;
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        return (new ChangeGroupCapacityHandler(repository), group);
    }
}
