# `tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs`

## Purpose

Declares `TransactionBoundaryTests`, `WriteThenFailMode`, `WriteThenFailCommand`, `WriteThenFailHandler`. The declaration map below identifies the current source locations; the overview explains the surrounding execution path.

## Where it fits

Category: **test**. [Open current file](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 89 lines and 3,542 bytes. Source hash: `e575a5f3b62ecca38528265f3d84bdf1ff71b6c12ba7322aad2a0e2b52be5e98`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.Extensions.DependencyInjection;`
- Line 3: `using TutoringCentre.Application.Centres;`
- Line 4: `using TutoringCentre.Application.Common.Cqrs;`
- Line 5: `using TutoringCentre.Application.Common.Security;`
- Line 6: `using TutoringCentre.Domain.Centres;`
- Line 7: `using TutoringCentre.Domain.Common;`
- Line 8: `using TutoringCentre.Infrastructure.Persistence;`
- Line 9: `using TutoringCentre.Infrastructure.Tests.Fixtures;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 13](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L13): `public sealed class TransactionBoundaryTests(PostgresFixture fixture) : PostgresTestBase(fixture)`
- [Line 16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L16): `public async Task SendAsync_HandlerThrowsAfterRowWasWritten_LeavesNoRowAndRethrows()`
- [Line 27](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L27): `public async Task SendAsync_HandlerReturnsFailureAfterRowWasWritten_LeavesNoRow()`
- [Line 40](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L40): `public async Task SendAsync_HandlerSucceedsAfterRowWasWritten_PersistsTheRow()`
- [Line 52](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L52): `private static void Register(IServiceCollection services) =>`
- [Line 56](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L56): `internal enum WriteThenFailMode`
- [Line 63](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L63): `internal sealed record WriteThenFailCommand(WriteThenFailMode Mode) : ICommand<string>;`
- [Line 66](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L66): `internal sealed class WriteThenFailHandler(ICentreRepository centres, AppDbContext db)`
- [Line 69](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/TransactionBoundaryTests.cs#L69): `public async Task<Result<string>> HandleAsync(WriteThenFailCommand command, CancellationToken cancellationToken)`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Centres;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Infrastructure.Persistence;
using TutoringCentre.Infrastructure.Tests.Fixtures;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

public sealed class TransactionBoundaryTests(PostgresFixture fixture) : PostgresTestBase(fixture)
{
    [Fact]
    public async Task SendAsync_HandlerThrowsAfterRowWasWritten_LeavesNoRowAndRethrows()
    {
        await using var provider = Fixture.CreateServiceProvider(Register);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            provider.SendAsAsync<WriteThenFailCommand, string>(new SystemActor(null), new WriteThenFailCommand(WriteThenFailMode.Throw)));

        Assert.Equal(0, await Fixture.CountCentresAsync());
    }

    [Fact]
    public async Task SendAsync_HandlerReturnsFailureAfterRowWasWritten_LeavesNoRow()
    {
        await using var provider = Fixture.CreateServiceProvider(Register);

        var result = await provider.SendAsAsync<WriteThenFailCommand, string>(
            new SystemActor(null), new WriteThenFailCommand(WriteThenFailMode.ReturnFailure));

        Assert.True(result.IsFailure);
        Assert.Equal("test.rule_broken", result.Error!.Code);
        Assert.Equal(0, await Fixture.CountCentresAsync());
    }

    [Fact]
    public async Task SendAsync_HandlerSucceedsAfterRowWasWritten_PersistsTheRow()
    {
        // Control: proves the two tests above would have seen the row had it survived.
        await using var provider = Fixture.CreateServiceProvider(Register);

        var result = await provider.SendAsAsync<WriteThenFailCommand, string>(
            new SystemActor(null), new WriteThenFailCommand(WriteThenFailMode.Succeed));

        Assert.True(result.IsSuccess);
        Assert.Equal(1, await Fixture.CountCentresAsync());
    }

    private static void Register(IServiceCollection services) =>
        services.AddScoped<ICommandHandler<WriteThenFailCommand, string>, WriteThenFailHandler>();
}

internal enum WriteThenFailMode
{
    Throw,
    ReturnFailure,
    Succeed,
}

internal sealed record WriteThenFailCommand(WriteThenFailMode Mode) : ICommand<string>;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by the DI container.")]
internal sealed class WriteThenFailHandler(ICentreRepository centres, AppDbContext db)
    : ICommandHandler<WriteThenFailCommand, string>
{
    public async Task<Result<string>> HandleAsync(WriteThenFailCommand command, CancellationToken cancellationToken)
    {
        var created = Centre.Create("Doomed Centre", "doomed-centre", "Africa/Cairo", SupportedLocale.En);
        centres.Add(created.Value);

        // Flush inside the dispatcher's transaction so the row physically exists before the failure.
        await db.SaveChangesAsync(cancellationToken);

        if (command.Mode == WriteThenFailMode.Throw)
        {
            throw new InvalidOperationException("Simulated unexpected fault after a write.");
        }

        if (command.Mode == WriteThenFailMode.ReturnFailure)
        {
            return Result<string>.Failure(Error.Rule("test.rule_broken", "Simulated business-rule failure after a write."));
        }

        return Result<string>.Success("ok");
    }
}
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
