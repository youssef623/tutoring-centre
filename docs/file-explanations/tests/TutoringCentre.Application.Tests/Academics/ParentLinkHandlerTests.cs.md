# `tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs`

## Purpose

Declares `ParentLinkHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 136 lines and 6,278 bytes. Source hash: `9180a6333fa86d024893296acc0043a7812608be61508fba9dd1b8398d1aace4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Students.Commands.LinkParent;`
- Line 2: `using TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;`
- Line 3: `using TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;`
- Line 4: `using TutoringCentre.Application.Tests.Fakes;`
- Line 5: `using TutoringCentre.Domain.Academics;`
- Line 6: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L10): `public sealed class ParentLinkHandlerTests`
- [Line 15](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L15): `public async Task LinkParent_ExistingParentNotYetLinked_Succeeds()`
- [Line 32](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L32): `public async Task LinkParent_UnknownParentId_ReturnsParentNotFound()`
- [Line 46](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L46): `public async Task LinkParent_AlreadyLinked_ReturnsAlreadyLinked()`
- [Line 63](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L63): `public async Task UnlinkParent_NonPrimarySecondParent_Succeeds()`
- [Line 79](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L79): `public async Task UnlinkParent_LastParent_ReturnsLastParentRule()`
- [Line 94](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L94): `public async Task UnlinkParent_NotLinked_ReturnsNotLinked()`
- [Line 108](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L108): `public async Task SetPrimaryParent_AlreadyLinkedSecondParent_BecomesPrimary()`
- [Line 124](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L124): `public async Task SetPrimaryParent_NotLinked_ReturnsNotLinked()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Students.Commands.LinkParent;
using TutoringCentre.Application.Academics.Students.Commands.SetPrimaryParent;
using TutoringCentre.Application.Academics.Students.Commands.UnlinkParent;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ParentLinkHandlerTests
{
    private static readonly Guid NileCentreId = Guid.CreateVersion7();

    [Fact]
    public async Task LinkParent_ExistingParentNotYetLinked_Succeeds()
    {
        var students = new FakeStudentRepository();
        var parents = new FakeParentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var parent = Parent.Create(NileCentreId, "Amal Fathy", PhoneNumber.Create("01212345678").Value).Value;
        parents.Existing.Add(parent);
        var handler = new LinkParentHandler(students, parents);

        var result = await handler.HandleAsync(new LinkParentCommand(student.Id, parent.Id, ParentRelationship.Mother, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, student.Parents.Count);
    }

    [Fact]
    public async Task LinkParent_UnknownParentId_ReturnsParentNotFound()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new LinkParentHandler(students, new FakeParentRepository());

        var result = await handler.HandleAsync(new LinkParentCommand(student.Id, Guid.CreateVersion7(), ParentRelationship.Mother, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("parent.not_found", result.Error!.Code);
    }

    [Fact]
    public async Task LinkParent_AlreadyLinked_ReturnsAlreadyLinked()
    {
        var students = new FakeStudentRepository();
        var parents = new FakeParentRepository();
        var parent = Parent.Create(NileCentreId, "Omar Hassan", PhoneNumber.Create("01012345678").Value).Value;
        parents.Existing.Add(parent);
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, parent.Id, ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new LinkParentHandler(students, parents);

        var result = await handler.HandleAsync(new LinkParentCommand(student.Id, parent.Id, ParentRelationship.Mother, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.parent_already_linked", result.Error!.Code);
    }

    [Fact]
    public async Task UnlinkParent_NonPrimarySecondParent_Succeeds()
    {
        var students = new FakeStudentRepository();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, Guid.CreateVersion7(), ParentRelationship.Father).Value;
        var secondParentId = Guid.CreateVersion7();
        student.AddParent(secondParentId, ParentRelationship.Mother);
        students.Existing.Add(student);
        var handler = new UnlinkParentHandler(students);

        var result = await handler.HandleAsync(new UnlinkParentCommand(student.Id, secondParentId, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(student.Parents);
    }

    [Fact]
    public async Task UnlinkParent_LastParent_ReturnsLastParentRule()
    {
        var students = new FakeStudentRepository();
        var primaryParentId = Guid.CreateVersion7();
        var student = Student.Register(NileCentreId, "Youssef Omar", null, null, primaryParentId, ParentRelationship.Father).Value;
        students.Existing.Add(student);
        var handler = new UnlinkParentHandler(students);

        var result = await handler.HandleAsync(new UnlinkParentCommand(student.Id, primaryParentId, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("student.last_parent", result.Error!.Code);
```

The excerpt stops at line 90; use the original file for the remaining 46 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
