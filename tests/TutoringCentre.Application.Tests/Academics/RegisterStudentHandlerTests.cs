using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RegisterStudentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_ExistingParentForm_AddsStudentLinkedToThatParent()
    {
        var (handler, students, parents, _) = CreateSut();
        var existingParent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        parents.Existing.Add(existingParent);

        var result = await handler.HandleAsync(
            new RegisterStudentCommand("Youssef Omar", null, null, new RegisterStudentParent(existingParent.Id, null, null), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(students.Added);
        Assert.Empty(parents.Added);
        Assert.Equal(existingParent.Id, result.Value.ParentId);
        Assert.Equal(students.Added[0].Id, result.Value.StudentId);
    }

    [Fact]
    public async Task HandleAsync_NewParentForm_AddsBothParentAndStudent()
    {
        var (handler, students, parents, _) = CreateSut();

        var result = await handler.HandleAsync(
            new RegisterStudentCommand("Youssef Omar", null, "Grade 10", new RegisterStudentParent(null, "Omar Hassan", "01012345678"), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(parents.Added);
        Assert.Single(students.Added);
        Assert.Equal(NileCentreId, parents.Added[0].CentreId);
        Assert.Equal("+201012345678", parents.Added[0].Phone.Value);
        Assert.Equal(result.Value.ParentId, parents.Added[0].Id);
    }

    [Fact]
    public async Task HandleAsync_UnknownExistingParentId_ReturnsParentNotFoundAndAddsNothing()
    {
        // FindAsync itself is tenant-scoped (IParentRepository's doc comment) — real cross-tenant isolation is
        // proven at the Infrastructure level; here, "not in Existing" stands in for either case, since both
        // reach the handler the same way: a null from FindAsync.
        var (handler, students, parents, _) = CreateSut();

        var result = await handler.HandleAsync(
            new RegisterStudentCommand("Smuggled Student", null, null, new RegisterStudentParent(Guid.CreateVersion7(), null, null), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.not_found", result.Error!.Code);
        Assert.Empty(students.Added);
        Assert.Empty(parents.Added);
    }

    [Fact]
    public async Task HandleAsync_NewParentPhoneAlreadyTaken_ReturnsParentPhoneTakenWithNestedFieldAndAddsNothing()
    {
        var (handler, students, parents, _) = CreateSut();
        parents.Existing.Add(Parent.Create(NileCentreId, "Existing Parent", PhoneNumber.Create("01012345678").Value).Value);

        var result = await handler.HandleAsync(
            new RegisterStudentCommand("Youssef Omar", null, null, new RegisterStudentParent(null, "Omar Hassan", "0101 234 5678"), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.phone_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Contains("parent.phone", result.Error.Fields!.Keys);
        Assert.Empty(parents.Added);
        Assert.Empty(students.Added);
    }

    [Fact]
    public async Task HandleAsync_NewParentInvalidPhone_ReturnsNestedParentPhoneFieldAndAddsNothing()
    {
        var (handler, students, parents, _) = CreateSut();

        var result = await handler.HandleAsync(
            new RegisterStudentCommand("Youssef Omar", null, null, new RegisterStudentParent(null, "Omar Hassan", "12345"), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(ErrorKind.Validation, result.Error!.Kind);
        Assert.Contains("parent.phone", result.Error.Fields!.Keys);
        Assert.Empty(parents.Added);
        Assert.Empty(students.Added);
    }

    [Fact]
    public async Task HandleAsync_StudentNameInvalid_AfterNewParentWasStaged_StudentIsNeverAdded()
    {
        // The new parent is staged into the repository's tracked Added list before Student.Register runs — that
        // is expected (nothing commits until the dispatcher saves once); what must never happen is the student
        // itself being added when its own registration fails.
        var (handler, students, parents, _) = CreateSut();

        var result = await handler.HandleAsync(
            new RegisterStudentCommand("Y", null, null, new RegisterStudentParent(null, "Omar Hassan", "01012345678"), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.name_invalid", result.Error!.Code);
        Assert.Single(parents.Added);
        Assert.Empty(students.Added);
    }

    [Fact]
    public async Task HandleAsync_StudentOwnPhoneInvalid_ReturnsTopLevelPhoneFieldAndAddsNothing()
    {
        var (handler, students, parents, _) = CreateSut();
        var existingParent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        parents.Existing.Add(existingParent);

        var result = await handler.HandleAsync(
            new RegisterStudentCommand("Youssef Omar", "12345", null, new RegisterStudentParent(existingParent.Id, null, null), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Contains("phone", result.Error!.Fields!.Keys);
        Assert.Empty(students.Added);
        Assert.Empty(parents.Added);
    }

    private static (RegisterStudentHandler Handler, FakeStudentRepository Students, FakeParentRepository Parents, CurrentActorContext ActorContext) CreateSut()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var students = new FakeStudentRepository();
        var parents = new FakeParentRepository();
        var handler = new RegisterStudentHandler(actorContext, students, parents);
        return (handler, students, parents, actorContext);
    }
}
