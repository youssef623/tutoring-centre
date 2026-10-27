# `tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs`

## Purpose

Declares `GetSystemInfoHandlerTests`, `FakeSystemInfoReadService`, `FakeDemoModeProvider`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 74 lines and 2,923 bytes. Source hash: `d99e2f676a47e096fc4282a2fddcf61e96b08887af51a363fa1a34739ead7bbc`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Application.Platform;`
- Line 2: `using TutoringCentre.Application.Platform.Queries.GetSystemInfo;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 6](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L6): `public sealed class GetSystemInfoHandlerTests`
- [Line 9](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L9): `public async Task HandleAsync_NoPendingMigrations_ReportsDatabaseUpToDate()`
- [Line 23](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L23): `public async Task HandleAsync_TwoPendingMigrations_ReportsDatabaseNotUpToDate()`
- [Line 36](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L36): `public async Task HandleAsync_AnyStatus_ReturnsNonEmptyVersion()`
- [Line 54](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L54): `public async Task HandleAsync_ReportsDemoModeFromProvider(bool demoEnabled)`
- [Line 65](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L65): `private sealed class FakeSystemInfoReadService(SchemaStatus status) : ISystemInfoReadService`
- [Line 67](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L67): `public Task<SchemaStatus> GetSchemaStatusAsync(CancellationToken ct) => Task.FromResult(status);`
- [Line 70](../../../../../tests/TutoringCentre.Application.Tests/Platform/GetSystemInfoHandlerTests.cs#L70): `private sealed class FakeDemoModeProvider(bool isEnabled) : IDemoModeProvider`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Application.Platform;
using TutoringCentre.Application.Platform.Queries.GetSystemInfo;

namespace TutoringCentre.Application.Tests.Platform;

public sealed class GetSystemInfoHandlerTests
{
    [Fact]
    public async Task HandleAsync_NoPendingMigrations_ReportsDatabaseUpToDate()
    {
        var handler = new GetSystemInfoHandler(
            new FakeSystemInfoReadService(new SchemaStatus("20261012_InitialPlatform", 0)),
            new FakeDemoModeProvider(false));

        var result = await handler.HandleAsync(new GetSystemInfoQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.True(result.Value.DatabaseUpToDate);
        Assert.Equal("20261012_InitialPlatform", result.Value.LatestMigration);
    }

    [Fact]
    public async Task HandleAsync_TwoPendingMigrations_ReportsDatabaseNotUpToDate()
    {
        var handler = new GetSystemInfoHandler(
            new FakeSystemInfoReadService(new SchemaStatus("20261012_InitialPlatform", 2)),
            new FakeDemoModeProvider(false));

        var result = await handler.HandleAsync(new GetSystemInfoQuery(), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.False(result.Value.DatabaseUpToDate);
    }

    [Fact]
    public async Task HandleAsync_AnyStatus_ReturnsNonEmptyVersion()
    {
        var handler = new GetSystemInfoHandler(
            new FakeSystemInfoReadService(new SchemaStatus(null, 0)),
            new FakeDemoModeProvider(false));

        var result = await handler.HandleAsync(new GetSystemInfoQuery(), CancellationToken.None);

        // Whatever the SDK put in AssemblyInformationalVersionAttribute is returned as-is (Task 34.2): once the
        // Dockerfile passes -p:SourceRevisionId=<git sha> to `dotnet publish`, that is a "+<sha>" suffix here,
        // deliberately kept rather than stripped, so this is how an operator confirms what is actually deployed.
        Assert.False(string.IsNullOrWhiteSpace(result.Value.ApplicationVersion));
        Assert.Null(result.Value.LatestMigration);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task HandleAsync_ReportsDemoModeFromProvider(bool demoEnabled)
    {
        var handler = new GetSystemInfoHandler(
            new FakeSystemInfoReadService(new SchemaStatus(null, 0)),
            new FakeDemoModeProvider(demoEnabled));

        var result = await handler.HandleAsync(new GetSystemInfoQuery(), CancellationToken.None);

        Assert.Equal(demoEnabled, result.Value.DemoMode);
    }

    private sealed class FakeSystemInfoReadService(SchemaStatus status) : ISystemInfoReadService
    {
        public Task<SchemaStatus> GetSchemaStatusAsync(CancellationToken ct) => Task.FromResult(status);
    }

    private sealed class FakeDemoModeProvider(bool isEnabled) : IDemoModeProvider
    {
        public bool IsEnabled => isEnabled;
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
