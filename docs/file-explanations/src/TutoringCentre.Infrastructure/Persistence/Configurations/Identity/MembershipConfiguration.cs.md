# `src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs`

## Purpose

Source summary (intent, not independent execution proof): Maps the Membership entity to identity.memberships.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 103 lines and 4,164 bytes. Source hash: `a5fb7ea8462e64ea4d6d234a2ccca511a231e403b1f9dcff92b9bc53e27ba805`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.EntityFrameworkCore.Metadata.Builders;`
- Line 3: `using Microsoft.EntityFrameworkCore.Storage.ValueConversion;`
- Line 4: `using TutoringCentre.Domain.Centres;`
- Line 5: `using TutoringCentre.Domain.Identity;`
- Line 6: `using TutoringCentre.Infrastructure.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs#L11): `internal sealed class MembershipConfiguration : IEntityTypeConfiguration<Membership>`
- [Line 16](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs#L16): `public void Configure(EntityTypeBuilder<Membership> builder)`
- [Line 74](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs#L74): `private static string RoleToDatabase(StaffRole role) => role switch`
- [Line 82](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs#L82): `private static StaffRole RoleFromDatabase(string value) => value switch`
- [Line 90](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs#L90): `private static string StatusToDatabase(MembershipStatus status) => status switch`
- [Line 97](../../../../../../../src/TutoringCentre.Infrastructure/Persistence/Configurations/Identity/MembershipConfiguration.cs#L97): `private static MembershipStatus StatusFromDatabase(string value) => value switch`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
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

        builder.HasRowVersion();

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
```

The excerpt stops at line 90; use the original file for the remaining 13 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
