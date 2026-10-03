# src/TutoringCentre.Domain/Common/Entity.cs

## Purpose
Abstract base class for all entities. It gives every entity a `Guid Id` that is a **UUIDv7 generated in the application** when the object is constructed, not by the database.

## Where it fits
Domain/Common. Inherited by `Centre`. Tested by `tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs`.

## Walkthrough
- **Line 6:** `protected Entity() => Id = Guid.CreateVersion7();`. The constructor runs for every subclass, including EF's materialisation constructor. EF then overwrites `Id` from the row.
- **Line 9:** `public Guid Id { get; private set; }`. Only the entity itself (and EF via reflection, "Day 8") can set it.

## Concepts used
- **UUIDv7:** a time-ordered UUID (timestamp prefix plus randomness). Creating it in the application means the id is known before saving, and sequential-ish inserts keep B-tree indexes efficient.
- **Template base class:** shared identity for all entities.

## Data and control flow
Construction → `Guid.CreateVersion7()` → `Id`.

## Configuration and environment
None.

## Gotchas and issues
- There is no equality override. Two instances with the same `Id` are not `Equals`. That is a deliberate choice or a pending decision; either way it isn't documented.
- An EF-materialised entity briefly gets a fresh UUIDv7 before EF sets the real one. Harmless.

## Related files
- [Centre.cs](../Centres/Centre.cs.md)
- [EntityTests.cs](../../../tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs.md)
