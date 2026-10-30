using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Time.Testing;
using Npgsql;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Commands.RunAgent;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Commands.RecordInboundMessage;
using TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;
using TutoringCentre.Application.Conversations.Inbound.Commands.ProcessInboundMessage;
using TutoringCentre.Application.Conversations.Web.Commands.SendWebMessage;
using TutoringCentre.Domain.Assistant;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Messaging;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Conversations;
using TutoringCentre.Infrastructure.Tests.Fixtures;
namespace TutoringCentre.Infrastructure.Tests.Assistant;
public sealed class AgentRunTests(PostgresFixture fixture) : ConversationsTestBase(fixture)
{
    [Fact]
    public async Task SequentialAndParallelClaimsReturnExactlyOneRun()
    {
        var inbound = await SeedWebAsync();
        var conversation = await Fixture.ScalarAsync<Guid>("select conversation_id from conversations.inbound_messages");
        var results = await Task.WhenAll(Enumerable.Range(0,12).Select(_=>ClaimAsync(conversation,inbound)));
        Assert.Single(results,x=>x is not null);
        Assert.Null(await ClaimAsync(conversation,inbound));
        Assert.Equal(1L,await Fixture.ScalarAsync<long>("select count(*) from assistant.agent_runs"));
    }
    [Theory]
    [InlineData(false)] [InlineData(true)]
    public async Task RealRelayProducesOneReplyAndRedeliveryIsNoOp(bool fail)
    {
        var model = new TestModel(fail);
        var time = new FakeTimeProvider(Now);
        await using var services = Fixture.CreateServiceProvider(s=>
        {
            s.AddSingleton<TimeProvider>(time);
            s.AddSingleton<ILanguageModel>(model);
        });
        var inbound = await SeedWebAsync(services, fail ? "\u0645\u0631\u062d\u0628\u0627" : "hello");
        var relay = services.GetRequiredService<OutboxRelayService>();
        Assert.Equal(1,await relay.ProcessBatchAsync());
        Assert.Equal(1,await relay.ProcessBatchAsync());
        Assert.Equal(1,await relay.ProcessBatchAsync());
        Assert.Equal(0,await relay.ProcessBatchAsync());
        var duplicate = await services.SendAsAsync<RunAgentCommand,RunAgentResult>(new SystemActor(NileCentreId),new(inbound));
        Assert.True(duplicate.IsSuccess); Assert.True(duplicate.Value.AlreadyRan);
        // Simulate a committed run whose relay acknowledgement was lost.
        await ExecuteAsync("update jobs.outbox_messages set processed_at=null,outcome=null,available_at='2027-03-01T08:00:00Z' where type='assistant.run_agent.v1'");
        Assert.Equal(1,await relay.ProcessBatchAsync());
        Assert.Equal(0,await relay.ProcessBatchAsync());
        Assert.Equal(1,model.Calls);
        Assert.Equal(1L,await Fixture.ScalarAsync<long>("select count(*) from assistant.agent_runs"));
        Assert.Equal(1L,await Fixture.ScalarAsync<long>("select count(*) from conversations.outbound_messages where status='sent'"));
        Assert.Equal(1L,await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages where type='conversations.send_outbound.v1'"));
        Assert.Equal(fail?"failed":"completed",await Fixture.ScalarAsync<string>("select status from assistant.agent_runs"));
        if(fail)
        {
            Assert.Equal("assistant.model_error",await Fixture.ScalarAsync<string>("select failure_code from assistant.agent_runs"));
            var fallback = await Fixture.ScalarAsync<string>("select text from conversations.outbound_messages");
            Assert.Contains("\u0639\u0630\u0631",fallback,StringComparison.Ordinal);
            Assert.DoesNotContain("??",fallback,StringComparison.Ordinal);
        }
        else Assert.Equal("Hello. I will be able to help soon.",await Fixture.ScalarAsync<string>("select text from conversations.outbound_messages"));
        Assert.False(await Fixture.ScalarAsync<bool>("select exists(select 1 from jobs.outbox_messages where payload ? 'text' or payload ? 'reply' or payload ? 'prompt')"));
        Assert.Equal(0L,await Fixture.ScalarAsync<long>("select count(*) from audit.audit_entries where entity_type='agent_run'"));
        Assert.DoesNotContain("+201",model.Request!.System,StringComparison.Ordinal);
        Assert.DoesNotContain(inbound.ToString(),model.Request.System,StringComparison.Ordinal);
    }
    [Fact]
    public async Task WhatsappStillProcessesWithoutAgentRun()
    {
        var result = await SendAsync<RecordInboundMessageCommand,RecordInboundMessageResult>(NileCentreId,
            new(NileAccountId,"201000000091","wa-agent-test",InboundMessageType.Text,"hello",Now));
        Assert.True(result.IsSuccess);
        var id=await Fixture.ScalarAsync<Guid>("select id from conversations.inbound_messages");
        var processed=await SendAsync<ProcessInboundMessageCommand,Unit>(NileCentreId,new(id));
        Assert.True(processed.IsSuccess);
        Assert.Equal("processed",await Fixture.ScalarAsync<string>("select status from conversations.inbound_messages"));
        Assert.Equal(0L,await Fixture.ScalarAsync<long>("select count(*) from assistant.agent_runs"));
        Assert.Equal(0L,await Fixture.ScalarAsync<long>("select count(*) from jobs.outbox_messages where type='assistant.run_agent.v1'"));
    }
    [Fact]
    public async Task CrossTenantAndStaffCannotRunAndRlsHidesClaims()
    {
        var inbound=await SeedWebAsync();
        await using var services=Fixture.CreateServiceProvider(s=>s.AddSingleton<ILanguageModel>(new TestModel(false)));
        var cross=await services.SendAsAsync<RunAgentCommand,RunAgentResult>(new SystemActor(MaadiCentreId),new(inbound));
        Assert.True(cross.IsFailure); Assert.Equal("assistant.inbound_not_found",cross.Error!.Code);
        var staff=await services.SendAsAsync<RunAgentCommand,RunAgentResult>(new StaffActor(Guid.NewGuid(),NileCentreId,StaffRole.Owner),new(inbound));
        Assert.True(staff.IsFailure);
        var conversation=await Fixture.ScalarAsync<Guid>("select conversation_id from conversations.inbound_messages");
        Assert.NotNull(await ClaimAsync(conversation,inbound));
        await using var connection = new NpgsqlConnection(Fixture.AppConnectionString);
        await connection.OpenAsync();
        await using var transaction = await connection.BeginTransactionAsync();
        await using var setting = new NpgsqlCommand("select set_config('app.current_centre', @centre, true)",connection,transaction);
        setting.Parameters.AddWithValue("centre",MaadiCentreId.ToString());
        await setting.ExecuteNonQueryAsync();
        await using var count = new NpgsqlCommand("select count(*) from assistant.agent_runs",connection,transaction);
        Assert.Equal(0L,await count.ExecuteScalarAsync());
        Assert.False(await Fixture.ScalarAsync<bool>("select has_table_privilege('tutoring_app','assistant.agent_runs','DELETE')"));
        Assert.True(await Fixture.ScalarAsync<bool>("select relrowsecurity and relforcerowsecurity from pg_class where oid='assistant.agent_runs'::regclass"));
    }
    [Fact]
    public async Task DatabaseRejectsInvalidRunCountsAndFailureState()
    {
        var inbound=await SeedWebAsync();
        var conversation=await Fixture.ScalarAsync<Guid>("select conversation_id from conversations.inbound_messages");
        await ClaimAsync(conversation,inbound);
        var counts=await Assert.ThrowsAsync<PostgresException>(()=>ExecuteAsync("update assistant.agent_runs set steps=-1"));
        Assert.Equal("ck_agent_runs_counts",counts.ConstraintName);
        var failure=await Assert.ThrowsAsync<PostgresException>(()=>ExecuteAsync("update assistant.agent_runs set status='failed',finished_at=now()"));
        Assert.Equal("ck_agent_runs_failure",failure.ConstraintName);
        Assert.Equal("assistant.already_ran",ConstraintErrorCatalog.FindUnique("ux_agent_runs_inbound_message")?.Code);
    }
    private async Task<Guid> SeedWebAsync(IServiceProvider? services=null, string text="hello")
    {
        var parent=await Fixture.SeedParentAsync(NileCentreId,"Agent Parent","+201000000098");
        var channel=await SendAsync<RegisterChannelAccountCommand,Guid>(NileCentreId,new(ChannelProvider.Web,"web:nile-centre",null));
        Assert.True(channel.IsSuccess);
        var sent=await (services??Fixture.Services).SendAsAsync<SendWebMessageCommand,SendWebMessageResult>(
            new ParentActor(Guid.NewGuid(),NileCentreId,parent),new(Guid.NewGuid(),text));
        Assert.True(sent.IsSuccess); return sent.Value.MessageId;
    }
    private async Task<AgentRun?> ClaimAsync(Guid conversation,Guid inbound)
    {
        await using var scope=Fixture.Services.CreateAsyncScope();
        var services=scope.ServiceProvider;
        services.GetRequiredService<CurrentActorContext>().Set(new SystemActor(NileCentreId));
        var unit=services.GetRequiredService<IUnitOfWork>();
        await unit.BeginAsync(false,CancellationToken.None);
        var run=await services.GetRequiredService<IAgentRunRepository>().TryStartAsync(conversation,inbound,Now,CancellationToken.None);
        Assert.True((await unit.SaveChangesAsync(CancellationToken.None)).IsSuccess);
        await unit.CommitAsync(CancellationToken.None); return run;
    }
    private sealed class TestModel(bool fail) : ILanguageModel
    {
        public int Calls { get; private set; }
        public ModelRequest? Request { get; private set; }
        public Task<Result<ModelResponse>> GenerateAsync(ModelRequest request,CancellationToken cancellationToken)
        {
            Calls++; Request=request;
            return Task.FromResult(fail?Result<ModelResponse>.Failure(Error.Rule("assistant.model_error","Unavailable")):
                Result<ModelResponse>.Success(new("Hello. I will be able to help soon.",[],new(12,9),ModelStopReason.Completed)));
        }
    }
}
