using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Application.Tests.Cqrs;

public sealed class DispatcherTests
{
    private static readonly TestCommand ValidCommand = new("Nile", "nile");

    [Fact]
    public async Task SendAsync_ValidCommandHandlerSucceeds_BeginsHandlesSavesAndCommits()
    {
        // Arrange
        using var fixture = CreateSut();

        // Act
        var result = await fixture.Dispatcher.SendAsync<TestCommand, string>(ValidCommand, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(["Begin(rw)", "Handle", "Save", "Commit"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_InvalidCommand_DoesNotBeginOrCallHandler()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.SendAsync<TestCommand, string>(new TestCommand("", "nile"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("validation.failed", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
        Assert.NotNull(result.Error.Fields);
        Assert.Contains("name", result.Error.Fields.Keys);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_HandlerReturnsFailure_RollsBackWithoutSavingOrCommitting()
    {
        using var fixture = CreateSut(HandlerMode.Fail);

        var result = await fixture.Dispatcher.SendAsync<TestCommand, string>(ValidCommand, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(TestErrors.HandlerFailure, result.Error);
        Assert.Equal(["Begin(rw)", "Handle", "Rollback"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_HandlerThrows_RollsBackAndRethrows()
    {
        using var fixture = CreateSut(HandlerMode.Throw);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Dispatcher.SendAsync<TestCommand, string>(ValidCommand, CancellationToken.None));

        Assert.Equal(["Begin(rw)", "Handle", "Rollback"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_SaveChangesThrows_RollsBackAndDoesNotCommit()
    {
        using var fixture = CreateSut();
        fixture.UnitOfWork.ThrowOnSave = true;

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Dispatcher.SendAsync<TestCommand, string>(ValidCommand, CancellationToken.None));

        Assert.Equal(["Begin(rw)", "Handle", "Save", "Rollback"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task QueryAsync_ValidQuery_BeginsReadOnlyHandlesAndCommitsWithoutSaving()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.QueryAsync<TestQuery, string>(new TestQuery("Nile"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(["Begin(ro)", "Handle", "Commit"], fixture.UnitOfWork.Calls);
        Assert.DoesNotContain("Save", fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task QueryAsync_InvalidQuery_DoesNotBeginOrCallHandler()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.QueryAsync<TestQuery, string>(new TestQuery(""), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("validation.failed", result.Error!.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task QueryAsync_HandlerThrows_RollsBackAndRethrows()
    {
        using var fixture = CreateSut(HandlerMode.Throw);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            fixture.Dispatcher.QueryAsync<TestQuery, string>(new TestQuery("Nile"), CancellationToken.None));

        Assert.Equal(["Begin(ro)", "Handle", "Rollback"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_SeveralFieldFailures_GroupsMessagesByCamelCaseFieldName()
    {
        using var fixture = CreateSut();

        var result = await fixture.Dispatcher.SendAsync<TestCommand, string>(new TestCommand("", "far-too-long"), CancellationToken.None);

        var fields = result.Error!.Fields!;
        Assert.Equal(["name", "slug"], fields.Keys.Order(StringComparer.Ordinal));
        Assert.Equal(2, fields["name"].Length);
        Assert.Equal(["Slug is too long."], fields["slug"]);
    }

    private static Fixture CreateSut(HandlerMode mode = HandlerMode.Succeed)
    {
        var unitOfWork = new FakeUnitOfWork();
        var behaviour = new HandlerBehaviour { Mode = mode };
        var provider = new ServiceCollection()
            .AddSingleton(unitOfWork)
            .AddSingleton(behaviour)
            .AddScoped<ICommandHandler<TestCommand, string>, TestCommandHandler>()
            .AddScoped<IQueryHandler<TestQuery, string>, TestQueryHandler>()
            .AddScoped<IValidator<TestCommand>, TestCommandValidator>()
            .AddScoped<IValidator<TestQuery>, TestQueryValidator>()
            .BuildServiceProvider();

        return new Fixture(new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance), unitOfWork, provider);
    }

    private sealed class Fixture(Dispatcher dispatcher, FakeUnitOfWork unitOfWork, ServiceProvider provider) : IDisposable
    {
        public Dispatcher Dispatcher { get; } = dispatcher;

        public FakeUnitOfWork UnitOfWork { get; } = unitOfWork;

        public void Dispose() => provider.Dispose();
    }
}
