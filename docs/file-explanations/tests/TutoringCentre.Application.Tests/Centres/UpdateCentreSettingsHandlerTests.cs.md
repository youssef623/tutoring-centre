# `tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs`

## Purpose

Declares `UpdateCentreSettingsHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 56 lines and 2,590 bytes. Source hash: `f1684f5e41621ac4309b360e5649cbea77d5ebe9650c3942f4d801c6eb34a4fb`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Application.Tests.Fakes;`
- Line 4: `using TutoringCentre.Domain.Centres;`
- Line 5: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs#L9): `public sealed class UpdateCentreSettingsHandlerTests`
- [Line 12](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs#L12): `public async Task HandleAsync_ValidUpdate_ChangesNameAndLocale()`
- [Line 29](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs#L29): `public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndChangesNothing()`
- [Line 47](../../../../../tests/TutoringCentre.Application.Tests/Centres/UpdateCentreSettingsHandlerTests.cs#L47): `public async Task HandleAsync_MissingCentre_Throws()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Centres;

public sealed class UpdateCentreSettingsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ValidUpdate_ChangesNameAndLocale()
    {
        var centre = Centre.Create("Nile Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar).Value;
        var repository = new FakeCentreRepository();
        repository.Existing.Add(centre);
        var currentActor = new FakeCurrentActor { Actor = new StaffActor(Guid.NewGuid(), centre.Id, StaffRole.Owner) };
        var handler = new UpdateCentreSettingsHandler(currentActor, repository);

        var result = await handler.HandleAsync(
            new UpdateCentreSettingsCommand("Nile Learning Centre", SupportedLocale.En, 0), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal("Nile Learning Centre", centre.Name);
        Assert.Equal(SupportedLocale.En, centre.DefaultLocale);
    }

    [Fact]
    public async Task HandleAsync_InvalidName_ReturnsNameInvalidAndChangesNothing()
    {
        var centre = Centre.Create("Nile Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar).Value;
        var repository = new FakeCentreRepository();
        repository.Existing.Add(centre);
        var currentActor = new FakeCurrentActor { Actor = new StaffActor(Guid.NewGuid(), centre.Id, StaffRole.Owner) };
        var handler = new UpdateCentreSettingsHandler(currentActor, repository);

        var result = await handler.HandleAsync(
            new UpdateCentreSettingsCommand("", SupportedLocale.En, 0), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("centre.name_invalid", result.Error!.Code);
        Assert.Equal("Nile Centre", centre.Name);
        Assert.Equal(SupportedLocale.Ar, centre.DefaultLocale);
    }

    [Fact]
    public async Task HandleAsync_MissingCentre_Throws()
    {
        var repository = new FakeCentreRepository();
        var currentActor = new FakeCurrentActor { Actor = new StaffActor(Guid.NewGuid(), Guid.CreateVersion7(), StaffRole.Owner) };
        var handler = new UpdateCentreSettingsHandler(currentActor, repository);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new UpdateCentreSettingsCommand("Name", SupportedLocale.En, 0), CancellationToken.None));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
