using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Centres;
using TutoringCentre.Application.Centres.Commands.UpdateCentreSettings;
using TutoringCentre.Application.Centres.Queries.GetCentreSettings;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Centres;

/// <summary>
/// Through the real dispatcher: Secretary and Teacher are refused both centre-settings requests
/// (auth.permission_denied), and a centre-less actor is refused before permission is even considered
/// (tenant.not_selected) — the same two pipeline guarantees proved for Audit, Staff and Subjects.
/// </summary>
public sealed class CentreSettingsDispatcherAuthorizationTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task QueryAsync_GetCentreSettings_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.QueryAsync<GetCentreSettingsQuery, CentreSettingsDto>(
            new GetCentreSettingsQuery(), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task SendAsync_UpdateCentreSettings_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.SendAsync<UpdateCentreSettingsCommand, Unit>(
            new UpdateCentreSettingsCommand("New Name", SupportedLocale.En, 0), CancellationToken.None);

        AssertPermissionDenied(result);
    }

    [Fact]
    public async Task QueryAsync_GetCentreSettings_CentrelessActor_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut(role: null);

        var result = await fixture.Dispatcher.QueryAsync<GetCentreSettingsQuery, CentreSettingsDto>(
            new GetCentreSettingsQuery(), CancellationToken.None);

        AssertTenantNotSelected(result);
    }

    [Fact]
    public async Task SendAsync_UpdateCentreSettings_CentrelessActor_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut(role: null);

        var result = await fixture.Dispatcher.SendAsync<UpdateCentreSettingsCommand, Unit>(
            new UpdateCentreSettingsCommand("New Name", SupportedLocale.En, 0), CancellationToken.None);

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

    private static Fixture CreateSut(StaffRole? role)
    {
        var actor = role is null
            ? new StaffActor(Guid.NewGuid(), CentreId: null, Role: null)
            : new StaffActor(Guid.NewGuid(), CentreId, role);
        var currentActor = new FakeCurrentActor { Actor = actor };

        var provider = new ServiceCollection()
            .AddSingleton(new FakeUnitOfWork())
            .AddSingleton<ICurrentActor>(currentActor)
            .AddSingleton<ICentreRepository>(new FakeCentreRepository())
            .AddSingleton<ICentreReadService>(new FakeCentreReadService())
            .AddScoped<ICommandHandler<UpdateCentreSettingsCommand, Unit>, UpdateCentreSettingsHandler>()
            .AddScoped<IQueryHandler<GetCentreSettingsQuery, CentreSettingsDto>, GetCentreSettingsHandler>()
            .AddScoped<IValidator<UpdateCentreSettingsCommand>, UpdateCentreSettingsValidator>()
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
