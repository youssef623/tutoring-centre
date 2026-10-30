using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;
namespace TutoringCentre.Infrastructure.Tests.Assistant;
[Collection(PostgresCollection.Name)]
public sealed class ConversationLockTests(PostgresFixture fixture)
{
    [Fact]
    public async Task BusyIsImmediateAndRollbackReleasesOnlyItsConversation()
    {
        await using var first = fixture.Services.CreateAsyncScope();
        await using var second = fixture.Services.CreateAsyncScope();
        var db1 = first.ServiceProvider.GetRequiredService<AppDbContext>();
        var db2 = second.ServiceProvider.GetRequiredService<AppDbContext>();
        await using var tx1 = await db1.Database.BeginTransactionAsync();
        await using var tx2 = await db2.Database.BeginTransactionAsync();
        var lock1 = first.ServiceProvider.GetRequiredService<IConversationLock>();
        var lock2 = second.ServiceProvider.GetRequiredService<IConversationLock>();
        var centre = Guid.NewGuid(); var conversation = Guid.NewGuid();
        Assert.True(await lock1.TryAcquireAsync(centre, conversation, default));
        Assert.False(await lock2.TryAcquireAsync(centre, conversation, default));
        Assert.True(await lock2.TryAcquireAsync(centre, Guid.NewGuid(), default));
        Assert.True(await lock2.TryAcquireAsync(Guid.NewGuid(), conversation, default));
        await tx1.RollbackAsync();
        Assert.True(await lock2.TryAcquireAsync(centre, conversation, default));
    }
}
