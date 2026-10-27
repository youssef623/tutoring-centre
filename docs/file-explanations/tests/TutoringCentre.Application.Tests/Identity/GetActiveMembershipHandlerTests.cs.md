# `tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs`

## Purpose

Declares `GetActiveMembershipHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 90 lines and 3,755 bytes. Source hash: `0826411bff8ac2c0938285948d4eebaf53145241cdaf6d183d3c4823b26291f6`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Application.Identity;`
- Line 3: `using TutoringCentre.Application.Identity.Queries.GetActiveMembership;`
- Line 4: `using TutoringCentre.Application.Tests.Fakes;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs#L10): `public sealed class GetActiveMembershipHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs#L13): `public async Task HandleAsync_AnonymousActor_ReturnsNotAuthenticated()`
- [Line 29](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs#L29): `public async Task HandleAsync_NoActiveMembershipInCentre_ReturnsTenantNoMembership()`
- [Line 47](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs#L47): `public async Task HandleAsync_MustChangePasswordPending_ReturnsPasswordChangeRequiredWithoutCheckingMembership()`
- [Line 70](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetActiveMembershipHandlerTests.cs#L70): `public async Task HandleAsync_ActiveMembershipInCentre_Succeeds()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Queries.GetActiveMembership;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Identity;

public sealed class GetActiveMembershipHandlerTests
{
    [Fact]
    public async Task HandleAsync_AnonymousActor_ReturnsNotAuthenticated()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        var handler = new GetActiveMembershipHandler(actorContext, new FakeMembershipReadService());

        // Act
        var result = await handler.HandleAsync(new GetActiveMembershipQuery(Guid.CreateVersion7()), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("auth.not_authenticated", result.Error!.Code);
        Assert.Equal(ErrorKind.Unauthenticated, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_NoActiveMembershipInCentre_ReturnsTenantNoMembership()
    {
        // Arrange — covers unknown centre, non-member and inactive member alike: the read service returns null for all three.
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), null, null));
        var readService = new FakeMembershipReadService { ActiveMembership = null };
        var handler = new GetActiveMembershipHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetActiveMembershipQuery(Guid.CreateVersion7()), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("tenant.no_membership", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_MustChangePasswordPending_ReturnsPasswordChangeRequiredWithoutCheckingMembership()
    {
        // Arrange
        var centreId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), null, null));
        var readService = new FakeMembershipReadService
        {
            MustChangePassword = true,
            ActiveMembership = new ActiveMembershipDto(centreId, "Nile Centre", StaffRole.Owner),
        };
        var handler = new GetActiveMembershipHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetActiveMembershipQuery(centreId), CancellationToken.None);

        // Assert — the gate fires even though an active membership exists: the flag is checked first.
        Assert.True(result.IsFailure);
        Assert.Equal("auth.password_change_required", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_ActiveMembershipInCentre_Succeeds()
    {
        // Arrange
        var centreId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(Guid.CreateVersion7(), null, null));
        var readService = new FakeMembershipReadService
        {
            ActiveMembership = new ActiveMembershipDto(centreId, "Nile Centre", StaffRole.Owner),
        };
        var handler = new GetActiveMembershipHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetActiveMembershipQuery(centreId), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(centreId, result.Value.CentreId);
        Assert.Equal(StaffRole.Owner, result.Value.Role);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
