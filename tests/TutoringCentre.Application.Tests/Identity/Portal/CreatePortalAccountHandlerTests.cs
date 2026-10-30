using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Identity.Portal;

public sealed class CreatePortalAccountHandlerTests
{
    private readonly Guid _centre = Guid.NewGuid();
    private readonly Guid _subject = Guid.NewGuid();
    private readonly Guid _creator = Guid.NewGuid();
    private readonly FakePortalDirectory _directory = new();
    private readonly FakeStaffAccountService _accounts = new();
    private readonly FakeMembershipRepository _memberships = new();
    private readonly FakePortalLinkRepository _links = new();

    private CreatePortalAccountHandler Handler()
    {
        var actor = new CurrentActorContext();
        actor.Set(new StaffActor(_creator, _centre, StaffRole.Owner));
        return new(actor, _directory, _accounts, _memberships, _links);
    }

    private CreatePortalAccountCommand Command(PortalSubjectKind kind = PortalSubjectKind.Parent) =>
        new(kind, _subject, "portal@nile.test", SupportedLocale.En);

    [Theory]
    [InlineData(PortalSubjectKind.Parent)]
    [InlineData(PortalSubjectKind.Student)]
    public async Task SuccessCreatesActiveMembershipAndExactlyOneSubjectLink(PortalSubjectKind kind)
    {
        (kind == PortalSubjectKind.Parent ? _directory.Parents : _directory.Students)[_subject] = new(_subject, "Subject Name", true);
        var result = await Handler().HandleAsync(Command(kind), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value.TemporaryPassword);
        var membership = Assert.Single(_memberships.Added);
        var link = Assert.Single(_links.Links);
        Assert.Equal(result.Value.MembershipId, membership.Id);
        Assert.Equal(result.Value.LinkId, link.Id);
        Assert.Equal(membership.UserId, link.UserId);
        Assert.Equal(_centre, link.CentreId);
        Assert.Equal(_creator, link.CreatedBy);
        Assert.True(membership.IsPortal);
        Assert.Equal(MembershipStatus.Active, membership.Status);
        Assert.Equal(kind == PortalSubjectKind.Parent ? "parent" : "student", link.RoleKey);
    }

    [Theory]
    [InlineData(PortalSubjectKind.Parent)]
    [InlineData(PortalSubjectKind.Student)]
    public async Task UnknownOrForeignSubjectDoesNotCreateAnAccount(PortalSubjectKind kind)
    {
        var result = await Handler().HandleAsync(Command(kind), CancellationToken.None);
        Assert.Equal("portal.subject_not_found", result.Error!.Code);
        Assert.Empty(_accounts.CreatedUserIds);
        Assert.Empty(_memberships.Added);
        Assert.Empty(_links.Links);
    }

    [Fact]
    public async Task InactiveStudentDoesNotCreateAnAccount()
    {
        _directory.Students[_subject] = new(_subject, "Inactive Student", false);
        var result = await Handler().HandleAsync(Command(PortalSubjectKind.Student), CancellationToken.None);
        Assert.Equal("portal.subject_not_found", result.Error!.Code);
        Assert.Empty(_accounts.CreatedUserIds);
    }

    [Fact]
    public async Task LinkedSubjectIsCheckedBeforeEnsuringAccount()
    {
        _directory.Parents[_subject] = new(_subject, "Parent", true);
        _links.Links.Add(PortalLink.CreateForParent(Guid.NewGuid(), _centre, _subject, _creator).Value);
        var result = await Handler().HandleAsync(Command(), CancellationToken.None);
        Assert.Equal("portal.subject_already_linked", result.Error!.Code);
        Assert.Empty(_accounts.CreatedUserIds);
        Assert.Empty(_memberships.Added);
    }

    [Fact]
    public async Task ExistingEmailReturnsNoPassword()
    {
        var user = Guid.NewGuid();
        _directory.Parents[_subject] = new(_subject, "Parent", true);
        _accounts.ExistingAccounts["portal@nile.test"] = user;
        var result = await Handler().HandleAsync(Command(), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(user, result.Value.UserId);
        Assert.Null(result.Value.TemporaryPassword);
        Assert.Empty(_accounts.CreatedUserIds);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task AnyExistingMembershipInThisCentreRefusesLinking(bool inactive)
    {
        var user = Guid.NewGuid();
        _directory.Parents[_subject] = new(_subject, "Parent", true);
        _accounts.ExistingAccounts["portal@nile.test"] = user;
        var membership = Membership.Create(user, _centre, StaffRole.Teacher).Value;
        if (inactive) membership.Deactivate();
        _memberships.Memberships.Add(membership);
        var result = await Handler().HandleAsync(Command(), CancellationToken.None);
        Assert.Equal("portal.user_already_linked", result.Error!.Code);
        Assert.Empty(_memberships.Added);
        Assert.Empty(_links.Links);
    }

    [Fact]
    public void ValidatorRejectsInvalidShapesAndAcceptsBothKinds()
    {
        var validator = new CreatePortalAccountValidator();
        Assert.True(validator.Validate(Command()).IsValid);
        Assert.True(validator.Validate(Command(PortalSubjectKind.Student)).IsValid);
        Assert.False(validator.Validate(Command() with { SubjectId = Guid.Empty }).IsValid);
        Assert.False(validator.Validate(Command() with { Kind = (PortalSubjectKind)99 }).IsValid);
        Assert.False(validator.Validate(Command() with { PreferredLocale = (SupportedLocale)99 }).IsValid);
        Assert.False(validator.Validate(Command() with { Email = "invalid" }).IsValid);
        Assert.False(validator.Validate(Command() with { Email = new string('a', 250) + "@nile.test" }).IsValid);
    }
}
