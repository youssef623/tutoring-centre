using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Common.Execution;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Contracts;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Conversations;
using TutoringCentre.Infrastructure.Tests.Fixtures;
namespace TutoringCentre.Infrastructure.Tests.Assistant;
public sealed class PortalActorFactoryTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    [Fact] public async Task StoredSubjectRequiresLiveMembershipAndLinkAndInnerContextIsSeparate()
    {
        var parent=await Fixture.SeedParentAsync(NileCentreId,"Parent","+201000000061");
        var created=await Fixture.SendAsAsync<CreatePortalAccountCommand,CreatePortalAccountResult>(new SystemActor(NileCentreId,await Fixture.SeedUserAsync()),new(PortalSubjectKind.Parent,parent,"actor@nile.test",SupportedLocale.En));
        Assert.True(created.IsSuccess,created.Error?.Message);
        var input=new AssistantConversationInput(Guid.NewGuid(),parent,null,true,"hello",created.Value.UserId);
        await using var outer=Fixture.Services.CreateAsyncScope(); var services=outer.ServiceProvider;
        services.GetRequiredService<CurrentActorContext>().Set(new SystemActor(NileCentreId));
        services.GetRequiredService<CorrelationContext>().Set("actor:integration");
        var unit=services.GetRequiredService<IUnitOfWork>();await unit.BeginAsync(false,CancellationToken.None);
        var factory=services.GetRequiredService<PortalActorFactory>();
        var actor=await factory.ForConversationAsync(input); Assert.True(actor.IsSuccess);
        var db=services.GetRequiredService<AppDbContext>();
        await services.GetRequiredService<IActorScopeRunner>().RunAsync(actor.Value,async inner=>{
            Assert.NotSame(db,inner.GetRequiredService<AppDbContext>());
            Assert.NotSame(db.Database.GetDbConnection(),inner.GetRequiredService<AppDbContext>().Database.GetDbConnection());
            Assert.IsType<ParentActor>(inner.GetRequiredService<ICurrentActor>().Actor);
            var innerUnit=inner.GetRequiredService<IUnitOfWork>();await innerUnit.BeginAsync(true,CancellationToken.None);
            var innerDb=inner.GetRequiredService<AppDbContext>();
            Assert.NotEqual(((Npgsql.NpgsqlConnection)db.Database.GetDbConnection()).ProcessID,((Npgsql.NpgsqlConnection)innerDb.Database.GetDbConnection()).ProcessID);
            await using var readOnly=innerDb.Database.GetDbConnection().CreateCommand();
            readOnly.Transaction=innerDb.Database.CurrentTransaction!.GetDbTransaction();
            readOnly.CommandText="show transaction_read_only";
            Assert.Equal("on",await readOnly.ExecuteScalarAsync());
            Assert.Equal(NileCentreId,inner.GetRequiredService<ICurrentActor>().CentreId);
            await innerUnit.CommitAsync(CancellationToken.None);return true;
        });
        Assert.IsType<SystemActor>(services.GetRequiredService<ICurrentActor>().Actor);
        await ExecuteAsync("update identity.memberships set status='inactive' where role='parent'");
        Assert.Equal("assistant.account_inactive",(await factory.ForConversationAsync(input)).Error!.Code);
        await ExecuteAsync("update identity.memberships set status='active' where role='parent'; delete from identity.portal_links");
        Assert.Equal("assistant.account_inactive",(await factory.ForConversationAsync(input)).Error!.Code);
        await unit.RollbackAsync(CancellationToken.None);
    }
}
