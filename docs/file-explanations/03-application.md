# `src/TutoringCentre.Application`

Folder map generated from the per-file explanations. Part of the [file index](INDEX.md); the teaching overview is [`../PROJECT_OVERVIEW2.md`](../PROJECT_OVERVIEW2.md). Each row links to the full explanation of that file (purpose, where it fits, walkthrough, concepts, flow, configuration, gotchas, related files). **33 files.**

## `src/TutoringCentre.Application`

| File | Purpose | Explanation |
| --- | --- | --- |
| `AssemblyMarker.cs` | Assembly handle used by handler scanning and by architecture tests. | [Explanation](./src/TutoringCentre.Application/AssemblyMarker.cs.md) |
| `DependencyInjection.cs` | The Application layer's registration entry point: `AddApplication`, called once from `Program.cs`. | [Explanation](./src/TutoringCentre.Application/DependencyInjection.cs.md) |
| `TutoringCentre.Application.csproj` | Project file for the use-case layer: references Domain only, plus validation and DI/logging *abstractions*. | [Explanation](./src/TutoringCentre.Application/TutoringCentre.Application.csproj.md) |

## `src/TutoringCentre.Application/Centres`

| File | Purpose | Explanation |
| --- | --- | --- |
| `ICentreRepository.cs` | Write-side persistence port for the Centre aggregate. | [Explanation](./src/TutoringCentre.Application/Centres/ICentreRepository.cs.md) |

## `src/TutoringCentre.Application/Centres/Commands/CreateCentre`

| File | Purpose | Explanation |
| --- | --- | --- |
| `CreateCentreCommand.cs` | The message representing the intent to create a centre. | [Explanation](./src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreCommand.cs.md) |
| `CreateCentreHandler.cs` | The use case 'create a centre': authorise, check uniqueness, let the domain validate and build the entity, then track it. | [Explanation](./src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs.md) |
| `CreateCentreResult.cs` | The success payload of `CreateCentreCommand`: the new centre's id and slug. | [Explanation](./src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreResult.cs.md) |
| `CreateCentreValidator.cs` | Shape validation of `CreateCentreCommand` (required fields, maximum lengths, defined enum). | [Explanation](./src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreValidator.cs.md) |

## `src/TutoringCentre.Application/Common/Cqrs`

| File | Purpose | Explanation |
| --- | --- | --- |
| `Dispatcher.cs` | The single entry point for every use case: validates, opens the right kind of transaction, runs the handler, saves once, commits or rolls back, and logs one outcome line. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs.md) |
| `HandlerRegistration.cs` | Reflection scan that registers every command/query handler and validator in the Application assembly. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/HandlerRegistration.cs.md) |
| `ICommand.cs` | Marker interface for commands, carrying the response type as a generic parameter. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/ICommand.cs.md) |
| `ICommandHandler.cs` | Contract for a class that executes one command and returns a `Result`. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/ICommandHandler.cs.md) |
| `IQuery.cs` | Marker interface for read-only requests. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/IQuery.cs.md) |
| `IQueryHandler.cs` | Contract for a class that answers one query inside a read-only transaction. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/IQueryHandler.cs.md) |
| `Unit.cs` | Placeholder 'no data' response type for commands that only succeed or fail. | [Explanation](./src/TutoringCentre.Application/Common/Cqrs/Unit.cs.md) |

## `src/TutoringCentre.Application/Common/Ports`

| File | Purpose | Explanation |
| --- | --- | --- |
| `IClock.cs` | Port for the current time and for converting between UTC instants and local wall-clock time in an IANA time zone. | [Explanation](./src/TutoringCentre.Application/Common/Ports/IClock.cs.md) |
| `IUnitOfWork.cs` | Port over the persistence transaction: begin (read-only or read-write), save, commit, rollback. | [Explanation](./src/TutoringCentre.Application/Common/Ports/IUnitOfWork.cs.md) |

## `src/TutoringCentre.Application/Common/Security`

| File | Purpose | Explanation |
| --- | --- | --- |
| `Actor.cs` | Defines who is executing a use case: the abstract `Actor` and its kinds `SystemActor`, `AnonymousActor` and `StaffActor`. | [Explanation](./src/TutoringCentre.Application/Common/Security/Actor.cs.md) |
| `CurrentActorContext.cs` | Holds the actor for one scope; starts anonymous, may be set exactly once, and may be replaced only through `Reauthenticate` at login. | [Explanation](./src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs.md) |
| `IAuthenticationService.cs` | Application's port for verifying staff credentials, plus the `AuthenticatedUser` result record. | [Explanation](./src/TutoringCentre.Application/Common/Security/IAuthenticationService.cs.md) |
| `ICurrentActor.cs` | Read-only view of the actor of the current scope. | [Explanation](./src/TutoringCentre.Application/Common/Security/ICurrentActor.cs.md) |

## `src/TutoringCentre.Application/Identity`

| File | Purpose | Explanation |
| --- | --- | --- |
| `IMembershipReadService.cs` | Read-side port for a staff member's own profile, active memberships and session state, with its DTO records. | [Explanation](./src/TutoringCentre.Application/Identity/IMembershipReadService.cs.md) |

## `src/TutoringCentre.Application/Identity/Queries/GetActiveMembership`

| File | Purpose | Explanation |
| --- | --- | --- |
| `GetActiveMembershipHandler.cs` | The tenant gate: the only way a centre id enters a session. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipHandler.cs.md) |
| `GetActiveMembershipQuery.cs` | The tenant-gate question: may the signed-in staff member act in this centre?. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/GetActiveMembership/GetActiveMembershipQuery.cs.md) |

## `src/TutoringCentre.Application/Identity/Queries/GetMyMemberships`

| File | Purpose | Explanation |
| --- | --- | --- |
| `GetMyMembershipsHandler.cs` | Builds the signed-in user's `MeDto` from a freshly read profile, dropping the actor's selected centre if it is no longer an active membership. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsHandler.cs.md) |
| `GetMyMembershipsQuery.cs` | Asks for the signed-in staff member's own profile and active memberships. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/GetMyMembershipsQuery.cs.md) |
| `MeDto.cs` | The response shape of `GET /api/me` and login: profile, active centre/role and active memberships. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/GetMyMemberships/MeDto.cs.md) |

## `src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession`

| File | Purpose | Explanation |
| --- | --- | --- |
| `ValidateStaffSessionHandler.cs` | Decides if a session's security stamp and (when a centre is selected) membership are still current. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionHandler.cs.md) |
| `ValidateStaffSessionQuery.cs` | Asks whether an existing session is still valid; internal to the authentication pipeline. | [Explanation](./src/TutoringCentre.Application/Identity/Queries/ValidateStaffSession/ValidateStaffSessionQuery.cs.md) |

## `src/TutoringCentre.Application/Platform`

| File | Purpose | Explanation |
| --- | --- | --- |
| `ISystemInfoReadService.cs` | Read-side port that returns database schema status as plain data, plus the `SchemaStatus` record. | [Explanation](./src/TutoringCentre.Application/Platform/ISystemInfoReadService.cs.md) |
| `SystemInfoDto.cs` | The public response shape of `GET /api/system/info`: application version, latest migration, up-to-date flag. | [Explanation](./src/TutoringCentre.Application/Platform/SystemInfoDto.cs.md) |

## `src/TutoringCentre.Application/Platform/Queries/GetSystemInfo`

| File | Purpose | Explanation |
| --- | --- | --- |
| `GetSystemInfoHandler.cs` | Builds `SystemInfoDto` from the read service and from the assembly's informational version. | [Explanation](./src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoHandler.cs.md) |
| `GetSystemInfoQuery.cs` | The parameterless query asking for application version and migration status. | [Explanation](./src/TutoringCentre.Application/Platform/Queries/GetSystemInfo/GetSystemInfoQuery.cs.md) |
