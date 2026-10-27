# `src/TutoringCentre.Application/Academics/Parents/IParentRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): Write-side persistence port for the Parent aggregate. Methods are named by use; nothing here saves — the dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query filter and row-level security, so this interface cannot be called with the wrong tenant. A parent of another centre is simply not found by any of these.

## Where it fits

Category: **source**. [Open current file](../../../../../../src/TutoringCentre.Application/Academics/Parents/IParentRepository.cs). [Complete file index](../../../../INDEX.md). [Current teaching overview](../../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 31 lines and 1,572 bytes. Source hash: `444118baaada1d62d58df9cf4d75fc9b2b6eac3c5b8953041c42728569b26bfd`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Domain.Academics;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../../src/TutoringCentre.Application/Academics/Parents/IParentRepository.cs#L9): `/// filter and row-level security, so this interface cannot be called with the wrong tenant. A parent of`
- [Line 12](../../../../../../src/TutoringCentre.Application/Academics/Parents/IParentRepository.cs#L12): `public interface IParentRepository`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs:11](../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/CreateParent/CreateParentHandler.cs#L11)
- [src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Parents/Commands/UpdateParent/UpdateParentHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs:14](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/LinkParent/LinkParentHandler.cs#L14)
- [src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs:17](../../../../../../src/TutoringCentre.Application/Academics/Students/Commands/RegisterStudent/RegisterStudentHandler.cs#L17)
- [src/TutoringCentre.Infrastructure/DependencyInjection.cs:136](../../../../../../src/TutoringCentre.Infrastructure/DependencyInjection.cs#L136)
- [src/TutoringCentre.Infrastructure/Repositories/ParentRepository.cs:16](../../../../../../src/TutoringCentre.Infrastructure/Repositories/ParentRepository.cs#L16)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:163](../../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L163)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:166](../../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L166)
- [tests/TutoringCentre.Application.Tests/Fakes/FakeParentRepository.cs:12](../../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentRepository.cs#L12)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Academics.Parents;

/// <summary>
/// Write-side persistence port for the Parent aggregate. Methods are named by use; nothing here saves — the
/// dispatcher does. No centre parameter anywhere: scoping is applied underneath this port by the EF query
/// filter and row-level security, so this interface cannot be called with the wrong tenant. A parent of
/// another centre is simply not found by any of these.
/// </summary>
public interface IParentRepository
{
    /// <summary>
    /// Loads one parent for modification, expecting it to still be at <paramref name="expectedVersion"/>; the
    /// save, not the handler, detects a stale version.
    /// </summary>
    Task<Parent?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct);

    /// <summary>Loads one parent for modification with no version expectation — for when a student references it (Day 40).</summary>
    Task<Parent?> FindAsync(Guid id, CancellationToken ct);

    /// <summary>The parent holding this phone in the acting centre, or null.</summary>
    Task<Parent?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct);

    /// <summary>Whether a parent with this phone already exists in the acting centre, optionally excluding one parent (for update).</summary>
    Task<bool> PhoneExistsAsync(PhoneNumber phone, Guid? excludingId, CancellationToken ct);

    /// <summary>Tracks a new parent; the dispatcher saves it.</summary>
    void Add(Parent parent);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
