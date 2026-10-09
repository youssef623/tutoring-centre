using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

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
    public async Task SendAsync_SaveFails_RollsBackWithoutCommittingAndReturnsTheSaveError()
    {
        // C14
        using var fixture = CreateSut();
        var saveError = Error.Conflict("test.save_conflict", "Someone else changed this first.");
        fixture.UnitOfWork.SaveFailure = saveError;

        var result = await fixture.Dispatcher.SendAsync<TestCommand, string>(ValidCommand, CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal(saveError, result.Error);
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

    [Fact]
    public async Task SendAsync_TenantScopedCommandStaffActorWithoutCentre_ReturnsTenantNotSelected()
    {
        // C10
        var actor = new StaffActor(Guid.NewGuid(), CentreId: null, Role: null);
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.SendAsync<TenantScopedTestCommand, string>(
            new TenantScopedTestCommand("Nile"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("tenant.not_selected", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task QueryAsync_TenantScopedQueryStaffActorWithoutCentre_ReturnsTenantNotSelected()
    {
        // C11
        var actor = new StaffActor(Guid.NewGuid(), CentreId: null, Role: null);
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.QueryAsync<TenantScopedTestQuery, string>(
            new TenantScopedTestQuery("Nile"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("tenant.not_selected", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_TenantScopedCommandAnonymousActor_ReturnsNotAuthenticated()
    {
        // C12 (command)
        using var fixture = CreateSut(actor: new AnonymousActor());

        var result = await fixture.Dispatcher.SendAsync<TenantScopedTestCommand, string>(
            new TenantScopedTestCommand("Nile"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("auth.not_authenticated", result.Error!.Code);
        Assert.Equal(ErrorKind.Unauthenticated, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task QueryAsync_TenantScopedQueryAnonymousActor_ReturnsNotAuthenticated()
    {
        // C12 (query)
        using var fixture = CreateSut(actor: new AnonymousActor());

        var result = await fixture.Dispatcher.QueryAsync<TenantScopedTestQuery, string>(
            new TenantScopedTestQuery("Nile"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("auth.not_authenticated", result.Error!.Code);
        Assert.Equal(ErrorKind.Unauthenticated, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_TenantScopedCommandActorWithCentre_InvokesHandler()
    {
        // C13 (tenant-scoped request with a centre)
        var actor = new StaffActor(Guid.NewGuid(), Guid.NewGuid(), StaffRole.Owner);
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.SendAsync<TenantScopedTestCommand, string>(
            new TenantScopedTestCommand("Nile"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(["Begin(rw)", "Handle", "Save", "Commit"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task QueryAsync_TenantScopedQueryActorWithCentre_InvokesHandler()
    {
        // C13 (tenant-scoped request with a centre)
        var actor = new StaffActor(Guid.NewGuid(), Guid.NewGuid(), StaffRole.Owner);
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.QueryAsync<TenantScopedTestQuery, string>(
            new TenantScopedTestQuery("Nile"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(["Begin(ro)", "Handle", "Commit"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_NonScopedRequestActorWithoutCentre_InvokesHandler()
    {
        // C13 (non-scoped request without a centre is unaffected)
        var actor = new StaffActor(Guid.NewGuid(), CentreId: null, Role: null);
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.SendAsync<TestCommand, string>(ValidCommand, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(["Begin(rw)", "Handle", "Save", "Commit"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_TenantScopedCommandInvalidAndActorWithoutCentre_ValidationWinsOverTenantStep()
    {
        // Ordering: validation runs before the tenant step, even when both would fail.
        var actor = new StaffActor(Guid.NewGuid(), CentreId: null, Role: null);
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.SendAsync<TenantScopedTestCommand, string>(
            new TenantScopedTestCommand(""), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("validation.failed", result.Error!.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_StaffActorWithThePermission_InvokesHandler()
    {
        // C15
        var actor = new StaffActor(Guid.NewGuid(), Guid.NewGuid(), StaffRole.Secretary);
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.SendAsync<PermissionRequiringTestCommand, string>(
            new PermissionRequiringTestCommand("Nile"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(["Begin(rw)", "Handle", "Save", "Commit"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_StaffActorWithoutThePermission_ReturnsPermissionDenied()
    {
        // C16 (command)
        var actor = new StaffActor(Guid.NewGuid(), Guid.NewGuid(), StaffRole.Teacher);
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.SendAsync<PermissionRequiringTestCommand, string>(
            new PermissionRequiringTestCommand("Nile"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("auth.permission_denied", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task QueryAsync_StaffActorWithoutThePermission_ReturnsPermissionDenied()
    {
        // C16 (query variant)
        var actor = new StaffActor(Guid.NewGuid(), Guid.NewGuid(), StaffRole.Teacher);
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.QueryAsync<PermissionRequiringTestQuery, string>(
            new PermissionRequiringTestQuery("Nile"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("auth.permission_denied", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_SystemActor_IsAllowedRegardlessOfPermission()
    {
        // C17
        var actor = new SystemActor(Guid.NewGuid());
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.SendAsync<PermissionRequiringTestCommand, string>(
            new PermissionRequiringTestCommand("Nile"), CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(["Begin(rw)", "Handle", "Save", "Commit"], fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task SendAsync_PermissionRequiringCommandActorWithoutCentre_TenantStepWins()
    {
        // C18
        var actor = new StaffActor(Guid.NewGuid(), CentreId: null, Role: null);
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.SendAsync<PermissionRequiringTestCommand, string>(
            new PermissionRequiringTestCommand("Nile"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("tenant.not_selected", result.Error!.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    [Fact]
    public async Task QueryAsync_PermissionRequiringQueryActorWithoutCentre_TenantStepWins()
    {
        // C19
        var actor = new StaffActor(Guid.NewGuid(), CentreId: null, Role: null);
        using var fixture = CreateSut(actor: actor);

        var result = await fixture.Dispatcher.QueryAsync<PermissionRequiringTestQuery, string>(
            new PermissionRequiringTestQuery("Nile"), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("tenant.not_selected", result.Error!.Code);
        Assert.Empty(fixture.UnitOfWork.Calls);
    }

    private static Fixture CreateSut(HandlerMode mode = HandlerMode.Succeed, Actor? actor = null)
    {
        var unitOfWork = new FakeUnitOfWork();
        var behaviour = new HandlerBehaviour { Mode = mode };
        var currentActor = new FakeCurrentActor();
        if (actor is not null)
        {
            currentActor.Actor = actor;
        }

        var provider = new ServiceCollection()
            .AddSingleton(unitOfWork)
            .AddSingleton(behaviour)
            .AddSingleton<ICurrentActor>(currentActor)
            .AddScoped<ICommandHandler<TestCommand, string>, TestCommandHandler>()
            .AddScoped<IQueryHandler<TestQuery, string>, TestQueryHandler>()
            .AddScoped<IValidator<TestCommand>, TestCommandValidator>()
            .AddScoped<IValidator<TestQuery>, TestQueryValidator>()
            .AddScoped<ICommandHandler<TenantScopedTestCommand, string>, TenantScopedTestCommandHandler>()
            .AddScoped<IQueryHandler<TenantScopedTestQuery, string>, TenantScopedTestQueryHandler>()
            .AddScoped<IValidator<TenantScopedTestCommand>, TenantScopedTestCommandValidator>()
            .AddScoped<IValidator<TenantScopedTestQuery>, TenantScopedTestQueryValidator>()
            .AddScoped<ICommandHandler<PermissionRequiringTestCommand, string>, PermissionRequiringTestCommandHandler>()
            .AddScoped<IQueryHandler<PermissionRequiringTestQuery, string>, PermissionRequiringTestQueryHandler>()
            .AddScoped<IValidator<PermissionRequiringTestCommand>, PermissionRequiringTestCommandValidator>()
            .AddScoped<IValidator<PermissionRequiringTestQuery>, PermissionRequiringTestQueryValidator>()
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
