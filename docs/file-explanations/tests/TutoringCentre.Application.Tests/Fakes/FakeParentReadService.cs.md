# `tests/TutoringCentre.Application.Tests/Fakes/FakeParentReadService.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory stand-in for parent reads.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentReadService.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 18 lines and 712 bytes. Source hash: `bf6de692ec768187b0e10bd5b38f86955aceb2b7a652989891a6fd94678ec3d2`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Parents;`
- Line 2: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 7](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentReadService.cs#L7): `public sealed class FakeParentReadService : IParentReadService`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentReadService.cs#L13): `public Task<ParentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct) =>`
- [Line 16](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentReadService.cs#L16): `public Task<ParentDto?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct) =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs:13](../../../../../tests/TutoringCentre.Application.Tests/Academics/GetParentHandlerTests.cs#L13)
- [tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs:14](../../../../../tests/TutoringCentre.Application.Tests/Academics/LookupParentByPhoneHandlerTests.cs#L14)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:154](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L154)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>In-memory stand-in for parent reads.</summary>
public sealed class FakeParentReadService : IParentReadService
{
    public List<ParentDetailDto> Details { get; } = [];

    public List<ParentDto> ByPhone { get; } = [];

    public Task<ParentDetailDto?> GetDetailAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Details.FirstOrDefault(parent => parent.Id == id));

    public Task<ParentDto?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct) =>
        Task.FromResult(ByPhone.FirstOrDefault(parent => parent.Phone == phone.Value));
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
