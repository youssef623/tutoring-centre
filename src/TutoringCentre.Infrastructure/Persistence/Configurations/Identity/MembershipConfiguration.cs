using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Identity;

/// <summary>Maps the Membership entity to identity.memberships.</summary>
internal sealed class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    private const int RoleMaxLength = 20;
    private const int StatusMaxLength = 10;

    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            "memberships",
            Schemas.Identity,
            table =>
            {
                table.HasCheckConstraint("ck_memberships_role", "role IN ('owner','teacher','secretary')");
                table.HasCheckConstraint("ck_memberships_status", "status IN ('active','inactive')");
            });

        builder.HasKey(membership => membership.Id);
        builder.Property(membership => membership.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.Property(membership => membership.UserId).IsRequired();
        builder
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(membership => membership.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(membership => membership.UserId).HasDatabaseName("ix_memberships_user_id");

        builder.Property(membership => membership.CentreId).IsRequired();
        builder
            .HasOne<Centre>()
            .WithMany()
            .HasForeignKey(membership => membership.CentreId)
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .Property(membership => membership.Role)
            .HasConversion(new ValueConverter<StaffRole, string>(
                role => RoleToDatabase(role),
                value => RoleFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(RoleMaxLength);

        builder
            .Property(membership => membership.Status)
            .HasConversion(new ValueConverter<MembershipStatus, string>(
                status => StatusToDatabase(status),
                value => StatusFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(StatusMaxLength);

        builder.HasIndex(membership => new { membership.CentreId, membership.UserId })
            .IsUnique()
            .HasDatabaseName("ux_memberships_centre_user");

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }

    private static string RoleToDatabase(StaffRole role) => role switch
    {
        StaffRole.Owner => "owner",
        StaffRole.Teacher => "teacher",
        StaffRole.Secretary => "secretary",
        _ => throw new ArgumentOutOfRangeException(nameof(role), role, "Unsupported staff role."),
    };

    private static StaffRole RoleFromDatabase(string value) => value switch
    {
        "owner" => StaffRole.Owner,
        "teacher" => StaffRole.Teacher,
        "secretary" => StaffRole.Secretary,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported role value in the database."),
    };

    private static string StatusToDatabase(MembershipStatus status) => status switch
    {
        MembershipStatus.Active => "active",
        MembershipStatus.Inactive => "inactive",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported membership status."),
    };

    private static MembershipStatus StatusFromDatabase(string value) => value switch
    {
        "active" => MembershipStatus.Active,
        "inactive" => MembershipStatus.Inactive,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported status value in the database."),
    };
}
