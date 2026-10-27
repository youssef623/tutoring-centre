# `tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Through the real dispatcher: only Owner holds teachers.manage, so the five commands succeed for Owner and are denied for both Secretary and Teacher. Both Owner and Secretary hold teachers.view, so the two queries succeed for both and are denied only for Teacher.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 275 lines and 11,248 bytes. Source hash: `98674450375b01a7d0d5c2ec466683c1b95da79a3720c7be9cd0c7482be58e59`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Academics.Groups;`
- Line 5: `using TutoringCentre.Application.Academics.Teachers;`
- Line 6: `using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;`
- Line 7: `using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;`
- Line 8: `using TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;`
- Line 9: `using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;`
- Line 10: `using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;`
- Line 11: `using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;`
- Line 12: `using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;`
- Line 13: `using TutoringCentre.Application.Common.Cqrs;`
- Line 14: `using TutoringCentre.Application.Common.Security;`
- Line 15: `using TutoringCentre.Application.Identity.Contracts;`
- Line 16: `using TutoringCentre.Application.Tests.Fakes;`
- Line 17: `using TutoringCentre.Domain.Academics;`
- Line 18: `using TutoringCentre.Domain.Common;`
- Line 19: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 28](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L28): `public sealed class TeacherDispatcherPermissionTests`
- [Line 33](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L33): `public async Task SendAsync_CreateTeacher_SucceedsForOwner()`
- [Line 46](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L46): `public async Task SendAsync_CreateTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 58](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L58): `public async Task SendAsync_UpdateTeacher_SucceedsForOwner()`
- [Line 71](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L71): `public async Task SendAsync_UpdateTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 82](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L82): `public async Task SendAsync_SetTeacherAccount_SucceedsForOwner()`
- [Line 95](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L95): `public async Task SendAsync_SetTeacherAccount_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 106](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L106): `public async Task SendAsync_ArchiveTeacher_SucceedsForOwner()`
- [Line 119](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L119): `public async Task SendAsync_ArchiveTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 130](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L130): `public async Task SendAsync_RestoreTeacher_SucceedsForOwner()`
- [Line 143](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L143): `public async Task SendAsync_RestoreTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)`
- [Line 156](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L156): `public async Task QueryAsync_ListTeachers_SucceedsForSecretaryAndOwner(StaffRole role)`
- [Line 167](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L167): `public async Task QueryAsync_ListTeachers_ReturnsPermissionDeniedForTeacher()`
- [Line 180](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L180): `public async Task QueryAsync_GetTeacher_SucceedsForSecretaryAndOwner(StaffRole role)`
- [Line 191](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L191): `public async Task QueryAsync_GetTeacher_ReturnsPermissionDeniedForTeacher()`
- [Line 201](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L201): `private static void AssertPermissionDenied<T>(Result<T> result, Fixture fixture)`
- [Line 209](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L209): `private static Fixture CreateSut(StaffRole role)`
- [Line 255](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L255): `private sealed class Fixture(`
- [Line 273](../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L273): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Teachers;
using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.RestoreTeacher;
using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;
using TutoringCentre.Application.Academics.Teachers.Commands.UpdateTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;
using TutoringCentre.Application.Academics.Teachers.Queries.ListTeachers;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Through the real dispatcher: only Owner holds teachers.manage, so the five commands succeed for Owner and
/// are denied for both Secretary and Teacher. Both Owner and Secretary hold teachers.view, so the two queries
/// succeed for both and are denied only for Teacher.
/// </summary>
public sealed class TeacherDispatcherPermissionTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Fact]
    public async Task SendAsync_CreateTeacher_SucceedsForOwner()
    {
        using var fixture = CreateSut(StaffRole.Owner);

        var result = await fixture.Dispatcher.SendAsync<CreateTeacherCommand, CreateTeacherResult>(
            new CreateTeacherCommand("Mona Said", null), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_CreateTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<CreateTeacherCommand, CreateTeacherResult>(
            new CreateTeacherCommand("Mona Said", null), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
        Assert.Empty(fixture.TeacherRepository.Added);
    }

    [Fact]
    public async Task SendAsync_UpdateTeacher_SucceedsForOwner()
    {
        using var fixture = CreateSut(StaffRole.Owner);

        var result = await fixture.Dispatcher.SendAsync<UpdateTeacherCommand, Unit>(
            new UpdateTeacherCommand(fixture.ActiveTeacher.Id, "Mona Said", null, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_UpdateTeacher_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<UpdateTeacherCommand, Unit>(
            new UpdateTeacherCommand(fixture.ActiveTeacher.Id, "Mona Said", null, 0), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Fact]
    public async Task SendAsync_SetTeacherAccount_SucceedsForOwner()
    {
        using var fixture = CreateSut(StaffRole.Owner);

        var result = await fixture.Dispatcher.SendAsync<SetTeacherAccountCommand, Unit>(
            new SetTeacherAccountCommand(fixture.ActiveTeacher.Id, null, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }
```

The excerpt stops at line 90; use the original file for the remaining 185 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
