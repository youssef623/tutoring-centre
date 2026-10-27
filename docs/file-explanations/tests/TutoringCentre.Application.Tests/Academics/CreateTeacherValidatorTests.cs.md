# `tests/TutoringCentre.Application.Tests/Academics/CreateTeacherValidatorTests.cs`

## Purpose

Declares `CreateTeacherValidatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 41 lines and 1,375 bytes. Source hash: `589d663ad858668f3371f9a631054709cd940d42c70200e8f0c95eb0866816a9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 5](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherValidatorTests.cs#L5): `public sealed class CreateTeacherValidatorTests`
- [Line 8](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherValidatorTests.cs#L8): `public void Validate_EmptyName_ReportsErrorOnFullName()`
- [Line 17](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherValidatorTests.cs#L17): `public void Validate_NameOver120Characters_ReportsErrorOnFullName()`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherValidatorTests.cs#L26): `public void Validate_PhoneOverRawLength_ReportsErrorOnPhone()`
- [Line 35](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateTeacherValidatorTests.cs#L35): `public void Validate_ValidWithNoPhone_HasNoErrors()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateTeacherValidatorTests
{
    [Fact]
    public void Validate_EmptyName_ReportsErrorOnFullName()
    {
        var result = new CreateTeacherValidator().Validate(new CreateTeacherCommand("", null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_NameOver120Characters_ReportsErrorOnFullName()
    {
        var result = new CreateTeacherValidator().Validate(new CreateTeacherCommand(new string('a', 121), null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_PhoneOverRawLength_ReportsErrorOnPhone()
    {
        var result = new CreateTeacherValidator().Validate(new CreateTeacherCommand("Amina Hassan", new string('1', 31)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Phone");
    }

    [Fact]
    public void Validate_ValidWithNoPhone_HasNoErrors()
    {
        var result = new CreateTeacherValidator().Validate(new CreateTeacherCommand("Amina Hassan", null));

        Assert.True(result.IsValid);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
