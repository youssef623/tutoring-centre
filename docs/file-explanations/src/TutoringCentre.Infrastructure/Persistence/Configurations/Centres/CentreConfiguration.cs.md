# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps the Centre entity to platform.centres. Lengths mirror the Domain's limits exactly.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 62 lines and 2,647 bytes. Source hash: `3735a73cc2500ee37b2c95cba76948384716e67624f8a37fa649254c80353196`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;`
- Line 4: `using TutoringCentre.Domain.Centres;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs#L9): `internal sealed class CentreConfiguration : IEntityTypeConfiguration<Centre>`
- [Line 14](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs#L14): `public void Configure(EntityTypeBuilder<Centre> builder)`
- [Line 49](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs#L49): `private static string LocaleToDatabase(SupportedLocale locale) => locale switch`
- [Line 56](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs#L56): `private static SupportedLocale LocaleFromDatabase(string value) => value switch`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
