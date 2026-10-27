# `src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs`

## Purpose

Constructs the canonical phone value object and asks the parent read service for an exact match. Absence is a successful null response rather than NotFound, supporting optional reuse in student registration. The endpoint uses a POST body to keep phone values out of URL paths and query strings.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 22 lines and 1,124 bytes. Source hash: `8226ff93dd8109609339db09cdb289a7b1efe41f588c7895e8004992d2d6e2af`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs#L9): `internal sealed class LookupParentByPhoneHandler(IParentReadService readService) : IQueryHandler<LookupParentByPhoneQuery, ParentLookupDto>`
- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs#L11): `public async Task<Result<ParentLookupDto>> HandleAsync(LookupParentByPhoneQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [docs/onboarding-work/extend_backend_purposes.py:33](../../../../../../../onboarding-work/extend_backend_purposes.py#L33)
- [tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs:16](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:168](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L168)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;

/// <summary>Exact match on the canonical number only — never a 404 for "no such parent"; that answer is data, not an error.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class LookupParentByPhoneHandler(IParentReadService readService) : IQueryHandler<LookupParentByPhoneQuery, ParentLookupDto>
{
    public async Task<Result<ParentLookupDto>> HandleAsync(LookupParentByPhoneQuery query, CancellationToken cancellationToken)
    {
        var phone = PhoneNumber.Create(query.Phone);
        if (phone.IsFailure)
        {
            return Result<ParentLookupDto>.Failure(ParentErrors.PhoneInvalid(phone.Error!));
        }

        var parent = await readService.FindByPhoneAsync(phone.Value, cancellationToken);
        return Result<ParentLookupDto>.Success(new ParentLookupDto(parent));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
