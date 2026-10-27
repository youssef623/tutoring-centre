# `tests/TutoringCentre.Application.Tests/Fakes/FakeParentRepository.cs`

## Purpose

Source summary (intent, not independent execution proof): In-memory parent repository. `Existing` simulates stored rows; `Added` records Add calls. `PhoneExistsCallCount` lets a test prove the uniqueness query never runs when the phone is unchanged on update.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentRepository.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 36 lines and 1,512 bytes. Source hash: `ce240ef0ab63bdcba78ff4cae521486081d674cab7a112cc7f8b8674ec07037b`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Academics.Parents;`
- Line 2: `using TutoringCentre.Domain.Academics;`
- Line 3: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentRepository.cs#L12): `public sealed class FakeParentRepository : IParentRepository`
- [Line 20](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentRepository.cs#L20): `public Task<Parent?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>`
- [Line 23](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentRepository.cs#L23): `public Task<Parent?> FindAsync(Guid id, CancellationToken ct) =>`
- [Line 26](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentRepository.cs#L26): `public Task<Parent?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct) =>`
- [Line 29](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentRepository.cs#L29): `public Task<bool> PhoneExistsAsync(PhoneNumber phone, Guid? excludingId, CancellationToken ct)`
- [Line 35](../../../../../tests/TutoringCentre.Application.Tests/Fakes/FakeParentRepository.cs#L35): `public void Add(Parent parent) => Added.Add(parent);`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs:19](../../../../../tests/TutoringCentre.Application.Tests/Academics/CreateParentHandlerTests.cs#L19)
- [tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs:153](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentDispatcherAuthorizationTests.cs#L153)
- [tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs:18](../../../../../tests/TutoringCentre.Application.Tests/Academics/ParentLinkHandlerTests.cs#L18)
- [tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs:136](../../../../../tests/TutoringCentre.Application.Tests/Academics/RegisterStudentHandlerTests.cs#L136)
- [tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs:153](../../../../../tests/TutoringCentre.Application.Tests/Academics/StudentDispatcherAuthorizationTests.cs#L153)
- [tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs:15](../../../../../tests/TutoringCentre.Application.Tests/Academics/UpdateParentHandlerTests.cs#L15)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Fakes;

/// <summary>
/// In-memory parent repository. <see cref="Existing"/> simulates stored rows; <see cref="Added"/> records Add
/// calls. <see cref="PhoneExistsCallCount"/> lets a test prove the uniqueness query never runs when the phone
/// is unchanged on update.
/// </summary>
public sealed class FakeParentRepository : IParentRepository
{
    public List<Parent> Existing { get; } = [];

    public List<Parent> Added { get; } = [];

    public int PhoneExistsCallCount { get; private set; }

    public Task<Parent?> GetByIdAsync(Guid id, uint expectedVersion, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(parent => parent.Id == id));

    public Task<Parent?> FindAsync(Guid id, CancellationToken ct) =>
        Task.FromResult(Existing.FirstOrDefault(parent => parent.Id == id));

    public Task<Parent?> FindByPhoneAsync(PhoneNumber phone, CancellationToken ct) =>
        Task.FromResult(Existing.Concat(Added).FirstOrDefault(parent => parent.Phone.Equals(phone)));

    public Task<bool> PhoneExistsAsync(PhoneNumber phone, Guid? excludingId, CancellationToken ct)
    {
        PhoneExistsCallCount++;
        return Task.FromResult(Existing.Concat(Added).Any(parent => parent.Phone.Equals(phone) && parent.Id != excludingId));
    }

    public void Add(Parent parent) => Added.Add(parent);
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
