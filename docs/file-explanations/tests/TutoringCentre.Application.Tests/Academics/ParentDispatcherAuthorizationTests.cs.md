# `tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Through the real dispatcher: Teacher is refused students.manage/students.view for all four parent requests, and a centre-less actor is refused all four before any handler runs — layer 1 (tenant scope) and the permission step, both proven before any transaction, mirroring Subject's Day 24/28 proofs.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 192 lines and 8,217 bytes. Source hash: `8b0bd5b75794d291313b4c42bdfb2ec2bb9c2e131148b7f5fb6478eb850432f2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Academics.Parents;`
- Line 5: `using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;`
- Line 6: `using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;`
- Line 7: `using TutoringCentre.Application.Academics.Parents.Queries.GetParent;`
- Line 8: `using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;`
- Line 9: `using TutoringCentre.Application.Common.Cqrs;`
- Line 10: `using TutoringCentre.Application.Common.Security;`
- Line 11: `using TutoringCentre.Application.Tests.Fakes;`
- Line 12: `using TutoringCentre.Domain.Academics;`
- Line 13: `using TutoringCentre.Domain.Common;`
- Line 14: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 23](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L23): `public sealed class ParentDispatcherAuthorizationTests`
- [Line 29](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L29): `public async Task SendAsync_CreateParent_ReturnsPermissionDeniedForTeacher()`
- [Line 41](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L41): `public async Task SendAsync_UpdateParent_ReturnsPermissionDeniedForTeacher()`
- [Line 52](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L52): `public async Task QueryAsync_GetParent_ReturnsPermissionDeniedForTeacher()`
- [Line 63](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L63): `public async Task QueryAsync_LookupParentByPhone_ReturnsPermissionDeniedForTeacher()`
- [Line 76](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L76): `public async Task SendAsync_CreateParent_SucceedsForSecretaryAndOwner(StaffRole role)`
- [Line 87](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L87): `public async Task SendAsync_CreateParent_ReturnsTenantNotSelectedForCentrelessActor()`
- [Line 99](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L99): `public async Task SendAsync_UpdateParent_ReturnsTenantNotSelectedForCentrelessActor()`
- [Line 110](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L110): `public async Task QueryAsync_GetParent_ReturnsTenantNotSelectedForCentrelessActor()`
- [Line 121](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L121): `public async Task QueryAsync_LookupParentByPhone_ReturnsTenantNotSelectedForCentrelessActor()`
- [Line 131](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L131): `private static void AssertPermissionDenied<T>(Result<T> result, Fixture fixture)`
- [Line 139](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L139): `private static void AssertTenantNotSelected<T>(Result<T> result, Fixture fixture)`
- [Line 147](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L147): `private static Fixture CreateSut(StaffRole role) => CreateSut(new StaffActor(Guid.NewGuid(), CentreId, role));`
- [Line 149](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L149): `private static Fixture CreateSut(StaffActor actor)`
- [Line 178](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L178): `private sealed class Fixture(`
- [Line 190](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L190): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Commands.CreateParent;
using TutoringCentre.Application.Academics.Parents.Commands.UpdateParent;
using TutoringCentre.Application.Academics.Parents.Queries.GetParent;
using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: Teacher is refused students.manage/students.view for all four parent
/// requests, and a centre-less actor is refused all four before any handler runs — layer 1 (tenant scope) and
/// the permission step, both proven before any transaction, mirroring Subject's Day 24/28 proofs.
/// </summary>
public sealed class ParentDispatcherAuthorizationTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();
    private static readonly StaffActor CentrelessActor = new(Guid.NewGuid(), CentreId: null, Role: null);

    [Fact]
    public async Task SendAsync_CreateParent_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.SendAsync<CreateParentCommand, CreateParentResult>(
            new CreateParentCommand("Omar Hassan", "01012345678"), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
        Assert.Empty(fixture.ParentRepository.Added);
    }

    [Fact]
    public async Task SendAsync_UpdateParent_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.SendAsync<UpdateParentCommand, Unit>(
            new UpdateParentCommand(fixture.ExistingParent.Id, "Omar Hassan Ali", "01012345678", 0), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_GetParent_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.QueryAsync<GetParentQuery, ParentDetailDto>(
            new GetParentQuery(fixture.ExistingParent.Id), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_LookupParentByPhone_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.QueryAsync<LookupParentByPhoneQuery, ParentLookupDto>(
            new LookupParentByPhoneQuery("01012345678"), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task SendAsync_CreateParent_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<CreateParentCommand, CreateParentResult>(
            new CreateParentCommand("Amal Fathy", "01212345678"), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SendAsync_CreateParent_ReturnsTenantNotSelectedForCentrelessActor()
    {
        using var fixture = CreateSut(CentrelessActor);

```

The excerpt stops at line 90; use the original file for the remaining 102 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
