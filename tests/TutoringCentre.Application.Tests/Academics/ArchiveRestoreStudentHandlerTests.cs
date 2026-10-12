using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;
using TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveRestoreStudentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task Archive_ActiveStudent_Succeeds()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new ArchiveStudentHandler(students);

        var result = await handler.HandleAsync(new ArchiveStudentCommand(student.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(StudentStatus.Archived, student.Status);
    }

    [Fact]
    public async Task Archive_AlreadyArchived_ReturnsAlreadyArchived()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        student.Archive();
        students.Existing.Add(student);
        var handler = new ArchiveStudentHandler(students);

        var result = await handler.HandleAsync(new ArchiveStudentCommand(student.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.already_archived", result.Error!.Code);
    }

    [Fact]
    public async Task Archive_UnknownStudentId_ReturnsStudentNotFound()
    {
        var handler = new ArchiveStudentHandler(new FakeStudentRepository());

        var result = await handler.HandleAsync(new ArchiveStudentCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task Restore_ArchivedStudent_Succeeds()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        student.Archive();
        students.Existing.Add(student);
        var handler = new RestoreStudentHandler(students);

        var result = await handler.HandleAsync(new RestoreStudentCommand(student.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(StudentStatus.Active, student.Status);
    }

    [Fact]
    public async Task Restore_AlreadyActive_ReturnsNotArchived()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new RestoreStudentHandler(students);

        var result = await handler.HandleAsync(new RestoreStudentCommand(student.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.not_archived", result.Error!.Code);
    }

    [Fact]
    public async Task Restore_UnknownStudentId_ReturnsStudentNotFound()
    {
        var handler = new RestoreStudentHandler(new FakeStudentRepository());

        var result = await handler.HandleAsync(new RestoreStudentCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.not_found", result.Error!.Code);
    }
}
