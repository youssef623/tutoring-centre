# `tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs`

## Purpose

Source summary (intent, not independent execution proof): Task 24.9: the layer 1 proof for this feature — through the real dispatcher, a centre-less staff actor is refused for every one of the six Subject requests, and the handler (and therefore the fake ports behind it) is never invoked.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 143 lines and 6,052 bytes. Source hash: `4d3def72261623f7aedccff4f428c4f0ec3beb2359f11a1861c0b44682b9e719`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 4: `using TutoringCentre.Application.Academics.Subjects;`
- Line 5: `using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;`
- Line 6: `using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;`
- Line 7: `using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;`
- Line 8: `using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;`
- Line 9: `using TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;`
- Line 10: `using TutoringCentre.Application.Academics.Subjects.Queries.ListSubjects;`
- Line 11: `using TutoringCentre.Application.Common.Cqrs;`
- Line 12: `using TutoringCentre.Application.Common.Security;`
- Line 13: `using TutoringCentre.Application.Tests.Fakes;`
- Line 14: `using TutoringCentre.Domain.Common;`
- Line 15: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 24](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L24): `public sealed class SubjectDispatcherTenantScopeTests`
- [Line 29](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L29): `public async Task SendAsync_CreateSubject_ReturnsTenantNotSelected()`
- [Line 41](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L41): `public async Task SendAsync_RenameSubject_ReturnsTenantNotSelected()`
- [Line 52](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L52): `public async Task SendAsync_ArchiveSubject_ReturnsTenantNotSelected()`
- [Line 63](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L63): `public async Task SendAsync_RestoreSubject_ReturnsTenantNotSelected()`
- [Line 74](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L74): `public async Task QueryAsync_ListSubjects_ReturnsTenantNotSelected()`
- [Line 85](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L85): `public async Task QueryAsync_GetSubject_ReturnsTenantNotSelected()`
- [Line 95](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L95): `private static void AssertTenantNotSelected<T>(Result<T> result, Fixture fixture)`
- [Line 103](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L103): `private static Fixture CreateSut()`
- [Line 131](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L131): `private sealed class Fixture(`
- [Line 141](../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L141): `public void Dispose() => provider.Dispose();`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
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
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

/// <summary>
/// Task 24.9: the layer 1 proof for this feature — through the real dispatcher, a centre-less staff actor is
/// refused for every one of the six Subject requests, and the handler (and therefore the fake ports behind it)
/// is never invoked.
/// </summary>
public sealed class SubjectDispatcherTenantScopeTests
{
    private static readonly StaffActor CentrelessActor = new(Guid.NewGuid(), CentreId: null, Role: null);

    [Fact]
    public async Task SendAsync_CreateSubject_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.SendAsync<CreateSubjectCommand, CreateSubjectResult>(
            new CreateSubjectCommand("Mathematics"), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
        Assert.Empty(fixture.SubjectRepository.Added);
    }

    [Fact]
    public async Task SendAsync_RenameSubject_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.SendAsync<RenameSubjectCommand, Unit>(
            new RenameSubjectCommand(Guid.CreateVersion7(), "Applied Mathematics", 0), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    [Fact]
    public async Task SendAsync_ArchiveSubject_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.SendAsync<ArchiveSubjectCommand, Unit>(
            new ArchiveSubjectCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    [Fact]
    public async Task SendAsync_RestoreSubject_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.SendAsync<RestoreSubjectCommand, Unit>(
            new RestoreSubjectCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_ListSubjects_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.QueryAsync<ListSubjectsQuery, IReadOnlyList<SubjectDto>>(
            new ListSubjectsQuery(false), CancellationToken.None);

        AssertTenantNotSelected(result, fixture);
    }

    [Fact]
    public async Task QueryAsync_GetSubject_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.QueryAsync<GetSubjectQuery, SubjectDto>(
            new GetSubjectQuery(Guid.CreateVersion7()), CancellationToken.None);
```

The excerpt stops at line 90; use the original file for the remaining 53 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
