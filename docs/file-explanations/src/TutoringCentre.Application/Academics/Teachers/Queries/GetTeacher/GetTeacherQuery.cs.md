# `src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): Used by the edit dialog and by the cross-tenant probes. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 10 lines and 459 bytes. Source hash: `c7e78774fe4ac2125e80ac911dd3b7e7d375fef6f7d4bc41746d25daa7539d89`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherQuery.cs#L7): `public sealed record GetTeacherQuery(Guid TeacherId) : IQuery<TeacherDto>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:489](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L489)
- [src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs:86](../../../../../../../../src/TutoringCentre.Api/Endpoints/TeacherEndpoints.cs#L86)
- [src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherHandler.cs:9](../../../../../../../../src/TutoringCentre.Application/Academics/Teachers/Queries/GetTeacher/GetTeacherHandler.cs#L9)
- [tests/TutoringCentre.Application.Tests/Academics/GetTeacherHandlerTests.cs:17](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/GetTeacherHandlerTests.cs#L17)
- [tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs:184](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/TeacherDispatcherPermissionTests.cs#L184)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs:69](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupTestBase.cs#L69)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs:190](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/GroupUseCaseTests.cs#L190)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs:34](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherAccountLinkTests.cs#L34)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs:33](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/TeacherUseCaseTests.cs#L33)
- [tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs:115](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Identity/TeacherScopeProviderTests.cs#L115)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Teachers.Queries.GetTeacher;

/// <summary>Used by the edit dialog and by the cross-tenant probes. No centre: scope comes from the actor.</summary>
public sealed record GetTeacherQuery(Guid TeacherId) : IQuery<TeacherDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.TeachersView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
