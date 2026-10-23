using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateGroupHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownSubject_ReturnsSubjectNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", Guid.CreateVersion7(), sut.ActiveTeacher.Id, 20), CancellationToken.None);

        AssertFailure(result, "subject.not_found", ErrorKind.NotFound);
        Assert.Empty(sut.Groups.Added);
    }

    [Fact]
    public async Task HandleAsync_ArchivedSubject_ReturnsSubjectArchived()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", sut.ArchivedSubject.Id, sut.ActiveTeacher.Id, 20), CancellationToken.None);

        AssertFailure(result, "group.subject_archived", ErrorKind.Rule);
        Assert.Empty(sut.Groups.Added);
    }

    [Fact]
    public async Task HandleAsync_UnknownTeacher_ReturnsTeacherNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", sut.ActiveSubject.Id, Guid.CreateVersion7(), 20), CancellationToken.None);

        AssertFailure(result, "teacher.not_found", ErrorKind.NotFound);
        Assert.Empty(sut.Groups.Added);
    }

    [Fact]
    public async Task HandleAsync_ArchivedTeacher_ReturnsTeacherArchived()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", sut.ActiveSubject.Id, sut.ArchivedTeacher.Id, 20), CancellationToken.None);

        AssertFailure(result, "group.teacher_archived", ErrorKind.Rule);
        Assert.Empty(sut.Groups.Added);
    }

    [Fact]
    public async Task HandleAsync_ArchivedSubjectAndUnknownTeacher_ReportsTheSubjectFirst()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", sut.ArchivedSubject.Id, Guid.CreateVersion7(), 20), CancellationToken.None);

        AssertFailure(result, "group.subject_archived", ErrorKind.Rule);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public async Task HandleAsync_CapacityOutOfRange_ReturnsCapacityInvalid(int capacity)
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", sut.ActiveSubject.Id, sut.ActiveTeacher.Id, capacity), CancellationToken.None);

        AssertFailure(result, "group.capacity_invalid", ErrorKind.Validation);
        Assert.Empty(sut.Groups.Added);
    }

    [Fact]
    public async Task HandleAsync_NameAlreadyUsed_ReturnsNameTakenOnNameField()
    {
        var sut = CreateSut();
        sut.Groups.Existing.Add(Group.Create(NileCentreId, "Grade 10 A", sut.ActiveSubject.Id, sut.ActiveTeacher.Id, 20).Value);

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("  grade   10 a ", sut.ActiveSubject.Id, sut.ActiveTeacher.Id, 20), CancellationToken.None);

        AssertFailure(result, "group.name_taken", ErrorKind.Conflict);
        Assert.Contains("name", result.Error!.Fields!.Keys);
        Assert.Empty(sut.Groups.Added);
    }

    [Fact]
    public async Task HandleAsync_Valid_AddsGroupInTheActorsCentre()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", sut.ActiveSubject.Id, sut.ActiveTeacher.Id, 20), CancellationToken.None);

        Assert.True(result.IsSuccess);
        var added = Assert.Single(sut.Groups.Added);
        Assert.Equal(result.Value.GroupId, added.Id);
        Assert.Equal(NileCentreId, added.CentreId);
        Assert.Equal(sut.ActiveSubject.Id, added.SubjectId);
        Assert.Equal(sut.ActiveTeacher.Id, added.TeacherId);
        Assert.Equal(20, added.Capacity);
        Assert.Equal(GroupStatus.Active, added.Status);
    }

    private static void AssertFailure<T>(Result<T> result, string code, ErrorKind kind)
    {
        Assert.True(result.IsFailure);
        Assert.Equal(code, result.Error!.Code);
        Assert.Equal(kind, result.Error.Kind);
    }

    private static Sut CreateSut()
    {
        var subjects = new FakeSubjectRepository();
        var teachers = new FakeTeacherRepository();
        var groups = new FakeGroupRepository();

        var activeSubject = Subject.Create(NileCentreId, "Mathematics").Value;
        var archivedSubject = Subject.Create(NileCentreId, "Chemistry").Value;
        archivedSubject.Archive();
        subjects.Existing.Add(activeSubject);
        subjects.Existing.Add(archivedSubject);

        var activeTeacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var archivedTeacher = Teacher.Create(NileCentreId, "Mona Said", null).Value;
        archivedTeacher.Archive();
        teachers.Existing.Add(activeTeacher);
        teachers.Existing.Add(archivedTeacher);

        var currentActor = new FakeCurrentActor { Actor = new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner) };
        var handler = new CreateGroupHandler(currentActor, groups, subjects, teachers);
        return new Sut(handler, groups, activeSubject, archivedSubject, activeTeacher, archivedTeacher);
    }

    private sealed record Sut(
        CreateGroupHandler Handler,
        FakeGroupRepository Groups,
        Subject ActiveSubject,
        Subject ArchivedSubject,
        Teacher ActiveTeacher,
        Teacher ArchivedTeacher);
}
