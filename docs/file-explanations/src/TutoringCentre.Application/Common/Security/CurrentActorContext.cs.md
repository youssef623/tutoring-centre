# src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs

## Purpose
Holds the actor for one scope. It starts as `AnonymousActor`. Trusted edge code (Api middleware, job runner, seed) calls `Set` **exactly once**. Handlers depend on `ICurrentActor`, which has no setter (lines 3–6).

## Where it fits
Application/Common/Security. Registered scoped (`Application/DependencyInjection.cs:15-16`), both as itself and as `ICurrentActor`. Tested by `CurrentActorContextTests`. No production code calls `Set` yet.

## Walkthrough
- **Line 9:** `private bool _isSet;`
- **Line 11:** `public Actor Actor { get; private set; } = new AnonymousActor();`
- **Lines 15–26, `Set(Actor actor)`:**
  1. Null guard (`ArgumentNullException`).
  2. If `_isSet`, throw `InvalidOperationException("...already been set and cannot be replaced.")`.
  3. Assign and mark as set.

## Concepts used
- **Ambient context via DI scope:** per-request state without passing it through every method.
- **Set-once (write-once) guard:** stops later code from escalating or switching identity mid-request.
- **Interface segregation:** a read-only interface for consumers, the concrete type for the writer.

## Data and control flow
Edge code → `Set(actor)` → `Actor` property → handlers via `ICurrentActor`.

## Configuration and environment
None.

## Gotchas and issues
- Not thread-safe (plain bool), which is fine for one request scope.
- Anyone who can resolve the concrete `CurrentActorContext` can call `Set` first. "Trusted edge code" is a convention.

## Related files
- [Actor.cs](Actor.cs.md)
- [ICurrentActor.cs](ICurrentActor.cs.md)
- [CurrentActorContextTests.cs](../../../../tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs.md)
