# `tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs`

## Purpose

Declares `GetAuditLogValidatorTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 120 lines and 3,989 bytes. Source hash: `4f40a1eb17520ccbef78f6b5897892de23554fb483547cbcb48a13f63a095034`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Audit;`
- Line 2: `using TutoringCentre.Application.Audit.Queries.GetAuditLog;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L6): `public sealed class GetAuditLogValidatorTests`
- [Line 11](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L11): `public void Validate_WithEveryFilterOmitted_Succeeds()`
- [Line 21](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L21): `public void Validate_WithPageSizeOutOfRange_Fails(int pageSize)`
- [Line 32](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L32): `public void Validate_WithPageSizeAtBoundary_Succeeds(int pageSize)`
- [Line 40](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L40): `public void Validate_WithFromAfterTo_Fails()`
- [Line 50](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L50): `public void Validate_WithFromEqualToTo_Succeeds()`
- [Line 59](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L59): `public void Validate_WithUnknownEntityType_Fails()`
- [Line 79](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L79): `public void Validate_WithKnownEntityType_Succeeds(string entityType)`
- [Line 87](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L87): `public void Validate_WithEntityIdButNoEntityType_Fails()`
- [Line 96](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L96): `public void Validate_WithEntityIdAndEntityType_Succeeds()`
- [Line 104](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L104): `public void Validate_WithGarbageCursor_Fails()`
- [Line 113](../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L113): `public void Validate_WithWellFormedCursor_Succeeds()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Audit.Queries.GetAuditLog;

namespace TutoringCentre.Application.Tests.Audit;

public sealed class GetAuditLogValidatorTests
{
    private readonly GetAuditLogValidator _validator = new();

    [Fact]
    public void Validate_WithEveryFilterOmitted_Succeeds()
    {
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, null, null, null, 50));

        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(101)]
    public void Validate_WithPageSizeOutOfRange_Fails(int pageSize)
    {
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, null, null, null, pageSize));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(GetAuditLogQuery.PageSize));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    public void Validate_WithPageSizeAtBoundary_Succeeds(int pageSize)
    {
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, null, null, null, pageSize));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithFromAfterTo_Fails()
    {
        var now = DateTimeOffset.UtcNow;
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, now, now.AddDays(-1), null, 50));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(GetAuditLogQuery.From));
    }

    [Fact]
    public void Validate_WithFromEqualToTo_Succeeds()
    {
        var now = DateTimeOffset.UtcNow;
        var result = _validator.Validate(new GetAuditLogQuery(null, null, null, now, now, null, 50));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithUnknownEntityType_Fails()
    {
        var result = _validator.Validate(new GetAuditLogQuery("not-a-real-type", null, null, null, null, null, 50));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, failure => failure.PropertyName == nameof(GetAuditLogQuery.EntityType));
    }

    [Theory]
    [InlineData("subject")]
    [InlineData("membership")]
    [InlineData("centre")]
    [InlineData("parent")]
    [InlineData("student")]
    [InlineData("student_parent")]
    [InlineData("teacher")]
    [InlineData("group")]
    [InlineData("enrollment")]
    [InlineData("schedule_rule")]
    [InlineData("class_session")]
    public void Validate_WithKnownEntityType_Succeeds(string entityType)
    {
        var result = _validator.Validate(new GetAuditLogQuery(entityType, null, null, null, null, null, 50));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_WithEntityIdButNoEntityType_Fails()
    {
        var result = _validator.Validate(new GetAuditLogQuery(null, Guid.CreateVersion7(), null, null, null, null, 50));

```

The excerpt stops at line 90; use the original file for the remaining 30 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
