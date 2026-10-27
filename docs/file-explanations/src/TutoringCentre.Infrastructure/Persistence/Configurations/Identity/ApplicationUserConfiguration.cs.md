# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps ApplicationUser to identity.users. Applied after Identity's own base configuration, so it wins.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 28 lines and 1,260 bytes. Source hash: `da95552fc46cee4450d1a915dd4f45f86880074826757cd1756d4f1b1c793bc1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using TutoringCentre.Infrastructure.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs#L8): `internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>`
- [Line 13](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/ApplicationUserConfiguration.cs#L13): `public void Configure(EntityTypeBuilder<ApplicationUser> builder)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TutoringCentre.Infrastructure.Identity;

namespace TutoringCentre.Infrastructure.Persistence.Configurations.Identity;

/// <summary>Maps ApplicationUser to identity.users. Applied after Identity's own base configuration, so it wins.</summary>
internal sealed class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    private const int DisplayNameMaxLength = 120;
    private const int PreferredLocaleMaxLength = 2;

    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ToTable(
            "users",
            Schemas.Identity,
            table => table.HasCheckConstraint("ck_users_preferred_locale", "preferred_locale IN ('ar','en')"));

        builder.Property(user => user.DisplayName).IsRequired().HasMaxLength(DisplayNameMaxLength);
        builder.Property(user => user.PreferredLocale).IsRequired().HasMaxLength(PreferredLocaleMaxLength);
        builder.Property(user => user.MustChangePassword).IsRequired();

        builder.HasIndex(user => user.NormalizedEmail).IsUnique().HasDatabaseName("ux_users_normalized_email");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
