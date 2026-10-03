# src/TutoringCentre.Domain/Common/ITenantOwned.cs

## Purpose
A marker interface for entities that belong to exactly one centre: `Guid CentreId { get; }`. The doc comment says Infrastructure will apply tenant filtering to every implementer automatically (Month 2), and that `Centre` itself doesn't implement it because it *is* the tenant.

## Where it fits
Domain/Common. **No implementers and no consumers yet** (verified by grep). It will drive EF global query filters and/or PostgreSQL RLS.

## Walkthrough
- **Lines 7–10:** `public interface ITenantOwned { Guid CentreId { get; } }`.

## Concepts used
- **Marker interface for cross-cutting behaviour:** Infrastructure can find all tenant-owned types by reflection or EF metadata and apply filters uniformly.
- **Multi-tenancy, shared database with a tenant column.**

## Data and control flow
Not applicable yet.

## Configuration and environment
None.

## Gotchas and issues
- Currently unused, a placeholder for Month 2.

## Related files
- [Centre.cs](../Centres/Centre.cs.md)
- [Actor.cs](../../TutoringCentre.Application/Common/Security/Actor.cs.md)
- [AppDbContext.cs](../../TutoringCentre.Infrastructure/Persistence/AppDbContext.cs.md)
