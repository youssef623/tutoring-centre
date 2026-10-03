# tests/TutoringCentre.Domain.Tests/Common/ErrorTests.cs

## Purpose

Tests the two `Error.Validation` overloads.

## Where It Fits

Domain.Tests; tests `Domain/Common/Error.cs`.

## Walkthrough

`Validation_WithFields_ExposesTheFields`: Arrange dictionary `{slug: [...]}`; Act `Error.Validation(code,message,fields)`; Assert kind Validation and `Fields["slug"]` equals the array. `Validation_WithoutFields_HasNullFields`: Assert `Fields` is null. Not tested: other factories, record equality, null `fields` guard.

## Concepts Used

### Records, immutability and primary constructors

#### What it means

A C# `record` is a type with value-based equality and (by default) immutable properties - suited to messages like commands, DTOs and errors. A *primary constructor* (`class X(IDep dep)`) declares constructor parameters on the type header; they are captured for use in members.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Record under test.

#### How it works here

`Error`.

#### Why it matters here

Confirms the factory contract.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Domain/Common/Error.cs`](../../../src/TutoringCentre.Domain/Common/Error.cs.md)
