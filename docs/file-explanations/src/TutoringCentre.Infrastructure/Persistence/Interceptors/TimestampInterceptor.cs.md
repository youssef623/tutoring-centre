# src/TutoringCentre.Infrastructure/Persistence/Interceptors/TimestampInterceptor.cs

## Purpose
An EF Core `SaveChangesInterceptor` that stamps the **shadow properties** `CreatedAt` (on Added entries) and `UpdatedAt` (on Modified entries) from `IClock.UtcNow`. The properties are declared in entity configurations, so Domain classes never contain persistence metadata (lines 7–10).

## Where it fits
Infrastructure/Persistence/Interceptors, `internal sealed`. Registered singleton and attached in `AddDbContext` (`DependencyInjection.cs:47,53`). It depends on the `IClock` port.

## Walkthrough
- **Lines 13–14:** property-name constants.
- **Lines 18–22:** the constructor takes `IClock`.
- **Lines 24–29, `SavingChanges`** (sync override): calls `Stamp(eventData.Context)`, then the base.
- **Lines 31–39, `SavingChangesAsync`** (async override): the same.
- **Lines 41–60, `Stamp`:**
  1. Returns if the context is null.
  2. Reads `_clock.UtcNow` **once**, so every entry in the save gets the same timestamp.
  3. For each `ChangeTracker.Entries()`:
     - Added, and the metadata has `CreatedAt` → set it.
     - Otherwise Modified, and the metadata has `UpdatedAt` → set it.

## Concepts used
- **EF Core interceptors:** hooks around EF operations.
- **Shadow properties:** columns in the model with no CLR property.
- **Opt-in per entity:** `FindProperty(...) is not null` means only entities that declare the property get stamped.

## Data and control flow
`SaveChanges` → interceptor → entries → shadow property values → SQL `INSERT`/`UPDATE`.

## Configuration and environment
None.

## Gotchas and issues
- **No effect yet.** No entity configuration declares these shadow properties.
- An Added entry does **not** get `UpdatedAt`, so it stays NULL (or its default) until the first modification. That is intentional by the `else if`, but worth knowing.
- It works because the interceptor is stateless. A singleton interceptor shared by all contexts is safe only while it stays stateless.
- **Untested** (it's `internal`, and there are no integration tests).

## Related files
- [IClock.cs](../../../TutoringCentre.Application/Common/Ports/IClock.cs.md)
- [DependencyInjection.cs](../../DependencyInjection.cs.md)
- [UnitOfWork.cs](../UnitOfWork.cs.md)
