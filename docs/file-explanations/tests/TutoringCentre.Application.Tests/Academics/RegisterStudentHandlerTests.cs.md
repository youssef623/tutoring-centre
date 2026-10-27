# `tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs`

## Purpose

Declares `RegisterStudentHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 145 lines and 6,671 bytes. Source hash: `5c62b0d9f9c5703f8bc11f9fe871be66ae2d9ce8d8c233fb2ba979757841efca`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Academics;`
- Line 5: `using TutoringCentre.Domain.Common;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L10): `public sealed class RegisterStudentHandlerTests`
- [Line 15](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L15): `public async Task HandleAsync_ExistingParentForm_AddsStudentLinkedToThatParent()`
- [Line 33](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L33): `public async Task HandleAsync_NewParentForm_AddsBothParentAndStudent()`
- [Line 50](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L50): `public async Task HandleAsync_UnknownExistingParentId_ReturnsParentNotFoundAndAddsNothing()`
- [Line 68](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L68): `public async Task HandleAsync_NewParentPhoneAlreadyTaken_ReturnsParentPhoneTakenWithNestedFieldAndAddsNothing()`
- [Line 86](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L86): `public async Task HandleAsync_NewParentInvalidPhone_ReturnsNestedParentPhoneFieldAndAddsNothing()`
- [Line 102](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L102): `public async Task HandleAsync_StudentNameInvalid_AfterNewParentWasStaged_StudentIsNeverAdded()`
- [Line 120](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L120): `public async Task HandleAsync_StudentOwnPhoneInvalid_ReturnsTopLevelPhoneFieldAndAddsNothing()`
- [Line 136](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L136): `private static (RegisterStudentHandler Handler, FakeStudentRepository Students, FakeParentRepository Parents, CurrentActorContext ActorContext) CreateSut()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RegisterStudentHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task HandleAsync_ExistingParentForm_AddsStudentLinkedToThatParent()
    {
        var (handler, students, parents, _) = CreateSut();
        var existingParent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        parents.Existing.Add(existingParent);

        var result = await handler.HandleAsync(
            new RegisterStudentCommand("Youssef Omar", null, null, new RegisterStudentParent(existingParent.Id, null, null), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(students.Added);
        Assert.Empty(parents.Added);
        Assert.Equal(existingParent.Id, result.Value.ParentId);
        Assert.Equal(students.Added[0].Id, result.Value.StudentId);
    }

    [Fact]
    public async Task HandleAsync_NewParentForm_AddsBothParentAndStudent()
    {
        var (handler, students, parents, _) = CreateSut();

        var result = await handler.HandleAsync(
            new RegisterStudentCommand("Youssef Omar", null, "Grade 10", new RegisterStudentParent(null, "Omar Hassan", "01012345678"), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(parents.Added);
        Assert.Single(students.Added);
        Assert.Equal(NileCentreId, parents.Added[0].CentreId);
        Assert.Equal("+201012345678", parents.Added[0].Phone.Value);
        Assert.Equal(result.Value.ParentId, parents.Added[0].Id);
    }

    [Fact]
    public async Task HandleAsync_UnknownExistingParentId_ReturnsParentNotFoundAndAddsNothing()
    {
        // FindAsync itself is tenant-scoped (IParentRepository's doc comment) — real cross-tenant isolation is
        // proven at the Infrastructure level; here, "not in Existing" stands in for either case, since both
        // reach the handler the same way: a null from FindAsync.
        var (handler, students, parents, _) = CreateSut();

        var result = await handler.HandleAsync(
            new RegisterStudentCommand("Smuggled Student", null, null, new RegisterStudentParent(Guid.CreateVersion7(), null, null), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.not_found", result.Error!.Code);
        Assert.Empty(students.Added);
        Assert.Empty(parents.Added);
    }

    [Fact]
    public async Task HandleAsync_NewParentPhoneAlreadyTaken_ReturnsParentPhoneTakenWithNestedFieldAndAddsNothing()
    {
        var (handler, students, parents, _) = CreateSut();
        parents.Existing.Add(Parent.Create(NileCentreId, "Existing Parent", PhoneNumber.Create("01012345678").Value).Value);

        var result = await handler.HandleAsync(
            new RegisterStudentCommand("Youssef Omar", null, null, new RegisterStudentParent(null, "Omar Hassan", "0101 234 5678"), ParentRelationship.Father),
            CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.phone_taken", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Contains("parent.phone", result.Error.Fields!.Keys);
        Assert.Empty(parents.Added);
        Assert.Empty(students.Added);
    }

    [Fact]
    public async Task HandleAsync_NewParentInvalidPhone_ReturnsNestedParentPhoneFieldAndAddsNothing()
    {
        var (handler, students, parents, _) = CreateSut();

        var result = await handler.HandleAsync(
```

The excerpt stops at line 90; use the original file for the remaining 55 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
