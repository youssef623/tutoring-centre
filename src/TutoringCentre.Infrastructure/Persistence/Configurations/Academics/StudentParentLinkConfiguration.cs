using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>
/// Maps StudentParentLink, the Student aggregate's child entity, to academics.student_parents. The
/// relationship to Student is declared from <see cref="StudentConfiguration"/> (the aggregate root's own
/// backing field); only the relationship to Parent is declared here, with no navigation back — a link knows
/// a ParentId, never a Parent object, so no handler can reach across aggregates through it.
/// </summary>
internal sealed class StudentParentLinkConfiguration : IEntityTypeConfiguration<StudentParentLink>
{
    private const int RelationshipMaxLength = 10;
    private const string TableName = "student_parents";

    public void Configure(EntityTypeBuilder<StudentParentLink> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table => table.HasCheckConstraint("ck_student_parents_relationship", "relationship IN ('father','mother','guardian','other')"));

        builder.HasKey(link => link.Id);
        builder.Property(link => link.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);

        builder.Property(link => link.StudentId).IsRequired();
        builder.Property(link => link.ParentId).IsRequired();
        builder.Property(link => link.IsPrimary).IsRequired();

        builder
            .Property(link => link.Relationship)
            .HasConversion(new ValueConverter<ParentRelationship, string>(
                relationship => RelationshipToDatabase(relationship),
                value => RelationshipFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(RelationshipMaxLength);

        builder
            .HasOne<Parent>()
            .WithMany()
            .HasForeignKey(link => new { link.CentreId, link.ParentId })
            .HasPrincipalKey(parent => new { parent.CentreId, parent.Id })
            .HasConstraintName("fk_student_parents_parents")
            .OnDelete(DeleteBehavior.Restrict);

        builder
            .HasIndex(link => new { link.CentreId, link.ParentId })
            .HasDatabaseName($"ix_{TableName}_centre_parent");

        builder
            .HasIndex(link => new { link.CentreId, link.StudentId, link.ParentId })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_student_parent");

        // Partial unique index: only rows where is_primary is true, so at most one primary link per student.
        builder
            .HasIndex(link => new { link.CentreId, link.StudentId })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_one_primary")
            .HasFilter("is_primary");

        // Shadow CreatedAt only, set by TimestampInterceptor: no UpdatedAt and no row version — the aggregate
        // is versioned at its root (Student.LinksRevision + the students row's own xmin).
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
    }

    private static string RelationshipToDatabase(ParentRelationship value) => value switch
    {
        ParentRelationship.Father => "father",
        ParentRelationship.Mother => "mother",
        ParentRelationship.Guardian => "guardian",
        ParentRelationship.Other => "other",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported parent relationship."),
    };

    private static ParentRelationship RelationshipFromDatabase(string value) => value switch
    {
        "father" => ParentRelationship.Father,
        "mother" => ParentRelationship.Mother,
        "guardian" => ParentRelationship.Guardian,
        "other" => ParentRelationship.Other,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported relationship value in the database."),
    };
}
