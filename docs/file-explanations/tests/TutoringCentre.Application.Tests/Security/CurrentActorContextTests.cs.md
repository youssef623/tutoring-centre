# `tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs`

## Purpose

Declares `CurrentActorContextTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 64 lines and 2,057 bytes. Source hash: `211b013e90517cee274abbe5d9d046f6126c2b015b700ed51f8be9378bc35829`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs#L6): `public sealed class CurrentActorContextTests`
- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs#L9): `public void Actor_ByDefault_IsAnonymous()`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs#L17): `public void Set_ThenActor_ReturnsTheSetActor()`
- [Line 31](../../../../../tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs#L31): `public void Set_CalledTwice_ThrowsInvalidOperationException()`
- [Line 40](../../../../../tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs#L40): `public void Reauthenticate_AfterSet_ReplacesTheActor()`
- [Line 56](../../../../../tests/TutoringCentre.Application.Tests/Security/CurrentActorContextTests.cs#L56): `public void Reauthenticate_ThenSet_ThrowsInvalidOperationException()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Security;

public sealed class CurrentActorContextTests
{
    [Fact]
    public void Actor_ByDefault_IsAnonymous()
    {
        var context = new CurrentActorContext();

        Assert.IsType<AnonymousActor>(context.Actor);
    }

    [Fact]
    public void Set_ThenActor_ReturnsTheSetActor()
    {
        // Arrange
        var context = new CurrentActorContext();
        var systemActor = new SystemActor(Guid.CreateVersion7());

        // Act
        context.Set(systemActor);

        // Assert
        Assert.Same(systemActor, context.Actor);
    }

    [Fact]
    public void Set_CalledTwice_ThrowsInvalidOperationException()
    {
        var context = new CurrentActorContext();
        context.Set(new SystemActor(null));

        Assert.Throws<InvalidOperationException>(() => context.Set(new SystemActor(Guid.CreateVersion7())));
    }

    [Fact]
    public void Reauthenticate_AfterSet_ReplacesTheActor()
    {
        // Arrange: mirrors a request that arrived with an existing session (ActorMiddleware already set the
        // actor from the inbound cookie) and then logs in as a different identity.
        var context = new CurrentActorContext();
        context.Set(new StaffActor(Guid.CreateVersion7(), Guid.CreateVersion7(), StaffRole.Owner));
        var reauthenticated = new StaffActor(Guid.CreateVersion7(), null, null);

        // Act
        context.Reauthenticate(reauthenticated);

        // Assert
        Assert.Same(reauthenticated, context.Actor);
    }

    [Fact]
    public void Reauthenticate_ThenSet_ThrowsInvalidOperationException()
    {
        // Reauthenticate still leaves the scope "set": nothing after login may further replace the actor.
        var context = new CurrentActorContext();
        context.Reauthenticate(new StaffActor(Guid.CreateVersion7(), null, null));

        Assert.Throws<InvalidOperationException>(() => context.Set(new SystemActor(null)));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
