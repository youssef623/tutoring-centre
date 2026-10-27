# `src/TutoringCentre.Application/Academics/Subjects/Queries/GetSubject/GetSubjectQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): Used by the edit dialog and by the cross-tenant probes. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/GetSubject/GetSubjectQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 459 bytes. Source hash: `6cd8a33aa1053d7534427bca0d712924fe4c18e70dbd399042a9a626546af7a9`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/GetSubject/GetSubjectQuery.cs#L7): `public sealed record GetSubjectQuery(Guid SubjectId) : IQuery<SubjectDto>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs:76](../../../../../../../../src/TutoringCentre.Api/Endpoints/SubjectEndpoints.cs#L76)
- [src/TutoringCentre.Application/Academics/Subjects/Queries/GetSubject/GetSubjectHandler.cs:9](../../../../../../../../src/TutoringCentre.Application/Academics/Subjects/Queries/GetSubject/GetSubjectHandler.cs#L9)
- [tests/TutoringCentre.Application.Tests/Academics/GetSubjectHandlerTests.cs:17](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GetSubjectHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs:148](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherPermissionTests.cs#L148)
- [tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs:89](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/SubjectDispatcherTenantScopeTests.cs#L89)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs:64](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/AuditTrailTests.cs#L64)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs:62](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L62)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs:32](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectCrossTenantTests.cs#L32)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs:53](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectStaleVersionTests.cs#L53)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs:35](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/SubjectUseCaseTests.cs#L35)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Subjects.Queries.GetSubject;

/// <summary>Used by the edit dialog and by the cross-tenant probes. No centre: scope comes from the actor.</summary>
public sealed record GetSubjectQuery(Guid SubjectId) : IQuery<SubjectDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.SubjectsView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
