using TutoringCentre.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Billing;
namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

internal sealed class GroupMonthlyFeeConfiguration : IEntityTypeConfiguration<GroupMonthlyFee>
{
    public void Configure(EntityTypeBuilder<GroupMonthlyFee> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("group_monthly_fees", Schemas.Academics, table =>
        {
            table.HasCheckConstraint("ck_group_monthly_fees_nonnegative", "fee >= 0");
            table.HasCheckConstraint("ck_group_monthly_fees_month", "extract(day from effective_month)=1");
        });
        builder.HasKey(x => x.Id); builder.Property(x => x.Id).ValueGeneratedNever(); builder.ConfigureTenantOwned("group_monthly_fees");
        builder.Property(x => x.EffectiveMonth).HasConversion(x => x.FirstDay, x => BillingMonth.Of(x));
        builder.Property(x => x.Fee).HasConversion(x => x.Amount, x => Money.Create(x).Value).HasPrecision(10, 2);
        builder.HasIndex(x => new { x.CentreId, x.GroupId, x.EffectiveMonth }).IsUnique().HasDatabaseName("ux_group_monthly_fees_month");
        builder.HasOne<Group>().WithMany(x => x.MonthlyFees).HasForeignKey(x => new { x.CentreId, x.GroupId }).HasPrincipalKey(x => new { x.CentreId, x.Id }).OnDelete(DeleteBehavior.Restrict);
    }
}
