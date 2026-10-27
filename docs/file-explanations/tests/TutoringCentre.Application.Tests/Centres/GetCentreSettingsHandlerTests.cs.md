# `tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs`

## Purpose

Declares `GetCentreSettingsHandlerTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 38 lines and 1,632 bytes. Source hash: `8168319a9c363817abc994bce00ace5a27d8d54c3068516ef6d4b802f73f8b9d`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres;`
- Line 2: `using TutoringCentre.Application.Centres.Queries.GetCentreSettings;`
- Line 3: `using TutoringCentre.Application.Common.Security;`
- Line 4: `using TutoringCentre.Application.Tests.Fakes;`
- Line 5: `using TutoringCentre.Domain.Centres;`
- Line 6: `using TutoringCentre.Domain.Identity;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 10](../../../../../tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs#L10): `public sealed class GetCentreSettingsHandlerTests`
- [Line 13](../../../../../tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs#L13): `public async Task HandleAsync_ReturnsTheActorsOwnCentreSettings()`
- [Line 29](../../../../../tests/TutoringCentre.Application.Tests/Centres/GetCentreSettingsHandlerTests.cs#L29): `public async Task HandleAsync_MissingCentre_Throws()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Centres;
using TutoringCentre.Application.Centres.Queries.GetCentreSettings;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Centres;

public sealed class GetCentreSettingsHandlerTests
{
    [Fact]
    public async Task HandleAsync_ReturnsTheActorsOwnCentreSettings()
    {
        var centreId = Guid.CreateVersion7();
        var readService = new FakeCentreReadService();
        var dto = new CentreSettingsDto("Nile Centre", "nile-centre", "Africa/Cairo", SupportedLocale.Ar, 7);
        readService.SettingsByCentreId[centreId] = dto;
        var currentActor = new FakeCurrentActor { Actor = new StaffActor(Guid.NewGuid(), centreId, StaffRole.Owner) };
        var handler = new GetCentreSettingsHandler(currentActor, readService);

        var result = await handler.HandleAsync(new GetCentreSettingsQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Same(dto, result.Value);
    }

    [Fact]
    public async Task HandleAsync_MissingCentre_Throws()
    {
        var readService = new FakeCentreReadService();
        var currentActor = new FakeCurrentActor { Actor = new StaffActor(Guid.NewGuid(), Guid.CreateVersion7(), StaffRole.Owner) };
        var handler = new GetCentreSettingsHandler(currentActor, readService);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            handler.HandleAsync(new GetCentreSettingsQuery(), CancellationToken.None));
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
