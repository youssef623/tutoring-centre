using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Centres;

/// <summary>Maps the Centre entity to platform.centres. Lengths mirror the Domain's limits exactly.</summary>
internal sealed class CentreConfiguration : IEntityTypeConfiguration<Centre>
{
    private const int TimeZoneIdMaxLength = 64;
    private const int LocaleMaxLength = 2;

    public void Configure(EntityTypeBuilder<Centre> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        // The database repeats the locale rule (defence against rows written outside the app).
        builder.ToTable(
            "centres",
            Schemas.Platform,
            table => table.HasCheckConstraint("ck_centres_default_locale", "default_locale IN ('ar','en')"));

        builder.HasKey(centre => centre.Id);
        builder.Property(centre => centre.Id).ValueGeneratedNever(); // UUIDv7 is assigned by the Domain

        builder.Property(centre => centre.Name).IsRequired().HasMaxLength(Centre.NameMaxLength);

        builder.Property(centre => centre.Slug).IsRequired().HasMaxLength(Centre.SlugMaxLength);
        builder.HasIndex(centre => centre.Slug).IsUnique().HasDatabaseName("ux_centres_slug");

        builder.Property(centre => centre.TimeZoneId).IsRequired().HasMaxLength(TimeZoneIdMaxLength);

        builder
            .Property(centre => centre.DefaultLocale)
            .HasConversion(new ValueConverter<SupportedLocale, string>(
                locale => LocaleToDatabase(locale),
                value => LocaleFromDatabase(value)))
            .IsRequired()
            .HasMaxLength(LocaleMaxLength);

        builder.HasRowVersion();

        // Persistence metadata: shadow properties set by TimestampInterceptor, invisible to the Domain.
        builder.Property<DateTimeOffset>("CreatedAt").IsRequired();
        builder.Property<DateTimeOffset?>("UpdatedAt");
    }

    private static string LocaleToDatabase(SupportedLocale locale) => locale switch
    {
        SupportedLocale.Ar => "ar",
        SupportedLocale.En => "en",
        _ => throw new ArgumentOutOfRangeException(nameof(locale), locale, "Unsupported locale."),
    };

    private static SupportedLocale LocaleFromDatabase(string value) => value switch
    {
        "ar" => SupportedLocale.Ar,
        "en" => SupportedLocale.En,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported locale value in the database."),
    };
}
