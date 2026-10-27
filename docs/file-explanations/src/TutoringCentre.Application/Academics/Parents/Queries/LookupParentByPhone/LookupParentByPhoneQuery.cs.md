# `src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneQuery.cs`

## Purpose

Source summary (intent, not independent execution proof): "Is there already a parent with this phone in my centre?" — the question the registration form asks before creating a duplicate. Absence is a normal answer (a null inside a success), never a 404. No centre: scope comes from the actor.

## Where it fits

Category: **source**. [Open current file](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneQuery.cs). [Complete file index](../../../../../../INDEX.md). [Current teaching overview](../../../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 14 lines and 639 bytes. Source hash: `1389a8deff548708875441cded4e5a28c531c0d07ada86aa01b8df61b6be00cd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Common.Cqrs;`
- Line 2: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 11](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneQuery.cs#L11): `public sealed record LookupParentByPhoneQuery(string Phone) : IQuery<ParentLookupDto>, ITenantScoped, IRequirePermission`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Api/Cli/SeedCommand.cs:263](../../../../../../../../src/TutoringCentre.Api/Cli/SeedCommand.cs#L263)
- [src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs:78](../../../../../../../../src/TutoringCentre.Api/Endpoints/ParentEndpoints.cs#L78)
- [src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs:9](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneValidator.cs:6](../../../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneValidator.cs#L6)
- [tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs:18](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:67](../../../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L67)
- [tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs:84](../../../../../../../../tests/TutoringCentre.Infrastructure.Tests/Academics/ParentPersistenceTests.cs#L84)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;

/// <summary>
/// "Is there already a parent with this phone in my centre?" — the question the registration form asks
/// before creating a duplicate. Absence is a normal answer (a null inside a success), never a 404. No centre:
/// scope comes from the actor.
/// </summary>
public sealed record LookupParentByPhoneQuery(string Phone) : IQuery<ParentLookupDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.StudentsView;
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
