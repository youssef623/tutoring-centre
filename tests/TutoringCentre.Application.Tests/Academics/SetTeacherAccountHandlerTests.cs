using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// The link is the security-relevant part of Task 43.8, so most cases here target its refusals rather than
/// the happy path alone: an unknown user, a user of another centre, a user with another role and an inactive
/// member must all answer teacher.user_not_eligible identically, and a user already linked elsewhere must
/// answer teacher.account_taken.
/// </summary>
public sealed class SetTeacherAccountHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownTeacherId_ReturnsNotFound()
    {
        var fixture = CreateSut();

        var result = await fixture.Handler.HandleAsync(
            new SetTeacherAccountCommand(Guid.CreateVersion7(), Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_NullUserId_Unlinks()
    {
        var fixture = CreateSut();
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var userId = Guid.CreateVersion7();
        teacher.LinkUser(userId);
        fixture.Repository.Existing.Add(teacher);

        var result = await fixture.Handler.HandleAsync(new SetTeacherAccountCommand(teacher.Id, null, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(teacher.UserId);
    }

    [Fact]
    public async Task HandleAsync_EligibleUser_Links()
    {
        var fixture = CreateSut();
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        fixture.Repository.Existing.Add(teacher);
        var userId = Guid.CreateVersion7();
        fixture.StaffDirectory.Members.Add((NileCentreId, userId, StaffRole.Teacher));

        var result = await fixture.Handler.HandleAsync(new SetTeacherAccountCommand(teacher.Id, userId, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, teacher.UserId);
    }

    [Fact]
    public async Task HandleAsync_UnknownUserId_ReturnsUserNotEligible()
    {
        var fixture = CreateSut();
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        fixture.Repository.Existing.Add(teacher);

        var result = await fixture.Handler.HandleAsync(
            new SetTeacherAccountCommand(teacher.Id, Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertUserNotEligible(result);
        Assert.Null(teacher.UserId);
    }

    [Fact]
    public async Task HandleAsync_UserOfAnotherCentre_ReturnsUserNotEligible()
    {
        var fixture = CreateSut();
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        fixture.Repository.Existing.Add(teacher);
        var otherCentreId = Guid.CreateVersion7();
        var userId = Guid.CreateVersion7();
        fixture.StaffDirectory.Members.Add((otherCentreId, userId, StaffRole.Teacher));

        var result = await fixture.Handler.HandleAsync(new SetTeacherAccountCommand(teacher.Id, userId, 0), CancellationToken.None);

        AssertUserNotEligible(result);
    }

    [Fact]
    public async Task HandleAsync_UserWithAnotherRole_ReturnsUserNotEligible()
    {
        var fixture = CreateSut();
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        fixture.Repository.Existing.Add(teacher);
        var userId = Guid.CreateVersion7();
        fixture.StaffDirectory.Members.Add((NileCentreId, userId, StaffRole.Secretary));

        var result = await fixture.Handler.HandleAsync(new SetTeacherAccountCommand(teacher.Id, userId, 0), CancellationToken.None);

        AssertUserNotEligible(result);
    }

    [Fact]
    public async Task HandleAsync_UserAlreadyLinkedToAnotherTeacher_ReturnsAccountTaken()
    {
        var fixture = CreateSut();
        var userId = Guid.CreateVersion7();
        var alreadyLinked = Teacher.Create(NileCentreId, "Mona Said", null).Value;
        alreadyLinked.LinkUser(userId);
        fixture.Repository.Existing.Add(alreadyLinked);

        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        fixture.Repository.Existing.Add(teacher);
        fixture.StaffDirectory.Members.Add((NileCentreId, userId, StaffRole.Teacher));

        var result = await fixture.Handler.HandleAsync(new SetTeacherAccountCommand(teacher.Id, userId, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.account_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Contains("userId", result.Error.Fields!.Keys);
        Assert.Null(teacher.UserId);
    }

    [Fact]
    public async Task HandleAsync_RelinkingTheSameTeacherToItsOwnUser_Succeeds()
    {
        var fixture = CreateSut();
        var userId = Guid.CreateVersion7();
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        teacher.LinkUser(userId);
        fixture.Repository.Existing.Add(teacher);
        fixture.StaffDirectory.Members.Add((NileCentreId, userId, StaffRole.Teacher));

        var result = await fixture.Handler.HandleAsync(new SetTeacherAccountCommand(teacher.Id, userId, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, teacher.UserId);
    }

    private static void AssertUserNotEligible<T>(Result<T> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("teacher.user_not_eligible", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Contains("userId", result.Error.Fields!.Keys);
    }

    private static Fixture CreateSut()
    {
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), NileCentreId, StaffRole.Owner));
        var repository = new FakeTeacherRepository();
        var staffDirectory = new FakeStaffDirectory();
        var handler = new SetTeacherAccountHandler(actorContext, repository, staffDirectory);
        return new Fixture(handler, repository, staffDirectory);
    }

    private sealed record Fixture(SetTeacherAccountHandler Handler, FakeTeacherRepository Repository, FakeStaffDirectory StaffDirectory);
}
