# `src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): Used by the parent detail page. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 436 bytes. Source hash: `c3dec11eb8d43fb37feb69387c49a76264a42cbe7109a9f8bdf7bba09b6c7f49`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentQuery.cs#L7): `public sealed record GetParentQuery(Guid ParentId) : IQuery<ParentDetailDto>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs:65](../../../../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L65)
- [src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs:9](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs#L9)
- [tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs:16](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:56](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L56)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs:48](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L48)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Parents.Queries.GetParent;

/// <summary>Used by the parent detail page. No centre: scope comes from the actor.</summary>
public sealed record GetParentQuery(Guid ParentId) : IQuery<ParentDetailDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
