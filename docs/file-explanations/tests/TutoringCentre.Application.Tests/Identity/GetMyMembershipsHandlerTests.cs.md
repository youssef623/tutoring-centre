# `tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs`

## Purpose

Declares `GetMyMembershipsHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 121 lines and 5,162 bytes. Source hash: `af203d0f099a26c5fe51f70d559b09f8eb3c702722d2a356e3ec2596ca52d5a7`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Security;`
- Line 2: `using TutoringCentre.Application.Identity;`
- Line 3: `using TutoringCentre.Application.Identity.Queries.GetMyMemberships;`
- Line 4: `using TutoringCentre.Application.Tests.Fakes;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs#L10): `public sealed class GetMyMembershipsHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs#L13): `public async Task HandleAsync_AnonymousActor_ReturnsNotAuthenticated()`
- [Line 29](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs#L29): `public async Task HandleAsync_NoProfileForActor_ReturnsNotAuthenticated()`
- [Line 47](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs#L47): `public async Task HandleAsync_ActorCentreStillActive_ReportsActiveCentreAndRole()`
- [Line 73](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs#L73): `public async Task HandleAsync_SecretaryWithActiveCentre_ReportsSortedPermissions()`
- [Line 98](../../../../../tests/TutoringCentre.Application.Tests/Identity/GetMyMembershipsHandlerTests.cs#L98): `public async Task HandleAsync_ActorCentreNoLongerActive_DropsActiveCentreAndRole()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Queries.GetMyMemberships;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Identity;

public sealed class GetMyMembershipsHandlerTests
{
    [Fact]
    public async Task HandleAsync_AnonymousActor_ReturnsNotAuthenticated()
    {
        // Arrange
        var actorContext = new CurrentActorContext();
        var handler = new GetMyMembershipsHandler(actorContext, new FakeMembershipReadService());

        // Act
        var result = await handler.HandleAsync(new GetMyMembershipsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("auth.not_authenticated", result.Error!.Code);
        Assert.Equal(ErrorKind.Unauthenticated, result.Error.Kind);
    }

    [Fact]
    public async Task HandleAsync_NoProfileForActor_ReturnsNotAuthenticated()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(userId, null, null));
        var readService = new FakeMembershipReadService { Profile = null };
        var handler = new GetMyMembershipsHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetMyMembershipsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsFailure);
        Assert.Equal("auth.not_authenticated", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_ActorCentreStillActive_ReportsActiveCentreAndRole()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var centreId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(userId, centreId, StaffRole.Teacher));
        var membership = new MembershipDto(centreId, "Nile Centre", "nile-centre", StaffRole.Teacher, "Africa/Cairo");
        var readService = new FakeMembershipReadService
        {
            Profile = new StaffProfileDto(userId, "Teacher", "teacher@both.test", "en", [membership], MustChangePassword: false),
        };
        var handler = new GetMyMembershipsHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetMyMembershipsQuery(), CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(centreId, result.Value.ActiveCentreId);
        Assert.Equal(StaffRole.Teacher, result.Value.ActiveRole);
        Assert.Single(result.Value.Memberships);
        Assert.Equal(["enrollments.view", "groups.view", "schedule.view", "subjects.view"], result.Value.Permissions);
    }

    [Fact]
    public async Task HandleAsync_SecretaryWithActiveCentre_ReportsSortedPermissions()
    {
        // Arrange
        var userId = Guid.CreateVersion7();
        var centreId = Guid.CreateVersion7();
        var actorContext = new CurrentActorContext();
        actorContext.Set(new StaffActor(userId, centreId, StaffRole.Secretary));
        var membership = new MembershipDto(centreId, "Nile Centre", "nile-centre", StaffRole.Secretary, "Africa/Cairo");
        var readService = new FakeMembershipReadService
        {
            Profile = new StaffProfileDto(userId, "Secretary", "secretary@nile.test", "ar", [membership], MustChangePassword: false),
        };
        var handler = new GetMyMembershipsHandler(actorContext, readService);

        // Act
        var result = await handler.HandleAsync(new GetMyMembershipsQuery(), CancellationToken.None);

        // Assert
```

The excerpt stops at line 90; use the original file for the remaining 31 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
