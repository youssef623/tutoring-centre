using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Conversations;

public sealed class InboundChannelResolverTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    [Fact]
    public async Task Active_ResolvesIdsWithoutActor()
    {
        await using var scope = Fixture.Services.CreateAsyncScope();
        Assert.Equal(new ResolvedChannel(NileCentreId, NileAccountId), await scope.ServiceProvider.GetRequiredService<IInboundChannelResolver>()
            .ResolveAsync(ChannelProvider.Whatsapp, "100000000000001"));
    }

    [Fact]
    public async Task Disabled_ReturnsNull()
    {
        await ExecuteAsync($"update conversations.channel_accounts set status = 'disabled' where id = '{NileAccountId}'");
        await using var scope = Fixture.Services.CreateAsyncScope();
        Assert.Null(await scope.ServiceProvider.GetRequiredService<IInboundChannelResolver>().ResolveAsync(ChannelProvider.Whatsapp, "100000000000001"));
    }

    [Fact]
    public async Task UnknownAndDisplayPhone_ReturnNull()
    {
        await using var scope = Fixture.Services.CreateAsyncScope();
        var resolver = scope.ServiceProvider.GetRequiredService<IInboundChannelResolver>();
        Assert.Null(await resolver.ResolveAsync(ChannelProvider.Whatsapp, "999"));
        Assert.Null(await resolver.ResolveAsync(ChannelProvider.Whatsapp, "+201000000091"));
    }

    [Fact]
    public async Task OtherCentreActor_DoesNotAffectResolution()
    {
        await using var scope = Fixture.Services.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new SystemActor(MaadiCentreId));
        Assert.Equal(new ResolvedChannel(NileCentreId, NileAccountId), await scope.ServiceProvider.GetRequiredService<IInboundChannelResolver>()
            .ResolveAsync(ChannelProvider.Whatsapp, "100000000000001"));
    }

    [Fact]
    public async Task Register_GlobalDuplicate_ReturnsConflict_AndMasksAudit()
    {
        var result = await RegisterAsync(MaadiCentreId, "100000000000001");
        Assert.Equal("conversations.channel_taken", result.Error!.Code);
        Assert.Equal(TutoringCentre.Domain.Common.ErrorKind.Conflict, result.Error.Kind);
        var audit = await Fixture.ScalarAsync<string>($"select changes::text from audit.audit_entries where entity_id = '{NileAccountId}'");
        Assert.DoesNotContain("+201000000091", audit, StringComparison.Ordinal);
        Assert.Contains("0091", audit, StringComparison.Ordinal);
    }
}
