# src/TutoringCentre.Application/Common/Security/ICurrentActor.cs

## Purpose
Read-only access to the current scope's actor: `Actor Actor { get; }`. This is what handlers will inject.

## Where it fits
Application/Common/Security. Implemented by `CurrentActorContext`. Registered as a forwarding scoped service (`Application/DependencyInjection.cs:16`). No consumers yet.

## Walkthrough
- **Lines 4–7:** a single read-only property.

## Concepts used
- **Interface segregation:** consumers get no setter.

## Data and control flow
Not applicable.

## Configuration and environment
None.

## Gotchas and issues
None found.

## Related files
- [CurrentActorContext.cs](CurrentActorContext.cs.md)
- [Actor.cs](Actor.cs.md)
