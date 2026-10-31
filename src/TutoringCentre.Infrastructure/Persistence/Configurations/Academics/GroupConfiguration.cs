using TutoringCentre.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>
/// Maps Group to academics.groups. Both the subject and teacher references are composite foreign keys through
/// (centre_id, id) — the same pattern as StudentParentLink's link to Parent — so a group can never store a
/// reference into another centre's subject or teacher, even through privileged SQL. No navigation property to
/// either: a handler reaches Subject or Teacher only through their own repositories.
/// </summary>
internal sealed class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    private const int StatusMaxLength = 10;
    private const string TableName = "groups";

    public void Configure(EntityTypeBuilder<Group> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table =>
            {
                table.HasCheckConstraint("ck_groups_status", "status IN ('active','archived')");
                table.HasCheckConstraint("ck_groups_monthly_fee", "monthly_fee >= 0");
                table.HasCheckConstraint("ck_groups_capacity", "capacity BETWEEN 1 AND 200");
            });

        builder.HasKey(group => group.Id);
        builder.Property(group => group.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);

        builder.Property(group => group.Name).IsRequired().HasMaxLength(Group.NameMaxLength);
        builder.Property(group => group.NormalizedName).IsRequired().HasMaxLength(Group.NameMaxLength);
        builder.Property(group => group.SubjectId).IsRequired();
        builder.Property(group => group.TeacherId).IsRequired();
        builder.Property(group => group.Capacity).IsRequired();
        builder.Property(group => group.MonthlyFee).HasConversion(x => x.Amount, x => TutoringCentre.Domain.Common.Money.Create(x).Value).HasPrecision(10, 2).HasDefaultValueSql("0");
        builder.Navigation(group => group.MonthlyFees).UsePropertyAccessMode(PropertyAccessMode.Field);

        builder
            .Property(group => group.Status)
            .HasConversion(new ValueConverter<GroupStatus, string>(
                status => StatusToDatabase(status),
                value => StatusFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(StatusMaxLength);

        builder
            .HasOne<Subject>()
            .WithMany()
            .HasForeignKey(group => new { group.CentreId, group.SubjectId })
            .HasPrincipalKey(subject => new { subject.CentreId, subject.Id })
            .HasConstraintName($"fk_{TableName}_subjects")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasOne<Teacher>()
            .WithMany()
            .HasForeignKey(group => new { group.CentreId, group.TeacherId })
            .HasPrincipalKey(teacher => new { teacher.CentreId, teacher.Id })
            .HasConstraintName($"fk_{TableName}_teachers")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(group => new { group.CentreId, group.TeacherId })
            .HasDatabaseName($"ix_{TableName}_centre_teacher");

        builder
            .HasIndex(group => new { group.CentreId, group.SubjectId })
            .HasDatabaseName($"ix_{TableName}_centre_subject");

        builder
            .HasIndex(group => new { group.CentreId, group.NormalizedName })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_centre_normalized_name");

        builder.HasRowVersion();

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }

    private static string StatusToDatabase(GroupStatus status) => status switch
    {
        GroupStatus.Active => "active",
        GroupStatus.Archived => "archived",
        _ => throw new ArgumentOutOfRangeException(nameof(status), status, "Unsupported group status."),
    };

    private static GroupStatus StatusFromDatabase(string value) => value switch
    {
        "active" => GroupStatus.Active,
        "archived" => GroupStatus.Archived,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported status value in the database."),
    };
}
