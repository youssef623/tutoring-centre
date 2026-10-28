using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Commands.IssueStudentCard;
using TutoringCentre.Application.Attendance.Commands.IssueGroupCards;
using TutoringCentre.Application.Attendance.Commands.RevokeStudentCard;
using TutoringCentre.Application.Attendance.Queries.GetStudentCard;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Attendance;

public sealed class StudentCardHandlerTests
{
    private readonly FakeAttendanceRosterSource roster = new();
    private readonly Cards repository = new();
    private readonly CurrentActorContext actor = new();
    private readonly FakeClock clock = new();
    private readonly StaffActor staff = new(Guid.NewGuid(), Guid.NewGuid(), StaffRole.Owner);
    private readonly Guid studentId = Guid.NewGuid();
    private readonly Guid groupId = Guid.NewGuid();

    public StudentCardHandlerTests()
    {
        actor.Set(staff);
        roster.CardStudents.Add(studentId, new(studentId, "Student", false));
        roster.CardGroups.Add(groupId, new(groupId, "Group", [new(studentId, "Student")]));
    }

    private Task<TutoringCentre.Domain.Common.Result<IssuedCard>> Issue() =>
        new IssueStudentCardHandler(roster, repository, actor, clock).HandleAsync(new(studentId), CancellationToken.None);

    [Fact]
    public async Task Issue_ReturnsValidToken_AndReissueRevokesPrevious()
    {
        var first = await Issue();
        Assert.True(CardToken.Parse(first.Value.Token).IsSuccess);
        Assert.DoesNotContain(first.Value.Token, first.Value.ToString());
        Assert.Equal(staff.UserId, repository.Items[0].IssuedByUserId);
        Assert.Equal(clock.UtcNow, first.Value.IssuedAt);
        var second = await Issue();
        Assert.True(second.IsSuccess);
        Assert.Equal(CardRevocationReason.Reissued, repository.Items[0].RevocationReason);
        Assert.Equal(StudentCardStatus.Revoked, repository.Items[0].Status);
        Assert.Equal(StudentCardStatus.Active, repository.Items[1].Status);
    }

    [Theory]
    [InlineData(false, "student.not_found")]
    [InlineData(true, "card.student_archived")]
    public async Task InvalidStudent_IssuesNothing(bool archived, string code)
    {
        if (archived) roster.CardStudents[studentId] = new(studentId, "Student", true);
        else roster.CardStudents.Clear();
        Assert.Equal(code, (await Issue()).Error!.Code);
        Assert.Empty(repository.Items);
    }

    [Fact]
    public async Task BulkOutsider_NamesEntryAndChangesNothing()
    {
        await Issue();
        var result = await new IssueGroupCardsHandler(roster, repository, actor, clock)
            .HandleAsync(new(groupId, [studentId, Guid.NewGuid()]), CancellationToken.None);
        Assert.Equal("card.student_not_in_group", result.Error!.Code);
        Assert.Contains("studentIds[1]", result.Error.Fields!.Keys);
        Assert.Equal(StudentCardStatus.Active, Assert.Single(repository.Items).Status);
    }

    [Fact]
    public async Task BulkUnknownGroup_IssuesNothing()
    {
        var result = await new IssueGroupCardsHandler(roster, repository, actor, clock)
            .HandleAsync(new(Guid.NewGuid(), [studentId]), CancellationToken.None);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Empty(repository.Items);
    }

    [Fact]
    public async Task BulkRotatesSelectedCardsTogether()
    {
        await Issue();
        var other = Guid.NewGuid();
        roster.CardGroups[groupId] = new(groupId, "Group", [new(studentId, "Student"), new(other, "Other")]);
        var result = await new IssueGroupCardsHandler(roster, repository, actor, clock)
            .HandleAsync(new(groupId, [studentId, other]), CancellationToken.None);
        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value.Cards.Count);
        Assert.All(result.Value.Cards, card => Assert.True(CardToken.Parse(card.Token).IsSuccess));
        Assert.Equal(CardRevocationReason.Reissued, repository.Items[0].RevocationReason);
    }

    [Fact]
    public async Task RevokeTwice_SecondFailsWithoutChangingReason()
    {
        var issued = (await Issue()).Value;
        var handler = new RevokeStudentCardHandler(repository, actor, clock);
        Assert.True((await handler.HandleAsync(new(issued.CardId, CardRevocationReason.Lost, 42), CancellationToken.None)).IsSuccess);
        Assert.Equal(42u, repository.ExpectedVersion);
        Assert.Equal("card.already_revoked", (await handler.HandleAsync(new(issued.CardId, CardRevocationReason.Damaged, 42), CancellationToken.None)).Error!.Code);
        Assert.Equal(CardRevocationReason.Lost, Assert.Single(repository.Items).RevocationReason);
        Assert.Equal("card.not_found", (await handler.HandleAsync(new(Guid.NewGuid(), CardRevocationReason.Lost, 1), CancellationToken.None)).Error!.Code);
    }

    [Theory]
    [InlineData(CardRevocationReason.Reissued)]
    [InlineData((CardRevocationReason)99)]
    public void RevokeValidator_RejectsClientReason(CardRevocationReason reason)
    {
        var result = new RevokeStudentCardValidator().Validate(new RevokeStudentCardCommand(Guid.NewGuid(), reason, 1));
        Assert.Contains(result.Errors, error => error.PropertyName == "Reason");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(61)]
    [InlineData(2)]
    public void BulkValidator_RejectsEmptyTooManyOrDuplicate(int count)
    {
        var ids = count == 2 ? new[] { studentId, studentId } : Enumerable.Range(0, count).Select(_ => Guid.NewGuid()).ToArray();
        Assert.False(new IssueGroupCardsValidator().Validate(new IssueGroupCardsCommand(groupId, ids)).IsValid);
    }

    [Fact]
    public async Task SystemActor_CannotManageCardsIncludingReads()
    {
        var actor = new CurrentActorContext();
        actor.Set(new SystemActor(staff.CentreId));
        Assert.Equal("auth.permission_denied", (await new IssueStudentCardHandler(roster, repository, actor, clock).HandleAsync(new(studentId), CancellationToken.None)).Error!.Code);
        Assert.Equal("auth.permission_denied", (await new IssueGroupCardsHandler(roster, repository, actor, clock).HandleAsync(new(groupId, [studentId]), CancellationToken.None)).Error!.Code);
        Assert.Equal("auth.permission_denied", (await new RevokeStudentCardHandler(repository, actor, clock).HandleAsync(new(Guid.NewGuid(), CardRevocationReason.Lost, 1), CancellationToken.None)).Error!.Code);
        Assert.Equal("auth.permission_denied", (await new GetStudentCardHandler(new ReadCards(), actor).HandleAsync(new(studentId), CancellationToken.None)).Error!.Code);
    }

    [Fact]
    public void AllRequests_RequireCardPermissionAndTenantScope()
    {
        object[] requests = [new IssueStudentCardCommand(studentId), new IssueGroupCardsCommand(groupId, [studentId]),
            new RevokeStudentCardCommand(Guid.NewGuid(), CardRevocationReason.Lost, 1), new GetStudentCardQuery(studentId)];
        Assert.All(requests, request =>
        {
            Assert.IsAssignableFrom<ITenantScoped>(request);
            Assert.Equal("attendance.cards.manage", Assert.IsAssignableFrom<IRequirePermission>(request).RequiredPermission);
        });
    }

    private sealed class ReadCards : IStudentCardReadService
    {
        public Task<StudentCardDto?> GetMostRecentAsync(Guid studentId, CancellationToken ct) => Task.FromResult<StudentCardDto?>(null);
    }

    private sealed class Cards : IStudentCardRepository
    {
        public List<StudentCard> Items { get; } = [];
        public uint ExpectedVersion { get; private set; }
        public Task<StudentCard?> GetActiveAsync(Guid studentId, CancellationToken ct) => Task.FromResult(Items.SingleOrDefault(card => card.StudentId == studentId && card.Status == StudentCardStatus.Active));
        public Task<IReadOnlyList<StudentCard>> GetActiveAsync(IReadOnlyCollection<Guid> studentIds, CancellationToken ct) => Task.FromResult<IReadOnlyList<StudentCard>>(Items.Where(card => studentIds.Contains(card.StudentId) && card.Status == StudentCardStatus.Active).ToArray());
        public Task<StudentCard?> GetByIdAsync(Guid cardId, uint expectedVersion, CancellationToken ct)
        {
            ExpectedVersion = expectedVersion;
            return Task.FromResult(Items.SingleOrDefault(card => card.Id == cardId));
        }
        public Task<StudentCard?> FindByTokenHashAsync(byte[] tokenHash, CancellationToken ct) => Task.FromResult(Items.SingleOrDefault(card => card.Matches(tokenHash)));
        public void Add(StudentCard card) => Items.Add(card);
    }
}
