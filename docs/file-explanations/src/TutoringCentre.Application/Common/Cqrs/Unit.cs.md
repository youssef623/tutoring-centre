# src/TutoringCentre.Application/Common/Cqrs/Unit.cs

## Purpose

Placeholder 'no data' response type for commands that only succeed or fail.

## Where It Fits

Application/Common/Cqrs. **Unused** by product code (verified by search); referenced only in doc comments.

## Walkthrough

`public readonly record struct Unit` with `public static readonly Unit Value;`. A `readonly record struct` is a value type with value equality; there is exactly one possible value. It exists because C# has no `void` generic argument, so `ICommand<Unit>` is how a command returns nothing.

## Concepts Used

### Records, immutability and primary constructors

#### What it means

A C# `record` is a type with value-based equality and (by default) immutable properties - suited to messages like commands, DTOs and errors. A *primary constructor* (`class X(IDep dep)`) declares constructor parameters on the type header; they are captured for use in members.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

`readonly record struct`.

#### How it works here

Declaration.

#### Why it matters here

A zero-size-meaning value type.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Dead code today; keep or remove deliberately.

## Related Files

- [`src/TutoringCentre.Application/Common/Cqrs/ICommand.cs`](ICommand.cs.md)
