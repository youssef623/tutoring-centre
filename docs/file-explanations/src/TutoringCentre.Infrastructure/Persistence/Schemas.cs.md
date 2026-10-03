# src/TutoringCentre.Infrastructure/Persistence/Schemas.cs

## Purpose
Constants naming the PostgreSQL schema for each feature module. Every entity configuration is meant to use them in `ToTable(name, schema)` (line 3).

## Where it fits
Infrastructure/Persistence, `internal static`. `Platform` is used by `DependencyInjection.cs:51` for the migrations history table. `Identity` is **unused** (verified by grep).

## Walkthrough
- **Line 6:** `Platform = "platform"`, presumably for centres and platform-wide data.
- **Line 7:** `Identity = "identity"`, presumably for users and login (planned).

## Concepts used
- **Schema-per-module:** logical separation inside one database, matching ADR 0001's "module boundaries enforced with folders".

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- Which entities go into which schema is not documented anywhere except as implied by the names.

## Related files
- [DependencyInjection.cs](../DependencyInjection.cs.md)
- [AppDbContext.cs](AppDbContext.cs.md)
