using TutoringCentre.Application.Academics.Groups.Commands.ArchiveGroup;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveGroupHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownId_ReturnsNotFound()
    {
        var handler = NewHandler(new FakeGroupRepository());

        var result = await handler.HandleAsync(new ArchiveGroupCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.not_found", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ActiveGroup_Archives()
    {
        var group = NewGroup();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = NewHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(GroupStatus.Archived, group.Status);
    }

    [Fact]
    public async Task HandleAsync_AlreadyArchived_ReturnsAlreadyArchived()
    {
        var group = NewGroup();
        group.Archive();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var handler = NewHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.already_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_GroupWithActiveEnrollments_ReturnsHasActiveEnrollmentsAndStaysActive()
    {
        var group = NewGroup();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        repository.ActiveEnrollmentCounts[group.Id] = 1;
        var handler = NewHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.has_active_enrollments", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(GroupStatus.Active, group.Status);
    }

    [Fact]
    public async Task HandleAsync_StaleVersion_ReturnsConcurrencyStaleAndStaysActive()
    {
        var group = NewGroup();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        repository.Versions[group.Id] = 3;
        var handler = NewHandler(repository);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 2), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("concurrency.stale", result.Error!.Code);
        Assert.Equal(GroupStatus.Active, group.Status);
    }

    [Fact]
    public async Task HandleAsync_GroupWithARuleStillInEffect_ReturnsHasActiveScheduleAndStaysActive()
    {
        var group = NewGroup();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var rules = new FakeScheduleRuleRepository();
        rules.Existing.Add(ScheduleTestData.SaturdayRule(group.Id));
        var handler = NewHandler(repository, rules);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("group.has_active_schedule", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(GroupStatus.Active, group.Status);
    }

    [Fact]
    public async Task HandleAsync_GroupWhoseOnlyRuleEndedBeforeToday_Archives()
    {
        var group = NewGroup();
        var repository = new FakeGroupRepository();
        repository.Existing.Add(group);
        var rule = ScheduleTestData.SaturdayRule(group.Id, ScheduleTestData.Today.AddDays(-30));
        rule.End(ScheduleTestData.Today.AddDays(-1));
        var rules = new FakeScheduleRuleRepository();
        rules.Existing.Add(rule);
        var handler = NewHandler(repository, rules);

        var result = await handler.HandleAsync(new ArchiveGroupCommand(group.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess, result.Error?.Message);
        Assert.Equal(GroupStatus.Archived, group.Status);
    }

    // Day 52: the handler also checks schedule rules; no rule exists in these Day 44 cases.
    private static ArchiveGroupHandler NewHandler(FakeGroupRepository groups) =>
        new(new FakeClock(), new FakeCentreTimeZoneProvider(), groups, new FakeScheduleRuleRepository());

    private static ArchiveGroupHandler NewHandler(FakeGroupRepository groups, FakeScheduleRuleRepository rules) =>
        new(ScheduleTestData.Clock(), new FakeCentreTimeZoneProvider(), groups, rules);

    private static Group NewGroup() =>
        Group.Create(NileCentreId, "Grade 10 A", Guid.CreateVersion7(), Guid.CreateVersion7(), 20).Value;
}
