using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Assistant;
using TutoringCentre.Infrastructure.Persistence;
namespace TutoringCentre.Infrastructure.Assistant;
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class AgentRunRepository(AppDbContext db, ICurrentActor actor) : IAgentRunRepository
{
    public async Task<AgentRun?> TryStartAsync(Guid conversationId, Guid inboundMessageId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        var centre = actor.Actor.CentreId ?? throw new InvalidOperationException("A run requires a centre.");
        var transaction = db.Database.CurrentTransaction ?? throw new InvalidOperationException("A run requires the dispatcher transaction.");
        var run = AgentRun.Start(centre, conversationId, inboundMessageId, now);
        await using var command = new NpgsqlCommand("""
            INSERT INTO assistant.agent_runs
              (centre_id,id,conversation_id,inbound_message_id,status,started_at,steps,input_tokens,output_tokens)
            VALUES (@centre,@id,@conversation,@inbound,'running',@now,0,0,0)
            ON CONFLICT (centre_id,inbound_message_id) DO NOTHING RETURNING id;
            """, (NpgsqlConnection)db.Database.GetDbConnection(), (NpgsqlTransaction)transaction.GetDbTransaction());
        command.Parameters.AddWithValue("centre", centre);
        command.Parameters.AddWithValue("id", run.Id);
        command.Parameters.AddWithValue("conversation", conversationId);
        command.Parameters.AddWithValue("inbound", inboundMessageId);
        command.Parameters.AddWithValue("now", now);
        if (await command.ExecuteScalarAsync(cancellationToken) is not Guid) return null;
        db.Attach(run);
        return run;
    }
}
