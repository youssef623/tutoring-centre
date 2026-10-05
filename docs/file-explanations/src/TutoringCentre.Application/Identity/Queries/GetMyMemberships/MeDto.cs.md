# src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/MeDto.cs

## Purpose

The response shape of `GET /api/me` and login: profile, active centre/role and active memberships.

## Where It Fits

Application/Identity/Queries/GetMyMemberships. Produced by `GetMyMembershipsHandler`; published in OpenAPI and generated as the TypeScript `MeDto`.

## Walkthrough

`MeDto(UserId, DisplayName, Email, PreferredLocale, Guid? ActiveCentreId, StaffRole? ActiveRole, IReadOnlyList<MembershipDto> Memberships)`. `ActiveCentreId`/`ActiveRole` are null unless the actor's selected centre is still among the user's active memberships (doc comment).

## Concepts Used

### Records, immutability and primary constructors

#### What it means

A C# `record` is a type with value-based equality and (by default) immutable properties - suited to messages like commands, DTOs and errors. A *primary constructor* (`class X(IDep dep)`) declares constructor parameters on the type header; they are captured for use in members.

(Full tutorial with execution traces: [PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher](../../../../../../PROJECT_OVERVIEW2.md#63-cqrs-and-the-hand-written-dispatcher).)

#### Where it appears in this file

DTO record.

#### How it works here

Declaration.

#### Why it matters here

Stable contract for the frontend session.

## Data and Control Flow

No runtime data flows through this file; it is consumed by tooling or readers, not executed by the application.

## Configuration and Environment

None. The file reads no environment variables and declares no configuration keys, URLs, ports or credentials.

## Gotchas and Issues

Enum `StaffRole` serialises as a camelCase string (`owner`, `teacher`, `secretary`) via the JSON converter in `Program.cs`.

## Related Files

- [`src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs`](GetMyMembershipsHandler.cs.md)
- [`frontend/src/features/session/meQueryOptions.ts`](../../../../../frontend/src/features/session/meQueryOptions.ts.md)
