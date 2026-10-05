# src/TutoringCentre.Api/Auth/LoginRequest.cs

## Purpose

The login request model and its FluentValidation validator, kept with the endpoint because login is not a CQRS command.

## Where It Fits

Api/Auth. `LoginAsync` instantiates `LoginRequestValidator` directly; the record is bound from the JSON body and published in OpenAPI.

## Walkthrough

`public sealed record LoginRequest(string Email, string Password);` `LoginRequestValidator : AbstractValidator<LoginRequest>` (`internal`): `Email` `NotEmpty().EmailAddress().MaximumLength(256)`; `Password` `NotEmpty().MaximumLength(128)` (constants 11-12). Doc: shape checks only; login accepts whatever password was set, with no composition rules (the password length policy lives in Identity options, Infrastructure). Because it is not registered through the Application assembly scan, the dispatcher never runs it.

## Concepts Used

### Two-tier validation

#### What it means

Validation asks whether input is acceptable. Cheap *shape* checks (required, length) can run first without touching business rules or the database; *business invariants* (slug format, time zone must exist) belong to the domain; *database constraints* are a last safety net against rows written by anything else.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#65-validation-two-tiers](../../../../PROJECT_OVERVIEW2.md#65-validation-two-tiers).)

#### Where it appears in this file

Shape-only validation.

#### How it works here

Validator class.

#### Why it matters here

Cheap rejection before any credential work.

### Records, immutability and primary constructors

#### What it means

A C# `record` is a type with value-based equality and (by default) immutable properties - suited to messages like commands, DTOs and errors. A *primary constructor* (`class X(IDep dep)`) declares constructor parameters on the type header; they are captured for use in members.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

Request record.

#### How it works here

Line 6.

#### Why it matters here

Immutable body model.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Validator is created with `new` per request (not from DI), unlike CQRS validators.

## Related Files

- [`src/TutoringCentre.Api/Auth/RequestValidation.cs`](RequestValidation.cs.md)
- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](AuthEndpoints.cs.md)
