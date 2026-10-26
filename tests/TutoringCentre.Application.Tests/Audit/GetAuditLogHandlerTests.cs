using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Audit.Queries.GetAuditLog;
using TutoringCentre.Application.Tests.Fakes;

namespace TutoringCentre.Application.Tests.Audit;

public sealed class GetAuditLogHandlerTests
{
    [Fact]
    public async Task HandleAsync_PassesEveryFilterThroughUnchanged()
    {
        var readService = new FakeAuditReadService();
        var handler = new GetAuditLogHandler(readService);
        var entityId = Guid.CreateVersion7();
        var actorUserId = Guid.CreateVersion7();
        var from = DateTimeOffset.UtcNow.AddDays(-7);
        var to = DateTimeOffset.UtcNow;
        var query = new GetAuditLogQuery("membership", entityId, actorUserId, from, to, "some-cursor", 25);

        var result = await handler.HandleAsync(query, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("membership", readService.LastEntityType);
        Assert.Equal(entityId, readService.LastEntityId);
        Assert.Equal(actorUserId, readService.LastActorUserId);
        Assert.Equal(from, readService.LastOccurredFrom);
        Assert.Equal(to, readService.LastOccurredTo);
        Assert.Equal("some-cursor", readService.LastCursor);
        Assert.Equal(25, readService.LastPageSize);
    }

    [Fact]
    public async Task HandleAsync_ReturnsThePageFromTheReadService()
    {
        var readService = new FakeAuditReadService
        {
            PageToReturn = new AuditPageDto(
                [new AuditEntryDto(Guid.CreateVersion7(), DateTimeOffset.UtcNow, "staff", Guid.CreateVersion7(), "Nile Owner", "updated", "membership", Guid.CreateVersion7(), [], "corr-1")],
                "next-cursor"),
        };
        var handler = new GetAuditLogHandler(readService);

        var result = await handler.HandleAsync(new GetAuditLogQuery(null, null, null, null, null, null, 50), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(readService.PageToReturn, result.Value);
    }
}
