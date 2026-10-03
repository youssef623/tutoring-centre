# src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs

## Purpose

The message representing the intent to create a centre.

## Where It Fits

Application/Centres/Commands/CreateCentre. Built by `SeedCommand` and tests; validated by `CreateCentreValidator`; handled by `CreateCentreHandler`.

## Walkthrough

`public sealed record CreateCentreCommand(string Name, string Slug, string TimeZoneId, SupportedLocale DefaultLocale) : ICommand<CreateCentreResult>;`. Doc: carries exactly the four fields a caller may provide - no actor, no id, no timestamps (those come from context/domain/interceptor). It is `public` because the Api project constructs it.

## Concepts Used

### Records, immutability and primary constructors

#### What it means

A C# `record` is a type with value-based equality and (by default) immutable properties - suited to messages like commands, DTOs and errors. A *primary constructor* (`class X(IDep dep)`) declares constructor parameters on the type header; they are captured for use in members.

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Positional record.

#### How it works here

Declaration.

#### Why it matters here

Immutable message with value equality.

### CQRS and the dispatcher pipeline

#### What it means

CQRS separates *commands* (intent to change state) from *queries* (read-only questions). A *handler* executes exactly one command or query. A *dispatcher* is the single entry point that finds the handler and wraps shared steps (validation, transaction, logging) around it, so every use case behaves the same way regardless of who calls it (web endpoint, CLI, future bot).

(Full tutorial with execution traces: [PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../../PROJECT_OVERVIEW.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

`ICommand<CreateCentreResult>`.

#### How it works here

Interface list.

#### Why it matters here

Pairs command to response for the dispatcher.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No HTTP endpoint binds this type; strict JSON (`UnmappedMemberHandling.Disallow`) would apply if one were added.

## Related Files

- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs`](CreateCentreHandler.cs.md)
- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs`](CreateCentreValidator.cs.md)
- [`src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreResult.cs`](CreateCentreResult.cs.md)
- [`src/TutoringCentre.Api/Cli/SeedCommand.cs`](../../../../TutoringCentre.Api/Cli/SeedCommand.cs.md)
