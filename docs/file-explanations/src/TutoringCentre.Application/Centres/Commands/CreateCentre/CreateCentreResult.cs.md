# src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreResult.cs

## Purpose

The success payload of `CreateCentreCommand`: the new centre's id and slug.

## Where It Fits

Application/Centres/Commands/CreateCentre. Created at the end of `CreateCentreHandler.HandleAsync`; read by `SeedCommand` (only `IsSuccess`) and tests.

## Walkthrough

`public sealed record CreateCentreResult(Guid CentreId, string Slug);` - deliberately small: callers need the id to refer to the centre and the slug to build links; the entity itself never leaves the layer.

## Concepts Used

### Records, immutability and primary constructors

#### What it means

A C# `record` is a type with value-based equality and (by default) immutable properties - suited to messages like commands, DTOs and errors. A *primary constructor* (`class X(IDep dep)`) declares constructor parameters on the type header; they are captured for use in members.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

DTO-style record.

#### How it works here

Declaration.

#### Why it matters here

Entities are not exposed across the boundary.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

None.

## Related Files

- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`](CreateCentreHandler.cs.md)
- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs`](CreateCentreCommand.cs.md)
