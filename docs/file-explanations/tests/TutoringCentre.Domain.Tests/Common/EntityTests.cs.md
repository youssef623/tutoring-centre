# tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs

## Purpose
Tests the `Entity` base class: new entities get different ids, and ids are version-7 UUIDs.

## Where it fits
Domain.Tests. It tests `Domain/Common/Entity.cs`, using a private nested `TestEntity : Entity` (line 8).

## Walkthrough
- **Lines 10–17:** two instances have different ids.
- **Lines 19–25:** `entity.Id.Version == 7` (`Guid.Version`, available since .NET 9).

## Concepts used
- **Testing an abstract class** through a minimal concrete subclass.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
None found.

## Related files
- [Entity.cs](../../../src/TutoringCentre.Domain/Common/Entity.cs.md)
