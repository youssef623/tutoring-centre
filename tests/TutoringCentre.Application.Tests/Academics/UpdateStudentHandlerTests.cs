using TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class UpdateStudentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_Valid_ReplacesAllThreeFields()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, "Grade 9", Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new UpdateStudentHandler(students);

        var result = await handler.HandleAsync(
            new UpdateStudentCommand(student.Id, "Youssef Omar Hassan", "01012345678", "Grade 10", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Youssef Omar Hassan", student.FullName);
        Assert.Equal("+201012345678", student.Phone!.Value);
        Assert.Equal("Grade 10", student.SchoolYear);
    }

    [Fact]
    public async Task HandleAsync_OmittedPhoneAndSchoolYear_ClearsBoth()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", PhoneNumber.Create("01012345678").Value, "Grade 9", Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new UpdateStudentHandler(students);

        var result = await handler.HandleAsync(new UpdateStudentCommand(student.Id, "Youssef Omar", null, null, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(student.Phone);
        Assert.Null(student.SchoolYear);
    }

    [Fact]
    public async Task HandleAsync_UnknownStudentId_ReturnsStudentNotFound()
    {
        var handler = new UpdateStudentHandler(new FakeStudentRepository());

        var result = await handler.HandleAsync(new UpdateStudentCommand(Guid.CreateVersion7(), "Youssef Omar", null, null, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_InvalidPhone_ReturnsPhoneInvalidWithField()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new UpdateStudentHandler(students);

        var result = await handler.HandleAsync(new UpdateStudentCommand(student.Id, "Youssef Omar", "12345", null, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("phone", result.Error!.Fields!.Keys);
    }

    [Fact]
    public async Task HandleAsync_ArchivedStudent_ReturnsArchivedReadOnly()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        student.Archive();
        students.Existing.Add(student);
        var handler = new UpdateStudentHandler(students);

        var result = await handler.HandleAsync(new UpdateStudentCommand(student.Id, "Youssef Omar Hassan", null, null, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.archived_read_only", result.Error!.Code);
    }
}
