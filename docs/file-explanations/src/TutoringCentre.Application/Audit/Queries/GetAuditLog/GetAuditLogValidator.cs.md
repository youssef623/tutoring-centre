# `src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only, per the Day 32 contract. The cursor's value is decoded here to validate it, then decoded again (never trusted) by the read service that actually uses it.

## Where it fits

Category: **source**. [Open current file](../../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs). [Complete file index](../../../../../INDEX.md). [Current teaching overview](../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 37 lines and 1,608 bytes. Source hash: `f52bf9e2302d1f10d01387758363d50f3402e1d3216201da8f1d14fba5a964b4`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`
- Line 2: `using TutoringCentre.Application.Audit;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs#L7): `internal sealed class GetAuditLogValidator : AbstractValidator<GetAuditLogQuery>`
- [Line 16](../../../../../../../src/TutoringCentre.Application/Audit/Queries/GetAuditLog/GetAuditLogValidator.cs#L16): `public GetAuditLogValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs:63](../../../../../../../tests/TutoringCentre.Application.Tests/Audit/AuditDispatcherAuthorizationTests.cs#L63)
- [tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs:8](../../../../../../../tests/TutoringCentre.Application.Tests/Audit/GetAuditLogValidatorTests.cs#L8)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;
using TutoringCentre.Application.Audit;

namespace TutoringCentre.Application.Audit.Queries.GetAuditLog;

/// <summary>Shape checks only, per the Day 32 contract. The cursor's value is decoded here to validate it, then decoded again (never trusted) by the read service that actually uses it.</summary>
internal sealed class GetAuditLogValidator : AbstractValidator<GetAuditLogQuery>
{
    // Every entity type AuditPolicy records (docs/architecture/audit.md), by the name AuditEntityTypeNames writes.
    private static readonly string[] KnownEntityTypes =
    [
        "subject", "membership", "centre", "parent", "student", "student_parent", "teacher", "group", "enrollment", "schedule_rule",
        "class_session",
    ];

    public GetAuditLogValidator()
    {
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);

        RuleFor(query => query.EntityType)
            .Must(entityType => entityType is null || KnownEntityTypes.Contains(entityType))
            .WithMessage("Entity type is not recognised.");

        RuleFor(query => query.EntityType)
            .NotNull()
            .When(query => query.EntityId is not null)
            .WithMessage("entityId requires entityType.");

        RuleFor(query => query.From)
            .Must((query, from) => from is null || query.To is null || from <= query.To)
            .WithMessage("'from' must be at or before 'to'.");

        RuleFor(query => query.Cursor)
            .Must(cursor => cursor is null || AuditCursor.TryDecode(cursor, out _))
            .WithMessage("Cursor is invalid.");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
