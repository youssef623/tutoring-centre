# src/TutoringCentre.Api/Auth/AntiforgeryTokenResponse.cs

## Purpose

Response record for `GET /api/auth/antiforgery`: just the request token string.

## Where It Fits

Api/Auth. Returned by `AuthEndpoints.GetAntiforgeryTokenAsync`; appears in the OpenAPI document and the generated `AntiforgeryTokenResponse` TypeScript interface.

## Walkthrough

`public sealed record AntiforgeryTokenResponse(string Token);` Doc: the cookie half of the pair is set directly on the response and never appears in the body, so a script only ever receives the request token.

## Concepts Used

### Records, immutability and primary constructors

#### What it means

A C# `record` is a type with value-based equality and (by default) immutable properties - suited to messages like commands, DTOs and errors. A *primary constructor* (`class X(IDep dep)`) declares constructor parameters on the type header; they are captured for use in members.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

DTO record.

#### How it works here

Declaration.

#### Why it matters here

Minimal wire contract.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

No bugs or surprises found while reading this file.

## Related Files

- [`src/TutoringCentre.Api/Auth/AuthEndpoints.cs`](AuthEndpoints.cs.md)
- [`frontend/src/api/csrf.ts`](../../../frontend/src/api/csrf.ts.md)
