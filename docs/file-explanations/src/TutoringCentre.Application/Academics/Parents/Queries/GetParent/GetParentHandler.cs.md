# `src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs`

## Purpose

Source summary (intent, not independent execution proof): Uniform not-found for unknown and foreign IDs — never null or an empty DTO in its place.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 16 lines and 921 bytes. Source hash: `ed6b8ad03c7c4bda6ca609acb8f987cc5fc293a00ca776660f17d5b39b2bdae1`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using TutoringCentre.Application.Common.Cqrs;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs#L9): `internal sealed class GetParentHandler(IParentReadService readService) : IQueryHandler<GetParentQuery, ParentDetailDto>`
- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs#L11): `public async Task<Result<ParentDetailDto>> HandleAsync(GetParentQuery query, CancellationToken cancellationToken)`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs:14](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs#L14)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:167](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L167)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents.Queries.GetParent;

/// <summary>Uniform not-found for unknown and foreign IDs — never null or an empty DTO in its place.</summary>
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class GetParentHandler(IParentReadService readService) : IQueryHandler<GetParentQuery, ParentDetailDto>
{
    public async Task<Result<ParentDetailDto>> HandleAsync(GetParentQuery query, CancellationToken cancellationToken)
    {
        var parent = await readService.GetDetailAsync(query.ParentId, cancellationToken);
        return parent is null ? Result<ParentDetailDto>.Failure(ParentErrors.NotFound()) : Result<ParentDetailDto>.Success(parent);
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
