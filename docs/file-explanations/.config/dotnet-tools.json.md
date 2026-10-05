# .config/dotnet-tools.json

## Purpose

Local .NET tool manifest that pins the `dotnet-ef` command-line tool to the same version as the EF Core packages.

## Where It Fits

Read by `dotnet tool restore` (CI step 'Tool restore' in `.github/workflows/ci.yml`) and by developers before running `dotnet ef migrations ...`. Used against `src/TutoringCentre.Infrastructure` (migrations) with `src/TutoringCentre.Api` as the startup project.

## Walkthrough

- `isRoot: true` - this manifest is the top of the lookup chain.
- `dotnet-ef` `version 10.0.12` - same as `Microsoft.EntityFrameworkCore` in `Directory.Packages.props`.
- `commands: ["dotnet-ef"]` - the command that becomes available.
- `rollForward: false` - never run a different tool version than pinned.
If the tool and the EF packages drift apart, `migrations add` can generate a snapshot with a different `ProductVersion` annotation (the snapshot currently says `10.0.12`).

## Concepts Used

### Database migrations

#### What it means

A migration is versioned code describing one schema change with an `Up` (apply) and `Down` (revert). EF records applied migrations in a history table and keeps a *model snapshot* of the last known model; the next `migrations add` diffs the current model against it.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here](../../PROJECT_OVERVIEW2.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

The `dotnet-ef` tool entry.

#### How it works here

`dotnet tool restore` installs the pinned tool into a local cache; CI then runs `dotnet ef migrations has-pending-model-changes`, which builds the model and compares it with `AppDbContextModelSnapshot`.

#### Why it matters here

It is how the 'forgot to add a migration' failure is detected automatically.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None in the file. (CI sets `ConnectionStrings__Postgres` when invoking the tool.)

## Gotchas and Issues

None.

## Related Files

- [`Directory.Packages.props`](../Directory.Packages.props.md)
- [`.github/workflows/ci.yml`](../.github/workflows/ci.yml.md)
- `src/TutoringCentre.Infrastructure/Persistence/Migrations/AppDbContextModelSnapshot.cs` (generated / lockfile / media: no separate explanation, see INDEX)
