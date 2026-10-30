using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Assistant;
namespace TutoringCentre.Infrastructure.Persistence.Configurations.Assistant;
public sealed class ToolCallConfiguration : IEntityTypeConfiguration<ToolCall>
{
    public void Configure(EntityTypeBuilder<ToolCall> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("tool_calls", Schemas.Assistant, table => {
            table.HasCheckConstraint("ck_tool_calls_decision", "decision IN ('allowed','denied','invalid','failed')");
            table.HasCheckConstraint("ck_tool_calls_reason", "decision = 'allowed' OR reason_code IS NOT NULL");
            table.HasCheckConstraint("ck_tool_calls_duration", "duration_ms >= 0");
        });
        builder.HasKey(x => new { x.CentreId, x.Id });
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.ConfigureTenantOwned("tool_calls");
        builder.Property(x => x.ToolName).HasMaxLength(60);
        builder.Property(x => x.Decision).HasConversion(x => x.ToString().ToLowerInvariant(), x => Enum.Parse<ToolDecision>(x, true)).HasMaxLength(12);
        builder.Property(x => x.ReasonCode).HasMaxLength(60);
        builder.Property(x => x.ArgumentSummary).HasMaxLength(200);
        builder.Property(x => x.ResultSummary).HasMaxLength(200);
        builder.HasIndex(x => new { x.CentreId, x.AgentRunId, x.Step, x.Ordinal }).IsUnique();
        builder.HasIndex(x => new { x.CentreId, x.AgentRunId });
        builder.HasOne<AgentRun>().WithMany().HasForeignKey(x => new { x.CentreId, x.AgentRunId }).OnDelete(DeleteBehavior.Restrict);
    }
}
