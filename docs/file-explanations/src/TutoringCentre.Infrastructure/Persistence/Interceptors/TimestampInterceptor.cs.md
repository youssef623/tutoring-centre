# src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs

## Purpose

Automatically fills `CreatedAt` and `UpdatedAt` shadow columns just before EF saves.

## Where It Fits

Infrastructure/Persistence/Interceptors, `internal`, singleton. Added to the context in `AddDbContext`. Depends on `IClock`.

## Walkthrough

`SaveChangesInterceptor` subclass. `SavingChanges` (24) and `SavingChangesAsync` (31) both call `Stamp(eventData.Context)` then the base. `Stamp` (41-): null-check the context; `now = _clock.UtcNow`; loop `context.ChangeTracker.Entries()`: if `Added` and the entity type has a property named `CreatedAt`, set `entry.Property("CreatedAt").CurrentValue = now`; else if `Modified` and has `UpdatedAt`, set that. The single `now` makes all rows in one save share a timestamp.
Edge cases: entities without those properties are ignored; nothing currently produces `Modified` centres, so `UpdatedAt` stays null (`CreateCentreTests` asserts `updated_at is null`).

## Concepts Used

### EF interceptors and shadow properties

#### What it means

An *interceptor* hooks into EF operations (here, just before `SaveChanges`). A *shadow property* is a column that exists in the EF model but not as a C# member, so persistence metadata (timestamps) never pollutes domain classes.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#69-ef-core-how-the-orm-actually-works-here](../../../../../PROJECT_OVERVIEW.md#69-ef-core-how-the-orm-actually-works-here).)

#### Where it appears in this file

SaveChanges hook.

#### How it works here

Overrides at lines 24 and 31.

#### Why it matters here

Cross-cutting persistence metadata without domain code.

### Time abstraction and time zones

#### What it means

Reading `DateTime.UtcNow` directly makes code untestable. A clock *port* (`IClock`) supplies time; tests substitute a fake. IANA zone ids (`Africa/Cairo`) resolve through the OS time-zone database and handle daylight saving.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning](../../../../../PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

`IClock` dependency.

#### How it works here

Constructor.

#### Why it matters here

Tests could substitute the clock.

### Service lifetimes (singleton, scoped, transient)

#### What it means

Lifetime says how long a container-built instance lives. *Singleton*: one for the whole application. *Scoped*: one per scope; in ASP.NET one scope = one HTTP request, and code can create its own scope (as the seed command does). *Transient*: a new instance on every resolution. A longer-lived service must not hold a shorter-lived one (a "captive dependency"), which `ValidateScopes` detects.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning](../../../../../PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

Registered singleton.

#### How it works here

`DependencyInjection.cs` line 52.

#### Why it matters here

Stateless; avoids per-request allocation.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Singleton interceptor used by a scoped context: fine because it holds only a singleton clock.

## Related Files

- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs`](../Configurations/Centres/CentreConfiguration.cs.md)
- [`src/TutoringCentre.Application/Common/Ports/IClock.cs`](../../../TutoringCentre.Application/Common/Ports/IClock.cs.md)
- [`src/TutoringCentre.Infrastructure/DependencyInjection.cs`](../../DependencyInjection.cs.md)
