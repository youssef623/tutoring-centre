# src/TutoringCentre.Application/Common/Security/ICurrentActor.cs

## Purpose

Read-only view of the actor of the current scope.

## Where It Fits

Application/Common/Security. Injected into handlers; resolves to `CurrentActorContext`.

## Walkthrough

`interface ICurrentActor { Actor Actor { get; } }` - no setter, so handlers cannot change identity (interface segregation: edge code uses the concrete context, handlers use this).

## Concepts Used

### Dependency Injection (DI) and the container

#### What it means

A class that needs a collaborator can create it itself (`new X()`), which hard-wires the choice, or it can *receive* it, usually as a constructor parameter. That second approach is dependency injection. A DI *container* stores *registrations* ("when asked for type A, build type B with lifetime L") and performs *resolution*: it picks a constructor, resolves each parameter recursively, builds the object and caches it according to the lifetime. This is inversion of control: the class no longer decides what it depends on.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning](../../../../../PROJECT_OVERVIEW.md#62-dependency-injection-lifetimes-and-scanning).)

#### Where it appears in this file

Interface vs concrete registration.

#### How it works here

Factory registration in `AddApplication` line 16.

#### Why it matters here

One instance, two capability levels.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

None.

## Related Files

- [`src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs`](CurrentActorContext.cs.md)
- [`src/TutoringCentre.Application/DependencyInjection.cs`](../../DependencyInjection.cs.md)
- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`](../../Centres/Commands/CreateCentre/CreateCentreHandler.cs.md)
