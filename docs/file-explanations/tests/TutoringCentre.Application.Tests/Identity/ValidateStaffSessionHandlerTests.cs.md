# `tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs`

## Purpose

Declares `ValidateStaffSessionHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 103 lines and 3,860 bytes. Source hash: `4534778cc075500bb6961b8d428334b30ea72b61a6f9b3a57cb56990183c7f69`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Identity;`
- Line 2: `using TutoringCentre.Application.Identity.Queries.ValidateStaffSession;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs#L8): `public sealed class ValidateStaffSessionHandlerTests`
- [Line 14](../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs#L14): `public async Task HandleAsync_UnknownUser_ReturnsFalse()`
- [Line 30](../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs#L30): `public async Task HandleAsync_WrongSecurityStamp_ReturnsFalse()`
- [Line 45](../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs#L45): `public async Task HandleAsync_CentreSetAndMembershipInactive_ReturnsFalse()`
- [Line 60](../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs#L60): `public async Task HandleAsync_RoleNoLongerMatchesTheMembership_ReturnsFalse()`
- [Line 75](../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs#L75): `public async Task HandleAsync_MatchingStampAndNoCentre_ReturnsTrue()`
- [Line 90](../../../../../tests/TutoringCentre.Application.Tests/Identity/ValidateStaffSessionHandlerTests.cs#L90): `public async Task HandleAsync_MatchingStampActiveCentreMembershipAndRole_ReturnsTrue()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Identity;
using TutoringCentre.Application.Identity.Queries.ValidateStaffSession;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Identity;

public sealed class ValidateStaffSessionHandlerTests
{
    private static readonly Guid UserId = Guid.CreateVersion7();
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownUser_ReturnsFalse()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = null };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", null, null);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.False(result.Value);
    }

    [Fact]
    public async Task HandleAsync_WrongSecurityStamp_ReturnsFalse()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-current", true, null) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-stale", null, null);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.Value);
    }

    [Fact]
    public async Task HandleAsync_CentreSetAndMembershipInactive_ReturnsFalse()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-1", false, StaffRole.Teacher) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", CentreId, StaffRole.Teacher);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.Value);
    }

    [Fact]
    public async Task HandleAsync_RoleNoLongerMatchesTheMembership_ReturnsFalse()
    {
        // Arrange: the cookie still claims Teacher, but the membership was promoted to Secretary since issue.
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-1", true, StaffRole.Secretary) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", CentreId, StaffRole.Teacher);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.False(result.Value);
    }

    [Fact]
    public async Task HandleAsync_MatchingStampAndNoCentre_ReturnsTrue()
    {
        // Arrange
        var readService = new FakeMembershipReadService { SessionState = new SessionStateDto("stamp-1", true, null) };
        var handler = new ValidateStaffSessionHandler(readService);
        var query = new ValidateStaffSessionQuery(UserId, "stamp-1", null, null);

        // Act
        var result = await handler.HandleAsync(query, CancellationToken.None);

        // Assert
        Assert.True(result.Value);
    }

    [Fact]
    public async Task HandleAsync_MatchingStampActiveCentreMembershipAndRole_ReturnsTrue()
```

The excerpt stops at line 90; use the original file for the remaining 13 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
