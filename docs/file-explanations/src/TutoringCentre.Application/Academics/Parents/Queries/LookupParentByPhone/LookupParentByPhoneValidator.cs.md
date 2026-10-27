# `src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneValidator.cs`

## Purpose

Source summary (intent, not independent execution proof): Shape checks only. The phone's real validity lives in PhoneNumber.Create.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneValidator.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 492 bytes. Source hash: `573c119ddc3901becf24cd42a2edf49fef67fd197f981ab8b56cfa39dc84337e`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using FluentValidation;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneValidator.cs#L6): `internal sealed class LookupParentByPhoneValidator : AbstractValidator<LookupParentByPhoneQuery>`
- [Line 10](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneValidator.cs#L10): `public LookupParentByPhoneValidator()`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:171](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L171)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using FluentValidation;

namespace TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;

/// <summary>Shape checks only. The phone's real validity lives in PhoneNumber.Create.</summary>
internal sealed class LookupParentByPhoneValidator : AbstractValidator<LookupParentByPhoneQuery>
{
    private const int PhoneMaxRawLength = 30;

    public LookupParentByPhoneValidator()
    {
        RuleFor(query => query.Phone).NotEmpty().MaximumLength(PhoneMaxRawLength);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
