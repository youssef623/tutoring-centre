using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Messaging;

namespace TutoringCentre.Infrastructure.Persistence.Conventions;

/// <summary>Attendance and the outbox define exact index contracts; keep default FK indexes elsewhere.</summary>
internal sealed class AttendanceIndexConvention(ProviderConventionSetBuilderDependencies dependencies)
    : ForeignKeyIndexConvention(dependencies)
{
    protected override IConventionIndex? CreateIndex(IReadOnlyList<IConventionProperty> properties, bool unique,
        IConventionEntityTypeBuilder entityTypeBuilder) =>
        entityTypeBuilder.Metadata.ClrType == typeof(AttendanceRecord) || entityTypeBuilder.Metadata.ClrType == typeof(StudentCard)
            || entityTypeBuilder.Metadata.ClrType == typeof(ChannelAccount)
            || entityTypeBuilder.Metadata.ClrType == typeof(Conversation)
            || entityTypeBuilder.Metadata.ClrType == typeof(InboundMessage)
            || entityTypeBuilder.Metadata.ClrType == typeof(OutboxMessage)
            ? null
            : base.CreateIndex(properties, unique, entityTypeBuilder);
}
