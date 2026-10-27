# `src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs`

## Purpose

Source summary (intent, not independent execution proof): Holds the actor for one scope. Starts anonymous; trusted edge code (Api middleware, job runner, seed) calls `Set` exactly once. Handlers depend on `ICurrentActor`, which has no setter.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 1,576 bytes. Source hash: `7ddbe55729480e04e52388b54ec2fc1bac7180926ff397302420925d89bfb3a0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

No import or project-reference declaration was recognized by the structural extractor. This does not prove the file has no consumers or dependencies.

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs#L7): `public sealed class CurrentActorContext : ICurrentActor`
- [Line 15](../../../../../../src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs#L15): `public void Set(Actor actor)`
- [Line 34](../../../../../../src/TutoringCentre.Application/Common/Security/CurrentActorContext.cs#L34): `public void Reauthenticate(StaffActor actor)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Auth/ActorMiddleware.cs:17](../../../../../../src/TutoringCentre.Api/Auth/ActorMiddleware.cs#L17)
- [src/TutoringCentre.Api/Auth/AuthEndpoints.cs:87](../../../../../../src/TutoringCentre.Api/Auth/AuthEndpoints.cs#L87)
- [src/TutoringCentre.Api/Cli/AppDbContextFactory.cs:40](../../../../../../src/TutoringCentre.Api/Cli/AppDbContextFactory.cs#L40)
- [src/TutoringCentre.Api/Cli/SeedCommand.cs:162](../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L162)
- [src/TutoringCentre.Application/DependencyInjection.cs:15](../../../../../../src/TutoringCentre.Application/DependencyInjection.cs#L15)
- [src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs:35](../../../../../../src/TutoringCentre.Infrastructure/Persistence/MigrationRunner.cs#L35)
- [tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs:214](../../../../../../tests/TutoringCentre.Api.Tests/Auth/SecurityTests.cs#L214)
- [tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs:17](../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs:18](../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateSubjectHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs:16](../../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs:136](../../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L136)
- [tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs:153](../../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L153)
- [tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs:15](../../../../../../tests/TutoringCentre.Application.Tests/Centres/CreateCentreHandlerTests.cs#L15)
- [tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs:16](../../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs:16](../../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs:11](../../../../../../tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs#L11)
- [tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs:104](../../../../../../tests/TutoringCentre.Application.Tests/Staff/ChangeStaffRoleHandlerTests.cs#L104)
- [tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs:87](../../../../../../tests/TutoringCentre.Application.Tests/Staff/CreateStaffHandlerTests.cs#L87)
- [tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs:16](../../../../../../tests/TutoringCentre.Application.Tests/Staff/DeactivateStaffHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Staff/ListStaffHandlerTests.cs:15](../../../../../../tests/TutoringCentre.Application.Tests/Staff/ListStaffHandlerTests.cs#L15)
- 9 further name-reference locations are omitted from this compact list.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
namespace TutoringCentre.Application.Common.Security;

/// <summary>
/// Holds the actor for one scope. Starts anonymous; trusted edge code (Api middleware, job runner, seed) calls
/// <see cref="Set"/> exactly once. Handlers depend on <see cref="ICurrentActor"/>, which has no setter.
/// </summary>
public sealed class CurrentActorContext : ICurrentActor
{
    private bool _isSet;

    public Actor Actor { get; private set; } = new AnonymousActor();

    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The actor was already set in this scope.</exception>
    public void Set(Actor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        if (_isSet)
        {
            throw new InvalidOperationException("The actor for this scope has already been set and cannot be replaced.");
        }

        Actor = actor;
        _isSet = true;
    }

    /// <summary>
    /// Replaces the actor for this scope, even if one was already set. Login is the one place allowed to do
    /// this: a request can arrive with an existing session (ActorMiddleware already set the actor from the
    /// inbound cookie) and then authenticate as a different identity, which must immediately supersede it.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is null.</exception>
    public void Reauthenticate(StaffActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        Actor = actor;
        _isSet = true;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
