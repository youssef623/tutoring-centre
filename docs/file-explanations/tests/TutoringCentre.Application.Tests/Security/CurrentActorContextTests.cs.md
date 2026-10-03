# tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs

## Purpose
Unit tests for `CurrentActorContext`'s three behaviours: it defaults to anonymous, `Set` takes effect, and setting twice throws.

## Where it fits
Application.Tests. It tests `Application/Common/Security/CurrentActorContext.cs` and uses `SystemActor` and `AnonymousActor`.

## Walkthrough
- **Lines 7–13, `Actor_ByDefault_IsAnonymous`:** `Assert.IsType<AnonymousActor>`.
- **Lines 15–27, `Set_ThenActor_ReturnsTheSetActor`:** `Assert.Same(systemActor, context.Actor)`, checking reference identity.
- **Lines 29–36, `Set_CalledTwice_ThrowsInvalidOperationException`.**

## Concepts used
- **Arrange/Act/Assert**, with **`Assert.Same`** for identity.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
- `Set(null)` (an `ArgumentNullException`) is not tested.

## Related files
- [CurrentActorContext.cs](../../../src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs.md)
- [Actor.cs](../../../src/TutoringCentre.Application/Common/Security/Actor.cs.md)
