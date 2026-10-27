# `tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs`

## Purpose

Source summary (intent, not independent execution proof): The link is the security-relevant part of Task 43.8, so most cases here target its refusals rather than the happy path alone: an unknown user, a user of another centre, a user with another role and an inactive member must all answer teacher.user_not_eligible identically, and a user already linked elsewhere must answer teacher.account_taken.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 162 lines and 6,653 bytes. Source hash: `2d63ff45a42c331244e22121cc2bbf814aacbd9a0e28bcce1cd17ea77ebb2bf0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L16): `public sealed class SetTeacherAccountHandlerTests`
- [Line 21](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L21): `public async Task HandleAsync_UnknownTeacherId_ReturnsNotFound()`
- [Line 33](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L33): `public async Task HandleAsync_NullUserId_Unlinks()`
- [Line 48](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L48): `public async Task HandleAsync_EligibleUser_Links()`
- [Line 63](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L63): `public async Task HandleAsync_UnknownUserId_ReturnsUserNotEligible()`
- [Line 77](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L77): `public async Task HandleAsync_UserOfAnotherCentre_ReturnsUserNotEligible()`
- [Line 92](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L92): `public async Task HandleAsync_UserWithAnotherRole_ReturnsUserNotEligible()`
- [Line 106](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L106): `public async Task HandleAsync_UserAlreadyLinkedToAnotherTeacher_ReturnsAccountTaken()`
- [Line 128](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L128): `public async Task HandleAsync_RelinkingTheSameTeacherToItsOwnUser_Succeeds()`
- [Line 143](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L143): `private static void AssertUserNotEligible<T>(Result<T> result)`
- [Line 151](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L151): `private static Fixture CreateSut()`
- [Line 161](../../../../../tests/TutoringCentre.Application.Tests/Academics/SetTeacherAccountHandlerTests.cs#L161): `private sealed record Fixture(SetTeacherAccountHandler Handler, FakeTeacherRepository Repository, FakeStaffDirectory StaffDirectory);`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// The link is the security-relevant part of Task 43.8, so most cases here target its refusals rather than
/// the happy path alone: an unknown user, a user of another centre, a user with another role and an inactive
/// member must all answer teacher.user_not_eligible identically, and a user already linked elsewhere must
/// answer teacher.account_taken.
/// </summary>
public sealed class SetTeacherAccountHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownTeacherId_ReturnsNotFound()
    {
        var fixture = CreateSut();

        var result = await fixture.Handler.HandleAsync(
            new SetTeacherAccountCommand(Guid.CreateVersion7(), Guid.CreateVersion7(), 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("teacher.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task HandleAsync_NullUserId_Unlinks()
    {
        var fixture = CreateSut();
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        var userId = Guid.CreateVersion7();
        teacher.LinkUser(userId);
        fixture.Repository.Existing.Add(teacher);

        var result = await fixture.Handler.HandleAsync(new SetTeacherAccountCommand(teacher.Id, null, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(teacher.UserId);
    }

    [Fact]
    public async Task HandleAsync_EligibleUser_Links()
    {
        var fixture = CreateSut();
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        fixture.Repository.Existing.Add(teacher);
        var userId = Guid.CreateVersion7();
        fixture.StaffDirectory.Members.Add((NileCentreId, userId, StaffRole.Teacher));

        var result = await fixture.Handler.HandleAsync(new SetTeacherAccountCommand(teacher.Id, userId, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(userId, teacher.UserId);
    }

    [Fact]
    public async Task HandleAsync_UnknownUserId_ReturnsUserNotEligible()
    {
        var fixture = CreateSut();
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        fixture.Repository.Existing.Add(teacher);

        var result = await fixture.Handler.HandleAsync(
            new SetTeacherAccountCommand(teacher.Id, Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertUserNotEligible(result);
        Assert.Null(teacher.UserId);
    }

    [Fact]
    public async Task HandleAsync_UserOfAnotherCentre_ReturnsUserNotEligible()
    {
        var fixture = CreateSut();
        var teacher = Teacher.Create(NileCentreId, "Amina Hassan", null).Value;
        fixture.Repository.Existing.Add(teacher);
        var otherCentreId = Guid.CreateVersion7();
        var userId = Guid.CreateVersion7();
        fixture.StaffDirectory.Members.Add((otherCentreId, userId, StaffRole.Teacher));

        var result = await fixture.Handler.HandleAsync(new SetTeacherAccountCommand(teacher.Id, userId, 0), CancellationToken.None);

        AssertUserNotEligible(result);
    }

```

The excerpt stops at line 90; use the original file for the remaining 72 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
