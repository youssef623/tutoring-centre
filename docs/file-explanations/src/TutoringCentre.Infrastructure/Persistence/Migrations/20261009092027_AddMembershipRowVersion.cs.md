# `src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.cs`

## Purpose

Declares `AddMembershipRowVersion`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **migration**. [Open current file](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 24 lines and 866 bytes. Source hash: `ca29f3cdae0e3c64b238db6c84c2b11d8a0d07f2fd0774b293ad762f14f5a844`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore.Migrations;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../../src/TutoringCentre.Infrastructure/Persistence/Migrations/20261009092027_AddMembershipRowVersion.cs#L8): `public partial class AddMembershipRowVersion : Migration`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TutoringCentre.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMembershipRowVersion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No DDL (Task 29.2, same reasoning as Task 23.2's AddSubjects migration): "xmin" is PostgreSQL's
            // own system column on every table already, not something a migration creates — EF's generated
            // AddColumn for it is removed by hand. This migration exists only to keep the model snapshot in
            // step with Membership now mapping HasRowVersion().
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
