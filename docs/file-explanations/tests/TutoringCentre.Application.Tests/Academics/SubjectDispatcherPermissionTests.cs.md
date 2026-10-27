# `tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 28.6: through the real dispatcher, Teacher is read-only at the use-case level for all six Subject requests, while Secretary and Owner can do everything. Closes the Day 24 authorization gap.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 224 lines and 9,098 bytes. Source hash: `0eced575ff1845a9d28e862e1a4272c67d781b8cbe993733d7ee806422cb3c3c`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Academics.Groups;`
- Line 5: `using TutoringCentre.Application.Academics.Subjects;`
- Line 6: `using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;`
- Line 7: `using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;`
- Line 8: `using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;`
- Line 9: `using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;`
- Line 10: `using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;`
- Line 11: `using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;`
- Line 12: `using TutoringCentre.Application.Common.Cqrs;`
- Line 13: `using TutoringCentre.Application.Common.Security;`
- Line 14: `using TutoringCentre.Application.Tests.Fakes;`
- Line 15: `using TutoringCentre.Domain.Academics;`
- Line 16: `using TutoringCentre.Domain.Common;`
- Line 17: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L25): `public sealed class SubjectDispatcherPermissionTests`
- [Line 32](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L32): `public async Task SendAsync_CreateSubject_SucceedsForSecretaryAndOwner(StaffRole role)`
- [Line 43](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L43): `public async Task SendAsync_CreateSubject_ReturnsPermissionDeniedForTeacher()`
- [Line 57](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L57): `public async Task SendAsync_RenameSubject_SucceedsForSecretaryAndOwner(StaffRole role)`
- [Line 68](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L68): `public async Task SendAsync_RenameSubject_ReturnsPermissionDeniedForTeacher()`
- [Line 81](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L81): `public async Task SendAsync_ArchiveSubject_SucceedsForSecretaryAndOwner(StaffRole role)`
- [Line 92](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L92): `public async Task SendAsync_ArchiveSubject_ReturnsPermissionDeniedForTeacher()`
- [Line 105](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L105): `public async Task SendAsync_RestoreSubject_SucceedsForSecretaryAndOwner(StaffRole role)`
- [Line 116](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L116): `public async Task SendAsync_RestoreSubject_ReturnsPermissionDeniedForTeacher()`
- [Line 130](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L130): `public async Task QueryAsync_ListSubjects_SucceedsForEveryRole(StaffRole role)`
- [Line 144](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L144): `public async Task QueryAsync_GetSubject_SucceedsForEveryRole(StaffRole role)`
- [Line 154](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L154): `private static void AssertPermissionDenied<T>(Result<T> result, Fixture fixture)`
- [Line 162](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L162): `private static Fixture CreateSut(StaffRole role)`
- [Line 204](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L204): `private sealed class Fixture(`
- [Line 222](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L222): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Academics.Groups;
using TutoringCentre.Application.Academics.Subjects;
using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;
using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;
using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Task 28.6: through the real dispatcher, Teacher is read-only at the use-case level for all six Subject
/// requests, while Secretary and Owner can do everything. Closes the Day 24 authorization gap.
/// </summary>
public sealed class SubjectDispatcherPermissionTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task SendAsync_CreateSubject_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<CreateSubjectCommand, CreateSubjectResult>(
            new CreateSubjectCommand("Biology"), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SendAsync_CreateSubject_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.SendAsync<CreateSubjectCommand, CreateSubjectResult>(
            new CreateSubjectCommand("Biology"), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
        Assert.Empty(fixture.SubjectRepository.Added);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task SendAsync_RenameSubject_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<RenameSubjectCommand, Unit>(
            new RenameSubjectCommand(fixture.ActiveSubject.Id, "Applied Mathematics", 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task SendAsync_RenameSubject_ReturnsPermissionDeniedForTeacher()
    {
        using var fixture = CreateSut(StaffRole.Teacher);

        var result = await fixture.Dispatcher.SendAsync<RenameSubjectCommand, Unit>(
            new RenameSubjectCommand(fixture.ActiveSubject.Id, "Applied Mathematics", 0), CancellationToken.None);

        AssertPermissionDenied(result, fixture);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Owner)]
    public async Task SendAsync_ArchiveSubject_SucceedsForSecretaryAndOwner(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<ArchiveSubjectCommand, Unit>(
            new ArchiveSubjectCommand(fixture.ActiveSubject.Id, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
    }

```

The excerpt stops at line 90; use the original file for the remaining 134 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
