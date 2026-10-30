using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Assistant;
using TutoringCentre.Domain.Conversations;
namespace TutoringCentre.Infrastructure.Persistence.Configurations.Assistant;
internal sealed class AgentRunConfiguration : IEntityTypeConfiguration<AgentRun>
{
    public void Configure(EntityTypeBuilder<AgentRun> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("agent_runs", Schemas.Assistant, table =>
        {
            table.HasCheckConstraint("ck_agent_runs_status", "status IN ('running','completed','failed')");
            table.HasCheckConstraint("ck_agent_runs_finished", "(status = 'running') = (finished_at IS NULL)");
            table.HasCheckConstraint("ck_agent_runs_failure", "(status = 'failed') = (failure_code IS NOT NULL)");
            table.HasCheckConstraint("ck_agent_runs_counts", "steps >= 0 AND input_tokens >= 0 AND output_tokens >= 0");
        });
        builder.HasKey(x => new { x.CentreId, x.Id });
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.ConfigureTenantOwned("agent_runs");
        builder.Property(x => x.Status).HasConversion(x => x.ToString().ToLowerInvariant(), x => Enum.Parse<AgentRunStatus>(x, true)).HasMaxLength(12);
        builder.Property(x => x.FailureCode).HasMaxLength(60);
        builder.Property(x => x.GuardReason).HasMaxLength(60);
        builder.Property(x => x.PromptVersion).HasMaxLength(30);
        builder.HasIndex(x => new { x.CentreId, x.InboundMessageId }).IsUnique().HasDatabaseName("ux_agent_runs_inbound_message");
        builder.HasOne<Conversation>().WithMany().HasForeignKey(x => new { x.CentreId, x.ConversationId }).HasPrincipalKey(x => new { x.CentreId, x.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_agent_runs_conversations");
        builder.HasOne<InboundMessage>().WithMany().HasForeignKey(x => new { x.CentreId, x.InboundMessageId }).HasPrincipalKey(x => new { x.CentreId, x.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_agent_runs_inbound_messages");
    }
}
