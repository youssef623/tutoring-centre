# `src/TutoringCentre.Application/Academics/Parents/IParentReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): Read-side port: returns DTO-shaped data directly. Never exposes the Parent entity or EF types.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Parents/IParentReadService.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 26 lines and 1,536 bytes. Source hash: `03c8d7603c506370f072ec6362b123635f6d9e7c7b16e7d7baaf01cd077fad89`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../../src/TutoringCentre.Application/Academics/Parents/IParentReadService.cs#L7): `public interface IParentReadService`
- [Line 17](../../../../../../src/TutoringCentre.Application/Academics/Parents/IParentReadService.cs#L17): `public sealed record ParentDto(Guid Id, string FullName, string Phone, uint Version);`
- [Line 20](../../../../../../src/TutoringCentre.Application/Academics/Parents/IParentReadService.cs#L20): `public sealed record ParentDetailDto(Guid Id, string FullName, string Phone, uint Version, IReadOnlyList<ParentChildDto> Children);`
- [Line 23](../../../../../../src/TutoringCentre.Application/Academics/Parents/IParentReadService.cs#L23): `public sealed record ParentChildDto(Guid StudentId, string FullName, StudentStatus Status, ParentRelationship Relationship, bool IsPrimary);`
- [Line 26](../../../../../../src/TutoringCentre.Application/Academics/Parents/IParentReadService.cs#L26): `public sealed record ParentLookupDto(ParentDto? Parent);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/GetParent/GetParentHandler.cs#L9)
- [src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs:9](../../../../../../src/TutoringCentre.Application/Academics/Parents/Queries/LookupParentByPhone/LookupParentByPhoneHandler.cs#L9)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:137](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L137)
- [src/TutoringCentre.Infrastructure/ReadServices/ParentReadService.cs:18](../../../../../../src/TutoringCentre.Infrastructure/ReadServices/ParentReadService.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:164](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L164)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeParentReadService.cs:7](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentReadService.cs#L7)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents;

/// <summary>Read-side port: returns DTO-shaped data directly. Never exposes the Parent entity or EF types.</summary>
public interface IParentReadService
{
    /// <summary>One parent with their children, or null when it does not exist or is not visible to the acting centre.</summary>
    Task<ParentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct);

    /// <summary>The parent holding this phone in the acting centre, or null.</summary>
    Task<ParentDto?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct);
}

/// <summary>The one shape every plain parent read returns. No centre, no search name, no timestamps — those are persistence details.</summary>
public sealed record ParentDto(Guid Id, string FullName, string Phone, uint Version);

/// <summary>A parent plus the students linked to them, for the parent detail page.</summary>
public sealed record ParentDetailDto(Guid Id, string FullName, string Phone, uint Version, IReadOnlyList<ParentChildDto> Children);

/// <summary>One student linked to a parent, from the parent's point of view.</summary>
public sealed record ParentChildDto(Guid StudentId, string FullName, StudentStatus Status, ParentRelationship Relationship, bool IsPrimary);

/// <summary>A lookup's answer: the parent holding the looked-up phone, or null — absence is data, never a 404.</summary>
public sealed record ParentLookupDto(ParentDto? Parent);
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
