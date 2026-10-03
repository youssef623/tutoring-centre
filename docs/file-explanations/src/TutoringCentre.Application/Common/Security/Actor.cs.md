# src/TutoringCentre.Application/Common/Security/Actor.cs

## Purpose
Models **who** is executing a use case and **in which centre** (tenant). There is exactly one actor per request or job scope. Handlers read it; they never take identity or tenant from request data (lines 3–6).

## Where it fits
Application/Common/Security. Held by `CurrentActorContext` and read through `ICurrentActor`. Tested by `CurrentActorContextTests`. The future tenant check in `Dispatcher` (Month 2) will read `CentreId`.

## Walkthrough
- **Lines 7–11:** `public abstract record Actor { public abstract Guid? CentreId { get; } }`. Null means no centre applies.
- **Lines 14–17:** `public sealed record SystemActor(Guid? CentreId) : Actor`, the application itself (background jobs, the seed command, CLI tools). It may act in one centre or platform-wide (null). It overrides the abstract property with the positional parameter's value.
- **Lines 20–23:** `public sealed record AnonymousActor : Actor` with `CentreId => null`. Example from the doc: the login request itself.

## Concepts used
- **Closed hierarchy of records:** an algebraic-data-type style. A future user actor (staff or parent) will be another sealed record.
- **Never trust tenant ids from input:** the tenant comes from the authenticated context.

## Data and control flow
Edge code creates an actor → `CurrentActorContext.Set` → handlers read `ICurrentActor.Actor`.

## Configuration and environment
None.

## Gotchas and issues
- There is no authenticated-user actor yet. Authentication isn't implemented.
- The "seed command" and "CLI tools" mentioned in the doc don't exist yet.

## Related files
- [CurrentActorContext.cs](CurrentActorContext.cs.md)
- [ICurrentActor.cs](ICurrentActor.cs.md)
- [ITenantOwned.cs](../../../TutoringCentre.Domain/Common/ITenantOwned.cs.md)
