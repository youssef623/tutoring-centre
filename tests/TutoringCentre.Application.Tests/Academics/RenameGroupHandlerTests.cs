using TutoringCentre.Application.Academics.Groups.Commands.RenameGroup;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RenameGroupHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var handler = new RenameGroupHandler(new FakeGroupRepository(), new FakeClock(), new FakeCentreTimeZoneProvider());

        var result = await handler.HandleAsync(new RenameGroupCommand(Guid.CreateVersion7(), "Grade 10 B", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_NewUniqueName_Renames()
    {
        var group = NewGroup("Grade 10 A");
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = new RenameGroupHandler(repository, new FakeClock(), new FakeCentreTimeZoneProvider());

        var result = await handler.HandleAsync(new RenameGroupCommand(group.Id, "Grade 10 B", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Grade 10 B", group.Name);
    }

    [Fact]
    public async Task HandleAsync_SameName_SucceedsWithNoChange()
    {
        var group = NewGroup("Grade 10 A");
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = new RenameGroupHandler(repository, new FakeClock(), new FakeCentreTimeZoneProvider());

        var result = await handler.HandleAsync(new RenameGroupCommand(group.Id, "Grade 10 A", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Grade 10 A", group.Name);
    }

    [Fact]
    public async Task HandleAsync_NameUsedByAnotherGroup_ReturnsNameTaken()
    {
        var group = NewGroup("Grade 10 A");
        var other = NewGroup("Grade 10 B");
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        repository.Existing.Add(other);
        var handler = new RenameGroupHandler(repository, new FakeClock(), new FakeCentreTimeZoneProvider());

        var result = await handler.HandleAsync(new RenameGroupCommand(group.Id, "grade 10 b", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.name_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Contains("name", result.Error.Fields!.Keys);
    }

    [Fact]
    public async Task HandleAsync_ArchivedGroup_ReturnsArchivedReadOnly()
    {
        var group = NewGroup("Grade 10 A");
        group.Archive();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = new RenameGroupHandler(repository, new FakeClock(), new FakeCentreTimeZoneProvider());

        var result = await handler.HandleAsync(new RenameGroupCommand(group.Id, "Grade 10 B", 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.archived_read_only", result.Error!.Code);
        Assert.Equal("Grade 10 A", group.Name);
    }

    private static Group NewGroup(string name) =>
        Group.Create(NileCentreId, name, Guid.CreateVersion7(), Guid.CreateVersion7(), 20).Value;
}
