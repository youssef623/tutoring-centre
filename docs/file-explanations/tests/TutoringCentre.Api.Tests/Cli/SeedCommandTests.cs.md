# `tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs`

## Purpose

Declares `SeedCommandTests`, `FakeHostEnvironment`, `per`, `owns`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 403 lines and 19,290 bytes. Source hash: `a08489fccd50918ce4622100d735c1cacbd67453c60d4aa6dfa8cdbf14f28a02`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using Microsoft.Extensions.FileProviders;`
- Line 2: `using Microsoft.Extensions.Hosting;`
- Line 3: `using TutoringCentre.Api.Cli;`
- Line 4: `using TutoringCentre.Api.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 9](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L9): `public sealed class SeedCommandTests(ApiFactory factory) : IAsyncLifetime`
- [Line 11](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L11): `public Task InitializeAsync() => factory.ResetAsync();`
- [Line 13](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L13): `public Task DisposeAsync() => Task.CompletedTask;`
- [Line 18](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L18): `public void SeedGate_OutsideProduction_AlwaysAllowed(string environmentName) =>`
- [Line 22](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L22): `public void SeedGate_Production_WithDemoDisabled_IsRefused() =>`
- [Line 26](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L26): `public void SeedGate_Production_WithDemoEnabled_IsAllowed() =>`
- [Line 32](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L32): `public void SeedGate_OutsideProduction_IncludesTheSetupCentre(string environmentName) =>`
- [Line 36](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L36): `public void SeedGate_Production_NeverIncludesTheSetupCentre_EvenOnADemoInstance()`
- [Line 42](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L42): `private static FakeHostEnvironment FakeEnvironment(string environmentName) => new(environmentName);`
- [Line 44](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L44): `private sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment`
- [Line 53](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L53): `public async Task RunAsync_CalledTwice_CreatesExactlyTheTwoDemoCentresAndTheEmptySetupCentre()`
- [Line 69](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L69): `public async Task RunAsync_LeavesTheSetupCentreEmpty_ExceptForItsOneOwner()`
- [Line 96](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L96): `public async Task RunAsync_CalledTwice_CreatesExactlyTheStaffSetIdempotently()`
- [Line 119](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L119): `public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoSubjectsIdempotently()`
- [Line 146](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L146): `public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoStudentsAndParentsIdempotently()`
- [Line 208](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L208): `public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoTeachersAndGroupsIdempotently()`
- [Line 261](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L261): `// The two-centre teacher account is linked to exactly one teacher record per centre, and in Nile that`
- [Line 262](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L262): `// record owns exactly one group; the unlinked Nile teacher owns the rest.`
- [Line 288](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L288): `public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoEnrollmentsIdempotently_LeavingOneSeatInTheTwoSeatGroup()`
- [Line 319](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L319): `public async Task RunAsync_CalledTwice_CreatesExactlyTheDemoSchedulesIdempotently_FromTheSeedDay()`
- [Line 366](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L366): `private Task<string> PerCentreCountsAsync(string table) =>`
- [Line 376](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L376): `private Task<string> DatasetCountsAsync() =>`
- [Line 395](../../../../../tests/TutoringCentre.Api.Tests/Cli/SeedCommandTests.cs#L395): `private Task<string> EnrollmentCountsAsync() =>`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [db/bootstrap-roles.sh:80](../../../../../db/bootstrap-roles.sh#L80)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using TutoringCentre.Api.Cli;
using TutoringCentre.Api.Tests.Fixtures;

namespace TutoringCentre.Api.Tests.Cli;

[Collection(ApiCollection.Name)]
public sealed class SeedCommandTests(ApiFactory factory) : IAsyncLifetime
{
    public Task InitializeAsync() => factory.ResetAsync();

    public Task DisposeAsync() => Task.CompletedTask;

    [Theory]
    [InlineData("Development")]
    [InlineData("Testing")]
    public void SeedGate_OutsideProduction_AlwaysAllowed(string environmentName) =>
        Assert.True(SeedGate.IsAllowed(FakeEnvironment(environmentName), new DemoOptions { Enabled = false }));

    [Fact]
    public void SeedGate_Production_WithDemoDisabled_IsRefused() =>
        Assert.False(SeedGate.IsAllowed(FakeEnvironment("Production"), new DemoOptions { Enabled = false }));

    [Fact]
    public void SeedGate_Production_WithDemoEnabled_IsAllowed() =>
        Assert.True(SeedGate.IsAllowed(FakeEnvironment("Production"), new DemoOptions { Enabled = true }));

    [Theory]
    [InlineData("Development")]
    [InlineData("Testing")]
    public void SeedGate_OutsideProduction_IncludesTheSetupCentre(string environmentName) =>
        Assert.True(SeedGate.IncludesSetupCentre(FakeEnvironment(environmentName)));

    [Fact]
    public void SeedGate_Production_NeverIncludesTheSetupCentre_EvenOnADemoInstance()
    {
        Assert.True(SeedGate.IsAllowed(FakeEnvironment("Production"), new DemoOptions { Enabled = true }));
        Assert.False(SeedGate.IncludesSetupCentre(FakeEnvironment("Production")));
    }

    private static FakeHostEnvironment FakeEnvironment(string environmentName) => new(environmentName);

    private sealed class FakeHostEnvironment(string environmentName) : IHostEnvironment
    {
        public string EnvironmentName { get; set; } = environmentName;
        public string ApplicationName { get; set; } = "TutoringCentre.Api.Tests";
        public string ContentRootPath { get; set; } = AppContext.BaseDirectory;
        public IFileProvider ContentRootFileProvider { get; set; } = new NullFileProvider();
    }

    [Fact]
    public async Task RunAsync_CalledTwice_CreatesExactlyTheTwoDemoCentresAndTheEmptySetupCentre()
    {
        var firstExitCode = await SeedCommand.RunAsync(factory.Services);
        var secondExitCode = await SeedCommand.RunAsync(factory.Services);

        Assert.Equal(0, firstExitCode);
        Assert.Equal(0, secondExitCode);

        // The tests run as Testing, so the setup centre (Development and Testing only) is here too.
        Assert.Equal(3, await factory.ScalarAsync<long>("select count(*) from platform.centres"));
        Assert.Equal(
            "giza-new,maadi-hub,nile-centre",
            await factory.ScalarAsync<string>("select string_agg(slug, ',' order by slug) from platform.centres"));
    }

    [Fact]
    public async Task RunAsync_LeavesTheSetupCentreEmpty_ExceptForItsOneOwner()
    {
        Assert.Equal(0, await SeedCommand.RunAsync(factory.Services));

        Assert.Equal(
            "owner@giza.test:owner:active",
            await factory.ScalarAsync<string>(
                """
                select string_agg(u.email || ':' || m.role || ':' || m.status, ',') from identity.memberships m
                join identity.users u on u.id = m.user_id
                join platform.centres c on c.id = m.centre_id
                where c.slug = 'giza-new'
                """));
        Assert.Equal(
            0,
            await factory.ScalarAsync<long>(
                """
                select (select count(*) from academics.subjects t join platform.centres c on c.id = t.centre_id where c.slug = 'giza-new')
                     + (select count(*) from academics.teachers t join platform.centres c on c.id = t.centre_id where c.slug = 'giza-new')
                     + (select count(*) from academics.groups t join platform.centres c on c.id = t.centre_id where c.slug = 'giza-new')
                     + (select count(*) from academics.students t join platform.centres c on c.id = t.centre_id where c.slug = 'giza-new')
                     + (select count(*) from academics.parents t join platform.centres c on c.id = t.centre_id where c.slug = 'giza-new')
```

The excerpt stops at line 90; use the original file for the remaining 313 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
