# `tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs`

## Purpose

Declares `CreateCentreTests`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 61 lines and 2,709 bytes. Source hash: `ac2920781f60221ebfa2199da9a3c3f3af756ec195e5d5930ba9a3bc7c3ae57f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Centres.Commands.CreateCentre;`
- Line 2: `using TutoringCentre.Application.Common.Security;`
- Line 3: `using TutoringCentre.Domain.Centres;`
- Line 4: `using TutoringCentre.Domain.Common;`
- Line 5: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs#L9): `public sealed class CreateCentreTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 11](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs#L11): `private static CreateCentreCommand ValidCommand(string slug = "nile-centre") =>`
- [Line 15](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs#L15): `public async Task SendAsync_SystemActorValidCommand_PersistsCentreWithCreatedAtAndNoUpdatedAt()`
- [Line 27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs#L27): `public async Task SendAsync_SlugAlreadyExists_ReturnsConflictAndKeepsOneRow()`
- [Line 40](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs#L40): `public async Task SendAsync_AnonymousActor_ReturnsForbiddenAndWritesNothing()`
- [Line 51](../../../../../tests/TutoringCentre.Infrastructure.Tests/Centres/CreateCentreTests.cs#L51): `public async Task SendAsync_InvalidTimeZone_ReturnsDomainErrorAndWritesNothing()`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Centres.Commands.CreateCentre;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Centres;

public sealed class CreateCentreTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    private static CreateCentreCommand ValidCommand(string slug = "nile-centre") =>
        new("Nile Tutoring Centre", slug, "Africa/Cairo", SupportedLocale.Ar);

    [Fact]
    public async Task SendAsync_SystemActorValidCommand_PersistsCentreWithCreatedAtAndNoUpdatedAt()
    {
        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), ValidCommand());

        Assert.True(result.IsSuccess);
        Assert.Equal("nile-centre", result.Value.Slug);
        Assert.Equal(1, await Fixture.CountCentresAsync());
        Assert.Equal(1, await Fixture.ScalarAsync<long>(
            "select count(*) from platform.centres where slug = 'nile-centre' and created_at is not null and updated_at is null"));
    }

    [Fact]
    public async Task SendAsync_SlugAlreadyExists_ReturnsConflictAndKeepsOneRow()
    {
        await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), ValidCommand());

        var second = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), ValidCommand());

        Assert.True(second.IsFailure);
        Assert.Equal("centre.slug_taken", second.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, second.Error.Kind);
        Assert.Equal(1, await Fixture.CountCentresAsync());
    }

    [Fact]
    public async Task SendAsync_AnonymousActor_ReturnsForbiddenAndWritesNothing()
    {
        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new AnonymousActor(), ValidCommand());

        Assert.True(result.IsFailure);
        Assert.Equal("centre.create_forbidden", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Equal(0, await Fixture.CountCentresAsync());
    }

    [Fact]
    public async Task SendAsync_InvalidTimeZone_ReturnsDomainErrorAndWritesNothing()
    {
        var command = new CreateCentreCommand("Nile Tutoring Centre", "nile-centre", "Mars/Base", SupportedLocale.Ar);

        var result = await Fixture.SendAsAsync<CreateCentreCommand, CreateCentreResult>(new SystemActor(null), command);

        Assert.True(result.IsFailure);
        Assert.Equal("centre.time_zone_invalid", result.Error!.Code);
        Assert.Equal(0, await Fixture.CountCentresAsync());
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
