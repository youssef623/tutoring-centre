# `tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs`

## Purpose

Declares `CreateGroupHandlerTests`, `Sut`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 156 lines and 5,947 bytes. Source hash: `dfe61c3d7e2626a26d5497b57fc6149fd68700b90f05c6c65c93e6c1afd3b326`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L10): `public sealed class CreateGroupHandlerTests`
- [Line 15](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L15): `public async Task HandleAsync_UnknownSubject_ReturnsSubjectNotFound()`
- [Line 27](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L27): `public async Task HandleAsync_ArchivedSubject_ReturnsSubjectArchived()`
- [Line 39](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L39): `public async Task HandleAsync_UnknownTeacher_ReturnsTeacherNotFound()`
- [Line 51](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L51): `public async Task HandleAsync_ArchivedTeacher_ReturnsTeacherArchived()`
- [Line 63](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L63): `public async Task HandleAsync_ArchivedSubjectAndUnknownTeacher_ReportsTheSubjectFirst()`
- [Line 76](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L76): `public async Task HandleAsync_CapacityOutOfRange_ReturnsCapacityInvalid(int capacity)`
- [Line 88](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L88): `public async Task HandleAsync_NameAlreadyUsed_ReturnsNameTakenOnNameField()`
- [Line 102](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L102): `public async Task HandleAsync_Valid_AddsGroupInTheActorsCentre()`
- [Line 119](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L119): `private static void AssertFailure<T>(Result<T> result, string code, ErrorKind kind)`
- [Line 126](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L126): `private static Sut CreateSut()`
- [Line 149](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateGroupHandlerTests.cs#L149): `private sealed record Sut(`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Groups.Commands.CreateGroup;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateGroupHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_UnknownSubject_ReturnsSubjectNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", Guid.CreateVersion7(), sut.ActiveTeacher.Id, 20), CancellationToken.None);

        AssertFailure(result, "subject.not_found", ErrorKind.NotFound);
        Assert.Empty(sut.Groups.Added);
    }

    [Fact]
    public async Task HandleAsync_ArchivedSubject_ReturnsSubjectArchived()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", sut.ArchivedSubject.Id, sut.ActiveTeacher.Id, 20), CancellationToken.None);

        AssertFailure(result, "group.subject_archived", ErrorKind.Rule);
        Assert.Empty(sut.Groups.Added);
    }

    [Fact]
    public async Task HandleAsync_UnknownTeacher_ReturnsTeacherNotFound()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", sut.ActiveSubject.Id, Guid.CreateVersion7(), 20), CancellationToken.None);

        AssertFailure(result, "teacher.not_found", ErrorKind.NotFound);
        Assert.Empty(sut.Groups.Added);
    }

    [Fact]
    public async Task HandleAsync_ArchivedTeacher_ReturnsTeacherArchived()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", sut.ActiveSubject.Id, sut.ArchivedTeacher.Id, 20), CancellationToken.None);

        AssertFailure(result, "group.teacher_archived", ErrorKind.Rule);
        Assert.Empty(sut.Groups.Added);
    }

    [Fact]
    public async Task HandleAsync_ArchivedSubjectAndUnknownTeacher_ReportsTheSubjectFirst()
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", sut.ArchivedSubject.Id, Guid.CreateVersion7(), 20), CancellationToken.None);

        AssertFailure(result, "group.subject_archived", ErrorKind.Rule);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(201)]
    public async Task HandleAsync_CapacityOutOfRange_ReturnsCapacityInvalid(int capacity)
    {
        var sut = CreateSut();

        var result = await sut.Handler.HandleAsync(
            new CreateGroupCommand("Grade 10 A", sut.ActiveSubject.Id, sut.ActiveTeacher.Id, capacity), CancellationToken.None);

        AssertFailure(result, "group.capacity_invalid", ErrorKind.Validation);
        Assert.Empty(sut.Groups.Added);
    }

    [Fact]
    public async Task HandleAsync_NameAlreadyUsed_ReturnsNameTakenOnNameField()
    {
        var sut = CreateSut();
```

The excerpt stops at line 90; use the original file for the remaining 66 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
