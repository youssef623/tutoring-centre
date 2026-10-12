using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Academics;

/// <summary>Maps the Parent entity to academics.parents. Lengths mirror the Domain's limits exactly.</summary>
internal sealed class ParentConfiguration : IEntityTypeConfiguration<Parent>
{
    private const int PhoneMaxLength = 16;
    private const string TableName = "parents";

    public void Configure(EntityTypeBuilder<Parent> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            TableName,
            Schemas.Academics,
            table => table.HasCheckConstraint("ck_parents_phone_format", PhoneFormatCheck.Sql));

        builder.HasKey(parent => parent.Id);
        builder.Property(parent => parent.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.ConfigureTenantOwned(TableName);

        builder.Property(parent => parent.FullName).IsRequired().HasMaxLength(Parent.NameMaxLength);
        builder.Property(parent => parent.SearchName).IsRequired().HasMaxLength(Parent.NameMaxLength);

        builder
            .Property(parent => parent.Phone)
            .HasConversion(PhoneNumberConverters.Required)
            .IsRequired()
            .HasMaxLength(PhoneMaxLength);

        builder
            .HasIndex(parent => new { parent.CentreId, parent.SearchName })
            .HasDatabaseName($"ix_{TableName}_centre_search_name");

        builder
            .HasIndex(parent => new { parent.CentreId, parent.Phone })
            .IsUnique()
            .HasDatabaseName($"ux_{TableName}_centre_phone");

        builder.HasRowVersion();

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }
}
