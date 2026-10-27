using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Infrastructure.Persistence.Conventions;

/// <summary>Attendance defines its indexes explicitly; keep default FK indexes for every other entity.</summary>
internal sealed class AttendanceIndexConvention(ProviderConventionSetBuilderDependencies dependencies)
    : ForeignKeyIndexConvention(dependencies)
{
    protected override IConventionIndex? CreateIndex(IReadOnlyList<IConventionProperty> properties, bool unique,
        IConventionEntityTypeBuilder entityTypeBuilder) =>
        entityTypeBuilder.Metadata.ClrType == typeof(AttendanceRecord)
            ? null
            : base.CreateIndex(properties, unique, entityTypeBuilder);
}
