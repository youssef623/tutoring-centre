# `tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs`

## Purpose

Declares `RegisterStudentValidatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 78 lines and 2,856 bytes. Source hash: `ec2285fbe804aeb5fd8259cef79a97a3ee9333ff69586ab2b98b084f4b2e9bdb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;`
- Line 2: `using TutoringCentre.Domain.Academics;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs#L6): `public sealed class RegisterStudentValidatorTests`
- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs#L9): `public void Validate_ExistingParentIdAlone_HasNoErrors()`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs#L17): `public void Validate_NewParentNameAndPhoneTogether_HasNoErrors()`
- [Line 25](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs#L25): `public void Validate_NeitherFormGiven_ReportsErrorOnParent()`
- [Line 34](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs#L34): `public void Validate_BothFormsGivenAtOnce_ReportsErrorOnParent()`
- [Line 43](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs#L43): `public void Validate_NewParentNameOnlyMissingPhone_ReportsErrorOnParentAndParentPhone()`
- [Line 53](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs#L53): `public void Validate_EmptyStudentName_ReportsErrorOnFullName()`
- [Line 64](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentValidatorTests.cs#L64): `public void Validate_UndefinedRelationship_ReportsErrorOnRelationship()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RegisterStudentValidatorTests
{
    [Fact]
    public void Validate_ExistingParentIdAlone_HasNoErrors()
    {
        var result = Validate(new RegisterStudentParent(Guid.CreateVersion7(), null, null));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_NewParentNameAndPhoneTogether_HasNoErrors()
    {
        var result = Validate(new RegisterStudentParent(null, "Omar Hassan", "01012345678"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_NeitherFormGiven_ReportsErrorOnParent()
    {
        var result = Validate(new RegisterStudentParent(null, null, null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Parent");
    }

    [Fact]
    public void Validate_BothFormsGivenAtOnce_ReportsErrorOnParent()
    {
        var result = Validate(new RegisterStudentParent(Guid.CreateVersion7(), "Omar Hassan", "01012345678"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Parent");
    }

    [Fact]
    public void Validate_NewParentNameOnlyMissingPhone_ReportsErrorOnParentAndParentPhone()
    {
        var result = Validate(new RegisterStudentParent(null, "Omar Hassan", null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Parent");
        Assert.Contains(result.Errors, error => error.PropertyName == "Parent.Phone");
    }

    [Fact]
    public void Validate_EmptyStudentName_ReportsErrorOnFullName()
    {
        var command = new RegisterStudentCommand("", null, null, new RegisterStudentParent(Guid.CreateVersion7(), null, null), ParentRelationship.Father);

        var result = new RegisterStudentValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_UndefinedRelationship_ReportsErrorOnRelationship()
    {
        var command = new RegisterStudentCommand(
            "Youssef Omar", null, null, new RegisterStudentParent(Guid.CreateVersion7(), null, null), (ParentRelationship)99);

        var result = new RegisterStudentValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Relationship");
    }

    private static FluentValidation.Results.ValidationResult Validate(RegisterStudentParent parent) =>
        new RegisterStudentValidator().Validate(
            new RegisterStudentCommand("Youssef Omar", null, null, parent, ParentRelationship.Father));
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
