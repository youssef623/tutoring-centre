using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveSubjectHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var repository = new FakeSubjectRepository();
        var handler = new ArchiveSubjectHandler(repository, new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveSubjectCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ActiveSubject_Archives()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new ArchiveSubjectHandler(repository, new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveSubjectCommand(subject.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SubjectStatus.Archived, subject.Status);
    }

    [Fact]
    public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        subject.Archive();
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var handler = new ArchiveSubjectHandler(repository, new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveSubjectCommand(subject.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.already_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_SubjectWithActiveGroup_ReturnsHasActiveGroupsAndStaysActive()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var groups = new FakeGroupRepository();
        groups.Existing.Add(Group.Create(NileCentreId, "Grade 10 A", subject.Id, Guid.CreateVersion7(), 20).Value);
        var handler = new ArchiveSubjectHandler(repository, groups);

        var result = await handler.HandleAsync(new ArchiveSubjectCommand(subject.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("subject.has_active_groups", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(SubjectStatus.Active, subject.Status);
    }

    [Fact]
    public async Task HandleAsync_SubjectWithOnlyArchivedGroups_Archives()
    {
        var subject = Subject.Create(NileCentreId, "Mathematics").Value;
        var repository = new FakeSubjectRepository();
        repository.Existing.Add(subject);
        var archivedGroup = Group.Create(NileCentreId, "Grade 10 A", subject.Id, Guid.CreateVersion7(), 20).Value;
        archivedGroup.Archive();
        var groups = new FakeGroupRepository();
        groups.Existing.Add(archivedGroup);
        var handler = new ArchiveSubjectHandler(repository, groups);

        var result = await handler.HandleAsync(new ArchiveSubjectCommand(subject.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(SubjectStatus.Archived, subject.Status);
    }
}
