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
