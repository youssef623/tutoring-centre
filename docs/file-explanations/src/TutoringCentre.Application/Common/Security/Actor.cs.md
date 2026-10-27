# `src/TutoringCentre.Application/Common/Security/Actor.cs`

## Purpose

Source summary (intent, not independent execution proof): Who is executing a use case and in which centre. Exactly one per request or job scope; handlers read it, they never receive identity or tenant from request data.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Security/Actor.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 34 lines and 1,314 bytes. Source hash: `03a58c2709ad43422817b9f0ea80d4f6d3899dae4b93e8916f08eb2152187512`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../src/TutoringCentre.Application/Common/Security/Actor.cs#L9): `public abstract record Actor`
- [Line 16](../../../../../../src/TutoringCentre.Application/Common/Security/Actor.cs#L16): `public sealed record SystemActor(Guid? CentreId) : Actor`
- [Line 22](../../../../../../src/TutoringCentre.Application/Common/Security/Actor.cs#L22): `public sealed record AnonymousActor : Actor`
- [Line 31](../../../../../../src/TutoringCentre.Application/Common/Security/Actor.cs#L31): `public sealed record StaffActor(Guid UserId, Guid? CentreId, StaffRole? Role) : Actor`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [frontend/src/routes/_authenticated/audit.test.tsx:156](../../../../../../frontend/src/routes/_authenticated/audit.test.tsx#L156)
- [src/TutoringCentre.Api/Auth/ActorMiddleware.cs:29](../../../../../../src/TutoringCentre.Api/Auth/ActorMiddleware.cs#L29)
- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:107](../../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L107)
- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:167](../../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L167)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:162](../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L162)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs:30](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/EnrollStudent/EnrollStudentHandler.cs#L30)
- [src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs:30](../../../../../../src/TutoringCentre.Application/Academics/Enrollments/Commands/TransferEnrollment/TransferEnrollmentHandler.cs#L30)
- [src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs:27](../../../../../../src/TutoringCentre.Application/Academics/Groups/Commands/CreateGroup/CreateGroupHandler.cs#L27)
- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs:17](../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs#L17)
- [src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs:33](../../../../../../src/TutoringCentre.Application/Academics/Scheduling/Commands/AddScheduleRule/AddScheduleRuleHandler.cs#L33)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs:23](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L23)
- [src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs:17](../../../../../../src/TutoringCentre.Application/Academics/Subjects/Commands/CreateSubject/CreateSubjectHandler.cs#L17)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs:17](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/CreateTeacher/CreateTeacherHandler.cs#L17)
- [src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs:37](../../../../../../src/TutoringCentre.Application/Academics/Teachers/Commands/SetTeacherAccount/SetTeacherAccountHandler.cs#L37)
- [src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs:17](../../../../../../src/TutoringCentre.Application/Centres/Commands/CreateCentre/CreateCentreHandler.cs#L17)
- [src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs:20](../../../../../../src/TutoringCentre.Application/Centres/Commands/UpdateCentreSettings/UpdateCentreSettingsHandler.cs#L20)
- [src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs:15](../../../../../../src/TutoringCentre.Application/Centres/Queries/GetCentreSettings/GetCentreSettingsHandler.cs#L15)
- [src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs:159](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L159)
- [src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs:162](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L162)
- [src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs:185](../../../../../../src/TutoringCentre.Application/Common/Cqrs/Dispatcher.cs#L185)
- 151 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Common.Security;

/// <summary>
/// Who is executing a use case and in which centre. Exactly one per request or job scope;
/// handlers read it, they never receive identity or tenant from request data.
/// </summary>
public abstract record Actor
{
    /// <summary>The centre (tenant) the actor acts in; null when no centre applies.</summary>
    public abstract Guid? CentreId { get; }
}

/// <summary>The application itself: background jobs, the seed command, CLI tools. May act in one centre or platform-wide (null).</summary>
public sealed record SystemActor(Guid? CentreId) : Actor
{
    public override Guid? CentreId { get; } = CentreId;
}

/// <summary>No authenticated caller (for example the login request itself). Never has a centre.</summary>
public sealed record AnonymousActor : Actor
{
    public override Guid? CentreId => null;
}

/// <summary>
/// A signed-in staff member. <see cref="CentreId"/> is null right after login, before a centre is selected;
/// <see cref="Role"/> is the actor's role in that centre and is null whenever <see cref="CentreId"/> is.
/// </summary>
public sealed record StaffActor(Guid UserId, Guid? CentreId, StaffRole? Role) : Actor
{
    public override Guid? CentreId { get; } = CentreId;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
