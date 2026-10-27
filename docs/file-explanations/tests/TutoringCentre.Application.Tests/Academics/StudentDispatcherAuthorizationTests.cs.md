# `tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Through the real dispatcher: Teacher is refused students.manage/students.view for all nine student requests, and a centre-less actor is refused all nine before any handler runs — layer 1 (tenant scope) and the permission step, both proven before any transaction, mirroring Parent's Day 39 proofs.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 215 lines and 10,637 bytes. Source hash: `8c0006a4642c086285daa9a784004354fb746a5cc2843b27968bbdd945e1f446`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Academics.Parents;`
- Line 5: `using TutoringCentre.Application.Academics.Students;`
- Line 6: `using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;`
- Line 7: `using TutoringCentre.Application.Academics.Students.Commands.LinkParent;`
- Line 8: `using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;`
- Line 9: `using TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;`
- Line 10: `using TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;`
- Line 11: `using TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;`
- Line 12: `using TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;`
- Line 13: `using TutoringCentre.Application.Academics.Students.Queries.GetStudent;`
- Line 14: `using TutoringCentre.Application.Academics.Students.Queries.ListStudents;`
- Line 15: `using TutoringCentre.Application.Common.Cqrs;`
- Line 16: `using TutoringCentre.Application.Common.Security;`
- Line 17: `using TutoringCentre.Application.Tests.Fakes;`
- Line 18: `using TutoringCentre.Domain.Academics;`
- Line 19: `using TutoringCentre.Domain.Common;`
- Line 20: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 29](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L29): `public sealed class StudentDispatcherAuthorizationTests`
- [Line 36](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L36): `public async Task EveryManageCommand_ReturnsPermissionDeniedForTeacher(StaffRole role)`
- [Line 64](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L64): `public async Task QueryAsync_GetStudent_ReturnsPermissionDeniedForTeacher()`
- [Line 75](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L75): `public async Task QueryAsync_ListStudents_ReturnsPermissionDeniedForTeacher()`
- [Line 88](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L88): `public async Task SendAsync_RegisterStudent_SucceedsForSecretaryAndOwner(StaffRole role)`
- [Line 100](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L100): `public async Task SendAsync_RegisterStudent_ReturnsTenantNotSelectedForCentrelessActor()`
- [Line 113](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L113): `public async Task QueryAsync_GetStudent_ReturnsTenantNotSelectedForCentrelessActor()`
- [Line 123](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L123): `private static async Task AssertPermissionDenied<T>(Task<Result<T>> resultTask, Fixture fixture)`
- [Line 129](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L129): `private static void AssertPermissionDenied<T>(Result<T> result, Fixture fixture)`
- [Line 137](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L137): `private static void AssertTenantNotSelected<T>(Result<T> result, Fixture fixture)`
- [Line 145](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L145): `private static Fixture CreateSut(StaffRole role) => CreateSut(new StaffActor(Guid.NewGuid(), CentreId, role));`
- [Line 147](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L147): `private static Fixture CreateSut(StaffActor actor)`
- [Line 195](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L195): `private sealed class Fixture(`
- [Line 213](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L213): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Students;
using TutoringCentre.Application.Academics.Students.Commands.ArchiveStudent;
using TutoringCentre.Application.Academics.Students.Commands.LinkParent;
using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;
using TutoringCentre.Application.Academics.Students.Commands.RestoreStudent;
using TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;
using TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;
using TutoringCentre.Application.Academics.Students.Commands.UpdateStudent;
using TutoringCentre.Application.Academics.Students.Queries.GetStudent;
using TutoringCentre.Application.Academics.Students.Queries.ListStudents;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: Teacher is refused students.manage/students.view for all nine student
/// requests, and a centre-less actor is refused all nine before any handler runs — layer 1 (tenant scope) and
/// the permission step, both proven before any transaction, mirroring Parent's Day 39 proofs.
/// </summary>
public sealed class StudentDispatcherAuthorizationTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();
    private static readonly StaffActor CentrelessActor = new(Guid.NewGuid(), CentreId: null, Role: null);

    [Theory]
    [InlineData(StaffRole.Teacher)]
    public async Task EveryManageCommand_ReturnsPermissionDeniedForTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<RegisterStudentCommand, RegisterStudentResult>(
            new RegisterStudentCommand("Youssef Omar", null, null, new RegisterStudentParent(fixture.ExistingParent.Id, null, null), ParentRelationship.Father),
            CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<UpdateStudentCommand, Unit>(
            new UpdateStudentCommand(fixture.ExistingStudent.Id, "Youssef Omar", null, null, 0), CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<ArchiveStudentCommand, Unit>(
            new ArchiveStudentCommand(fixture.ExistingStudent.Id, 0), CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<RestoreStudentCommand, Unit>(
            new RestoreStudentCommand(fixture.ExistingStudent.Id, 0), CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<LinkParentCommand, Unit>(
            new LinkParentCommand(fixture.ExistingStudent.Id, fixture.ExistingParent.Id, ParentRelationship.Mother, 0), CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<UnlinkParentCommand, Unit>(
            new UnlinkParentCommand(fixture.ExistingStudent.Id, fixture.ExistingParent.Id, 0), CancellationToken.None), fixture);

        await AssertPermissionDenied(fixture.Dispatcher.SendAsync<SetPrimaryParentCommand, Unit>(
            new SetPrimaryParentCommand(fixture.ExistingStudent.Id, fixture.ExistingParent.Id, 0), CancellationToken.None), fixture);
    }

    [Fact]
    public async Task QueryAsync_GetStudent_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.QueryAsync<GetStudentQuery, StudentDetailDto>(
            new GetStudentQuery(fixture.ExistingStudent.Id), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_ListStudents_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.QueryAsync<ListStudentsQuery, StudentPageDto>(
            new ListStudentsQuery(null, false, null, 25), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task SendAsync_RegisterStudent_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);
```

The excerpt stops at line 90; use the original file for the remaining 125 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
