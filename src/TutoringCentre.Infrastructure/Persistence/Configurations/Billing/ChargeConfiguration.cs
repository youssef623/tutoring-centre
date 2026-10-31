using TutoringCentre.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Billing;
namespace TutoringCentre.Infrastructure.Persistence.Configurations.Billing;

internal sealed class ChargeConfiguration : IEntityTypeConfiguration<Charge>
{
    public void Configure(EntityTypeBuilder<Charge> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.ToTable("charges", Schemas.Billing, table =>
        {
            table.HasCheckConstraint("ck_charges_kind_amount", "(kind='monthly' AND amount>0) OR (kind='adjustment' AND amount<>0 AND reason IS NOT NULL)");
            table.HasCheckConstraint("ck_charges_month", "extract(day from billing_month)=1");
            table.HasCheckConstraint("ck_charges_due", "due_date >= billing_month");
        });
        builder.HasKey(x => new { x.CentreId, x.Id }); builder.ConfigureTenantOwned("charges");
        builder.Property(x => x.Id).ValueGeneratedNever();
        builder.Property(x => x.Kind).HasConversion(x => x.ToString().ToLowerInvariant(), x => Enum.Parse<ChargeKind>(x, true)).HasMaxLength(10);
        builder.Property(x => x.Amount).HasConversion(x => x.Amount, x => Money.Create(x).Value).HasPrecision(10, 2);
        builder.Property(x => x.BillingMonth).HasConversion(x => x.FirstDay, x => BillingMonth.Of(x));
        builder.Property(x => x.Reason).HasMaxLength(200);
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.HasOne<Enrollment>().WithMany().HasForeignKey(x => new { x.CentreId, x.EnrollmentId, x.StudentId }).HasPrincipalKey(x => new { x.CentreId, x.Id, x.StudentId }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_charges_enrollments");
        builder.HasOne<Group>().WithMany().HasForeignKey(x => new { x.CentreId, x.GroupId }).HasPrincipalKey(x => new { x.CentreId, x.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_charges_groups");
        builder.HasOne<Subject>().WithMany().HasForeignKey(x => new { x.CentreId, x.SubjectId }).HasPrincipalKey(x => new { x.CentreId, x.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_charges_subjects");
        builder.HasOne<Teacher>().WithMany().HasForeignKey(x => new { x.CentreId, x.TeacherId }).HasPrincipalKey(x => new { x.CentreId, x.Id }).OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_charges_teachers");
        builder.HasIndex(x => new { x.CentreId, x.StudentId, x.SubjectId, x.BillingMonth }).IsUnique().HasFilter("kind='monthly'").HasDatabaseName("ux_charges_monthly");
        builder.HasIndex(x => new { x.CentreId, x.StudentId, x.DueDate }).HasDatabaseName("ix_charges_student_due");
    }
}
