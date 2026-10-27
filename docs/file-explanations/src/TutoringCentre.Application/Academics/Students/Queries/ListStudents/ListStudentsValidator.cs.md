# `src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. The cursor's value is decoded here to validate it, then decoded again (never trusted) by the read service that actually uses it.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 650 bytes. Source hash: `c2ea2e72d9569d23e128825a33e2b1c4b64b4447b572af4dd682e100070d4e3d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsValidator.cs#L6): `internal sealed class ListStudentsValidator : AbstractValidator<ListStudentsQuery>`
- [Line 8](../../../../../../../../src/TutoringCentre.Application/Academics/Students/Queries/ListStudents/ListStudentsValidator.cs#L8): `public ListStudentsValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs:10](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ListStudentsValidatorTests.cs#L10)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:183](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L183)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;

namespace TutoringCentre.Application.Academics.Students.Queries.ListStudents;

/// <summary>Shape checks only. The cursor's value is decoded here to validate it, then decoded again (never trusted) by the read service that actually uses it.</summary>
internal sealed class ListStudentsValidator : AbstractValidator<ListStudentsQuery>
{
    public ListStudentsValidator()
    {
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);

        RuleFor(query => query.Cursor)
            .Must(cursor => cursor is null || StudentCursor.TryDecode(cursor, out _))
            .WithMessage("Cursor is invalid.");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
