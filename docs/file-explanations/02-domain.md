# `src/TutoringCentre.Domain`

Folder map generated from the per-file explanations. Part of the [file index](INDEX.md); the teaching overview is [`../PROJECT_OVERVIEW2.md`](../PROJECT_OVERVIEW2.md). Each row links to the full explanation of that file (purpose, where it fits, walkthrough, concepts, flow, configuration, gotchas, related files). **12 files.**

## `src/TutoringCentre.Domain`

| File | Purpose | Explanation |
| --- | --- | --- |
| `AssemblyMarker.cs` | An empty internal class used only so tests can obtain this assembly with `typeof(AssemblyMarker).Assembly`. | [Explanation](./src/TutoringCentre.Domain/AssemblyMarker.cs.md) |
| `TutoringCentre.Domain.csproj` | Project file for the innermost layer. | [Explanation](./src/TutoringCentre.Domain/TutoringCentre.Domain.csproj.md) |

## `src/TutoringCentre.Domain/Centres`

| File | Purpose | Explanation |
| --- | --- | --- |
| `Centre.cs` | The only entity in the system: a tutoring centre (the tenant). | [Explanation](./src/TutoringCentre.Domain/Centres/Centre.cs.md) |
| `SupportedLocale.cs` | Enum of languages a centre can use by default: `Ar` and `En`. | [Explanation](./src/TutoringCentre.Domain/Centres/SupportedLocale.cs.md) |

## `src/TutoringCentre.Domain/Common`

| File | Purpose | Explanation |
| --- | --- | --- |
| `Entity.cs` | Base class for domain entities: gives each one a UUIDv7 identity at construction. | [Explanation](./src/TutoringCentre.Domain/Common/Entity.cs.md) |
| `Error.cs` | Immutable description of an expected business failure: a stable code, a developer message, a kind and optional per-field messages. | [Explanation](./src/TutoringCentre.Domain/Common/Error.cs.md) |
| `ErrorKind.cs` | Closed enum of failure categories; each maps to exactly one HTTP status. | [Explanation](./src/TutoringCentre.Domain/Common/ErrorKind.cs.md) |
| `ITenantOwned.cs` | Marker interface for future entities that belong to one centre (tenant). | [Explanation](./src/TutoringCentre.Domain/Common/ITenantOwned.cs.md) |
| `Result.cs` | Defines `Result` and `Result<T>`, the return types for operations that can fail for expected business reasons. | [Explanation](./src/TutoringCentre.Domain/Common/Result.cs.md) |

## `src/TutoringCentre.Domain/Identity`

| File | Purpose | Explanation |
| --- | --- | --- |
| `Membership.cs` | The entity linking one user to one centre with a role and an active/inactive status. | [Explanation](./src/TutoringCentre.Domain/Identity/Membership.cs.md) |
| `MembershipStatus.cs` | Enum: `Active` or `Inactive` - whether a membership currently grants access. | [Explanation](./src/TutoringCentre.Domain/Identity/MembershipStatus.cs.md) |
| `StaffRole.cs` | Enum of the three staff roles within a centre: Owner, Teacher, Secretary. | [Explanation](./src/TutoringCentre.Domain/Identity/StaffRole.cs.md) |
