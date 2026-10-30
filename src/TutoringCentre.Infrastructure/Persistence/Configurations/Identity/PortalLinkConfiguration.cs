using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Identity;

internal sealed class PortalLinkConfiguration : IEntityTypeConfiguration<PortalLink>
{
    public void Configure(EntityTypeBuilder<PortalLink> builder)
    {
        builder.ToTable("portal_links", Schemas.Identity, table =>
        {
            table.HasCheckConstraint("ck_portal_links_role", "role_key IN ('parent','student')");
            table.HasCheckConstraint("ck_portal_links_subject", "(role_key = 'parent' AND parent_id IS NOT NULL AND student_id IS NULL) OR (role_key = 'student' AND student_id IS NOT NULL AND parent_id IS NULL)");
        });
        builder.HasKey(link => link.Id);
        builder.Property(link => link.Id).ValueGeneratedNever();
        builder.ConfigureTenantOwned("portal_links");
        builder.Property(link => link.RoleKey).IsRequired().HasMaxLength(20);
        builder.Property(link => link.UserId).IsRequired();
        builder.Property(link => link.ParentId);
        builder.Property(link => link.StudentId);
        builder.Property(link => link.CreatedBy).IsRequired();
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.HasOne<Parent>().WithMany().HasForeignKey(link => new { link.CentreId, link.ParentId })
            .HasPrincipalKey(parent => new { parent.CentreId, parent.Id }).OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_portal_links_parents");
        builder.HasOne<Student>().WithMany().HasForeignKey(link => new { link.CentreId, link.StudentId })
            .HasPrincipalKey(student => new { student.CentreId, student.Id }).OnDelete(DeleteBehavior.Restrict)
            .HasConstraintName("fk_portal_links_students");
        builder.HasOne<ApplicationUser>().WithMany().HasForeignKey(link => link.CreatedBy)
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_portal_links_created_by");
        builder.HasOne<Membership>().WithMany().HasForeignKey(link => new { link.CentreId, link.UserId })
            .HasPrincipalKey(membership => new { membership.CentreId, membership.UserId })
            .OnDelete(DeleteBehavior.Restrict).HasConstraintName("fk_portal_links_membership_identity");
        // The role-bearing membership FK is migration SQL: its principal uses an enum converter,
        // while this immutable fact exposes the persisted role key directly.
        builder.HasIndex(link => new { link.CentreId, link.UserId }).IsUnique().HasDatabaseName("ux_portal_links_centre_user");
        builder.HasIndex(link => new { link.CentreId, link.ParentId }).IsUnique()
            .HasFilter("parent_id IS NOT NULL").HasDatabaseName("ux_portal_links_centre_parent");
        builder.HasIndex(link => new { link.CentreId, link.StudentId }).IsUnique()
            .HasFilter("student_id IS NOT NULL").HasDatabaseName("ux_portal_links_centre_student");
    }
}
