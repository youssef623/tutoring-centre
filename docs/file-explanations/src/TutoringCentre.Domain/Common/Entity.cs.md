# src/TutoringCentre.Domain/Common/Entity.cs

## Purpose

Base class for domain entities: gives each one a UUIDv7 identity at construction.

## Where It Fits

Domain/Common. Inherited by `Centre`. EF Core maps `Id` (configured in `CentreConfiguration`).

## Walkthrough

`abstract class Entity` with `protected Entity() => Id = Guid.CreateVersion7();` (line 5) and `public Guid Id { get; private set; }` (line 8). The ID exists before any database call, so `CentreConfiguration` sets `ValueGeneratedNever()`. The private setter stops outside code from changing identity; EF Core can still set it via reflection when materialising rows. `EntityTests` check uniqueness and `Id.Version == 7`.

## Concepts Used

### UUIDv7 identifiers

#### What it means

A UUID is a 128-bit identifier. Version 7 puts a timestamp in the leading bits, so ids sort roughly by creation time. This repo assigns ids in code at construction instead of letting the database generate them.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#66-entities-encapsulation-and-factory-methods](../../../../PROJECT_OVERVIEW.md#66-entities-encapsulation-and-factory-methods).)

#### Where it appears in this file

`Guid.CreateVersion7()`.

#### How it works here

Line 5.

#### Why it matters here

Time-ordered ids; the repo does not state the reason - an index-locality benefit is an interpretation.

### Entities, encapsulation and factory methods

#### What it means

An *entity* has an identity that stays the same while its data changes. *Encapsulation* keeps its state behind rules (private setters, private constructors). A *factory method* is the only public way to build one, so an invalid instance cannot exist.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#66-entities-encapsulation-and-factory-methods](../../../../PROJECT_OVERVIEW.md#66-entities-encapsulation-and-factory-methods).)

#### Where it appears in this file

The base class.

#### How it works here

`Entity`.

#### Why it matters here

One identity rule for all entities.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Every entity must call the base constructor; EF's materialisation constructor in `Centre` does, which generates a throwaway v7 id that EF then overwrites with the stored id.

## Related Files

- [`src/TutoringCentre.Domain/Centres/Centre.cs`](../Centres/Centre.cs.md)
- [`src/TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs`](../../TutoringCentre.Infrastructure/Persistence/Configurations/Centres/CentreConfiguration.cs.md)
- [`tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs`](../../../tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs.md)
