using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Staff;
using TutoringCentre.Application.Staff.Commands.ChangeStaffRole;
using TutoringCentre.Application.Staff.Commands.CreateStaff;
using TutoringCentre.Application.Staff.Commands.DeactivateStaff;
using TutoringCentre.Application.Staff.Commands.ReactivateStaff;
using TutoringCentre.Application.Staff.Queries.ListStaff;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Staff;

/// <summary>
/// Task 29.11: through the real dispatcher, Secretary and Teacher are refused all five staff requests
/// (auth.permission_denied), and a centre-less actor is refused all five before permission is even considered
/// (tenant.not_selected) — the same two pipeline guarantees Day 28/22 already proved for Subjects.
/// </summary>
public sealed class StaffDispatcherAuthorizationTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_CreateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<CreateStaffCommand, CreateStaffResult>(
            new CreateStaffCommand("new@nile.test", "New Person", StaffRole.Teacher, "en"), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_ChangeStaffRole_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<ChangeStaffRoleCommand, Unit>(
            new ChangeStaffRoleCommand(Guid.CreateVersion7(), StaffRole.Teacher, 0), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_DeactivateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<DeactivateStaffCommand, Unit>(
            new DeactivateStaffCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_ReactivateStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<ReactivateStaffCommand, Unit>(
            new ReactivateStaffCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task QueryAsync_ListStaff_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.QueryAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(
            new ListStaffQuery(), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Fact]
    public async Task SendAsync_CreateStaff_CentrelessActor_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut(role: null);

        var result = await fixture.Dispatcher.SendAsync<CreateStaffCommand, CreateStaffResult>(
            new CreateStaffCommand("new@nile.test", "New Person", StaffRole.Teacher, "en"), CancellationToken.None);

        AssertTenantNotSelected(result);
    }

    [Fact]
    public async Task SendAsync_ChangeStaffRole_CentrelessActor_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut(role: null);

        var result = await fixture.Dispatcher.SendAsync<ChangeStaffRoleCommand, Unit>(
            new ChangeStaffRoleCommand(Guid.CreateVersion7(), StaffRole.Teacher, 0), CancellationToken.None);

        AssertTenantNotSelected(result);
    }

    [Fact]
    public async Task SendAsync_DeactivateStaff_CentrelessActor_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut(role: null);

        var result = await fixture.Dispatcher.SendAsync<DeactivateStaffCommand, Unit>(
            new DeactivateStaffCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertTenantNotSelected(result);
    }

    [Fact]
    public async Task SendAsync_ReactivateStaff_CentrelessActor_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut(role: null);

        var result = await fixture.Dispatcher.SendAsync<ReactivateStaffCommand, Unit>(
            new ReactivateStaffCommand(Guid.CreateVersion7(), 0), CancellationToken.None);

        AssertTenantNotSelected(result);
    }

    [Fact]
    public async Task QueryAsync_ListStaff_CentrelessActor_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut(role: null);

        var result = await fixture.Dispatcher.QueryAsync<ListStaffQuery, IReadOnlyList<StaffMemberDto>>(
            new ListStaffQuery(), CancellationToken.None);

        AssertTenantNotSelected(result);
    }

    private static void AssertPermissionDenied<T>(Result<T> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("auth.permission_denied", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
    }

    private static void AssertTenantNotSelected<T>(Result<T> result)
    {
        Assert.True(result.IsFailure);
        Assert.Equal("tenant.not_selected", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
    }

    /// <summary>role: null produces a centre-less StaffActor; otherwise a StaffActor with that role in <see cref="CentreId"/>.</summary>
    private static Fixture CreateSut(StaffRole? role)
    {
        var actor = role is null
            ? new StaffActor(Guid.NewGuid(), CentreId: null, Role: null)
            : new StaffActor(Guid.NewGuid(), CentreId, role);
        var currentActor = new FakeCurrentActor { Actor = actor };

        var provider = new ServiceCollection()
            .AddSingleton(new FakeUnitOfWork())
            .AddSingleton<ICurrentActor>(currentActor)
            .AddSingleton<IMembershipRepository>(new FakeMembershipRepository())
            .AddSingleton<IStaffAccountService>(new FakeStaffAccountService())
            .AddSingleton<IStaffReadService>(new FakeStaffReadService())
            .AddScoped<ICommandHandler<CreateStaffCommand, CreateStaffResult>, CreateStaffHandler>()
            .AddScoped<ICommandHandler<ChangeStaffRoleCommand, Unit>, ChangeStaffRoleHandler>()
            .AddScoped<ICommandHandler<DeactivateStaffCommand, Unit>, DeactivateStaffHandler>()
            .AddScoped<ICommandHandler<ReactivateStaffCommand, Unit>, ReactivateStaffHandler>()
            .AddScoped<IQueryHandler<ListStaffQuery, IReadOnlyList<StaffMemberDto>>, ListStaffHandler>()
            .AddScoped<IValidator<CreateStaffCommand>, CreateStaffValidator>()
            .AddScoped<IValidator<ChangeStaffRoleCommand>, ChangeStaffRoleValidator>()
            .AddScoped<IValidator<DeactivateStaffCommand>, DeactivateStaffValidator>()
            .AddScoped<IValidator<ReactivateStaffCommand>, ReactivateStaffValidator>()
            .BuildServiceProvider();

        var unitOfWork = provider.GetRequiredService<FakeUnitOfWork>();
        return new Fixture(new Dispatcher(provider, unitOfWork, NullLogger<Dispatcher>.Instance), provider);
    }

    private sealed class Fixture(Dispatcher dispatcher, ServiceProvider provider) : IDisposable
    {
        public Dispatcher Dispatcher { get; } = dispatcher;

        public void Dispose() => provider.Dispose();
    }
}
