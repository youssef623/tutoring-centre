using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Attendance;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Attendance;

internal sealed class StudentCardConfiguration : IEntityTypeConfiguration<StudentCard>
{
    public void Configure(EntityTypeBuilder<StudentCard> builder)
    {
        builder.ToTable("student_cards", Schemas.Attendance, table =>
        {
            table.HasCheckConstraint("ck_student_cards_hash_length", "octet_length(token_hash) = 32");
            table.HasCheckConstraint("ck_student_cards_status", "status IN ('active','revoked')");
            table.HasCheckConstraint("ck_student_cards_reason", "revocation_reason IN ('reissued','lost','damaged','withdrawn')");
            table.HasCheckConstraint("ck_student_cards_revocation", "(status = 'revoked') = (revoked_at IS NOT NULL AND revocation_reason IS NOT NULL)");
        });
        builder.HasKey(card => card.Id);
        builder.Property(card => card.Id).ValueGeneratedNever();
        builder.ConfigureTenantOwned("student_cards");
        builder.Property(card => card.StudentId).IsRequired();
        builder.Property(card => card.TokenHash).HasField("tokenHash").UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnType("bytea").IsRequired();
        builder.Property(card => card.Status).HasConversion(value => value.ToString().ToLowerInvariant(),
            value => Enum.Parse<StudentCardStatus>(value, true)).HasMaxLength(10).IsRequired();
        builder.Property(card => card.RevocationReason).HasConversion(value => value!.Value.ToString().ToLowerInvariant(),
            value => Enum.Parse<CardRevocationReason>(value, true)).HasMaxLength(12);
        builder.Property(card => card.IssuedAt).IsRequired();
        builder.Property(card => card.IssuedByUserId).IsRequired();
        builder.Property(card => card.RevokedAt);
        builder.Property(card => card.RevokedByUserId);
        builder.HasOne<Student>().WithMany().HasForeignKey(card => new { card.CentreId, card.StudentId })
            .HasPrincipalKey(student => new { student.CentreId, student.Id })
            .HasConstraintName("fk_student_cards_students").OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(card => card.IssuedByUserId)
            .HasConstraintName("fk_student_cards_issued_by").OnDelete(DeleteBehavior.Restrict);
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(card => card.RevokedByUserId)
            .HasConstraintName("fk_student_cards_revoked_by").OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(card => new { card.CentreId, card.TokenHash }).IsUnique()
            .HasDatabaseName("ux_student_cards_centre_token_hash");
        builder.HasIndex(card => new { card.CentreId, card.StudentId }).IsUnique()
            .HasFilter("status = 'active'").HasDatabaseName("ux_student_cards_active_student");
        builder.HasRowVersion();
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }
}
