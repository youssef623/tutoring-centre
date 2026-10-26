using TutoringCentre.Application.Academics.Students.Commands.LinkParent;
using TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;
using TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ParentLinkHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task LinkParent_ExistingParentNotYetLinked_Succeeds()
    {
        var students = new FakeStudentRepository();
        var parents = new FakeParentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var parent = Parent.Create(NileCentreId, "Amal Fathy", PhoneNumber.Create("01212345678").Value).Value;
        parents.Existing.Add(parent);
        var handler = new LinkParentHandler(students, parents);

        var result = await handler.HandleAsync(new LinkParentCommand(student.Id, parent.Id, ParentRelationship.Mother, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, student.Parents.Count);
    }

    [Fact]
    public async Task LinkParent_UnknownParentId_ReturnsParentNotFound()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new LinkParentHandler(students, new FakeParentRepository());

        var result = await handler.HandleAsync(new LinkParentCommand(student.Id, Guid.CreateVersion7(), ParentRelationship.Mother, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task LinkParent_AlreadyLinked_ReturnsAlreadyLinked()
    {
        var students = new FakeStudentRepository();
        var parents = new FakeParentRepository();
        var parent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        parents.Existing.Add(parent);
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, parent.Id, ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new LinkParentHandler(students, parents);

        var result = await handler.HandleAsync(new LinkParentCommand(student.Id, parent.Id, ParentRelationship.Mother, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.parent_already_linked", result.Error!.Code);
    }

    [Fact]
    public async Task UnlinkParent_NonPrimarySecondParent_Succeeds()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        var secondParentId = Guid.CreateVersion7();
        student.AddParent(secondParentId, ParentRelationship.Mother);
        students.Existing.Add(student);
        var handler = new UnlinkParentHandler(students);

        var result = await handler.HandleAsync(new UnlinkParentCommand(student.Id, secondParentId, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(student.Parents);
    }

    [Fact]
    public async Task UnlinkParent_LastParent_ReturnsLastParentRule()
    {
        var students = new FakeStudentRepository();
        var primaryParentId = Guid.CreateVersion7();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, primaryParentId, ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new UnlinkParentHandler(students);

        var result = await handler.HandleAsync(new UnlinkParentCommand(student.Id, primaryParentId, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.last_parent", result.Error!.Code);
    }

    [Fact]
    public async Task UnlinkParent_NotLinked_ReturnsNotLinked()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new UnlinkParentHandler(students);

        var result = await handler.HandleAsync(new UnlinkParentCommand(student.Id, Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.parent_not_linked", result.Error!.Code);
    }

    [Fact]
    public async Task SetPrimaryParent_AlreadyLinkedSecondParent_BecomesPrimary()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        var secondParentId = Guid.CreateVersion7();
        student.AddParent(secondParentId, ParentRelationship.Mother);
        students.Existing.Add(student);
        var handler = new SetPrimaryParentHandler(students);

        var result = await handler.HandleAsync(new SetPrimaryParentCommand(student.Id, secondParentId, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(student.Parents.Single(link => link.ParentId == secondParentId).IsPrimary);
    }

    [Fact]
    public async Task SetPrimaryParent_NotLinked_ReturnsNotLinked()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new SetPrimaryParentHandler(students);

        var result = await handler.HandleAsync(new SetPrimaryParentCommand(student.Id, Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.parent_not_linked", result.Error!.Code);
    }
}
