# `tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs`

## Purpose

Declares `PersistenceErrorTranslatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 202 lines and 7,182 bytes. Source hash: `113755574ea3a7c3a6f7a17984863f4e9b9cd5422ebc6cf975efa42af225155a`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.EntityFrameworkCore;`
- Line 2: `using Microsoft.Extensions.Logging.Abstractions;`
- Line 3: `using Npgsql;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Infrastructure.Persistence;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L9): `public sealed class PersistenceErrorTranslatorTests`
- [Line 12](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L12): `public void TryTranslate_UniqueViolationOnRegisteredConstraint_ReturnsMappedConflict()`
- [Line 24](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L24): `public void TryTranslate_UniqueViolationOnParentPhoneConstraint_ReturnsParentPhoneTaken()`
- [Line 36](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L36): `public void TryTranslate_UniqueViolationOnStudentParentConstraint_ReturnsParentAlreadyLinked()`
- [Line 48](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L48): `public void TryTranslate_UniqueViolationOnActiveEnrollmentIndex_ReturnsAlreadyEnrolledConflict()`
- [Line 61](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L61): `public void TryTranslate_UniqueViolationOnOnePrimaryIndex_IsNotTranslated()`
- [Line 71](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L71): `public void TryTranslate_UniqueViolationOnUnregisteredConstraint_ReturnsGenericDuplicate()`
- [Line 83](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L83): `public void TryTranslate_ForeignKeyViolation_IsNotTranslated()`
- [Line 94](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L94): `public void TryTranslate_CheckViolation_IsNotTranslated()`
- [Line 105](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L105): `public void TryTranslate_ConcurrencyException_ReturnsConcurrencyStale()`
- [Line 117](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L117): `public void TryTranslate_UnrelatedException_IsNotTranslated()`
- [Line 127](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L127): `public void TryTranslate_ExclusionViolationOnTeacherOverlap_ReturnsTeacherDoubleBookedConflict()`
- [Line 141](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L141): `public void TryTranslate_ExclusionViolationOnGroupOverlap_ReturnsGroupDoubleBookedConflict()`
- [Line 154](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L154): `public void TryTranslate_ExclusionViolationOnUnlistedConstraint_IsNotTranslated()`
- [Line 164](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L164): `public void TryTranslate_UniqueViolationOnSessionRuleDate_IsNotTranslated()`
- [Line 173](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L173): `private static DbUpdateException ExclusionViolation(string constraintName) =>`
- [Line 176](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L176): `private static DbUpdateException UniqueViolation(string constraintName) =>`
- [Line 179](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L179): `private static DbUpdateException ConstraintViolation(string sqlState, string messageText, string constraintName)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

public sealed class PersistenceErrorTranslatorTests
{
    [Fact]
    public void TryTranslate_UniqueViolationOnRegisteredConstraint_ReturnsMappedConflict()
    {
        var exception = UniqueViolation("ux_centres_slug");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("centre.slug_taken", result.Error!.Code);
    }

    [Fact]
    public void TryTranslate_UniqueViolationOnParentPhoneConstraint_ReturnsParentPhoneTaken()
    {
        var exception = UniqueViolation("ux_parents_centre_phone");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("parent.phone_taken", result.Error!.Code);
    }

    [Fact]
    public void TryTranslate_UniqueViolationOnStudentParentConstraint_ReturnsParentAlreadyLinked()
    {
        var exception = UniqueViolation("ux_student_parents_student_parent");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("student.parent_already_linked", result.Error!.Code);
    }

    [Fact]
    public void TryTranslate_UniqueViolationOnActiveEnrollmentIndex_ReturnsAlreadyEnrolledConflict()
    {
        var exception = UniqueViolation("ux_enrollments_active_student_group");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("enrollment.already_enrolled", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
    }

    [Fact]
    public void TryTranslate_UniqueViolationOnOnePrimaryIndex_IsNotTranslated()
    {
        var exception = UniqueViolation("ux_student_parents_one_primary");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.Null(result);
    }

    [Fact]
    public void TryTranslate_UniqueViolationOnUnregisteredConstraint_ReturnsGenericDuplicate()
    {
        var exception = UniqueViolation("ux_something_nobody_registered");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("conflict.duplicate", result.Error!.Code);
    }

    [Fact]
    public void TryTranslate_ForeignKeyViolation_IsNotTranslated()
    {
        var postgresException = new PostgresException("fk violation", "ERROR", "ERROR", "23503");
        var exception = new DbUpdateException("update failed", postgresException);

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.Null(result);
```

The excerpt stops at line 90; use the original file for the remaining 112 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
