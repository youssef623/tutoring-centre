# tests/TutoringCentre.Domain.Tests/Common/EntityTests.cs

## Purpose

Tests the `Entity` base class's identity generation.

## Where It Fits

Domain.Tests; tests `Domain/Common/Entity.cs`.

## Walkthrough

Private nested `TestEntity : Entity` (a concrete subclass because `Entity` is abstract). `NewEntities_GetDifferentIds`: two instances have different `Id`s. `Id_IsVersion7Uuid`: `Assert.Equal(7, entity.Id.Version)`. Not tested: time ordering of successive ids.

## Concepts Used

### UUIDv7 identifiers

#### What it means

A UUID is a 128-bit identifier. Version 7 puts a timestamp in the leading bits, so ids sort roughly by creation time. This repo assigns ids in code at construction instead of letting the database generate them.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#66-entities-encapsulation-and-factory-methods](../../../../PROJECT_OVERVIEW.md#66-entities-encapsulation-and-factory-methods).)

#### Where it appears in this file

UUID version assertion.

#### How it works here

`Id.Version`.

#### Why it matters here

Pins the identity scheme.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Domain/Common/Entity.cs`](../../../src/TutoringCentre.Domain/Common/Entity.cs.md)
