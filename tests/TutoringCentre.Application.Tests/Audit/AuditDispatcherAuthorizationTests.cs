using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using TutoringCentre.Application.Audit;
using TutoringCentre.Application.Audit.Queries.GetAuditLog;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Tests.Fakes;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Tests.Audit;

/// <summary>
/// Through the real dispatcher: Secretary and Teacher are refused (auth.permission_denied), and a centre-less
/// actor is refused before permission is even considered (tenant.not_selected) — the same two pipeline
/// guarantees proved for Staff and Subjects.
/// </summary>
public sealed class AuditDispatcherAuthorizationTests
{
    private static readonly Guid CentreId = Guid.CreateVersion7();

    [Theory]
    [InlineData(StaffRole.Secretary)]
    [InlineData(StaffRole.Teacher)]
    public async Task QueryAsync_GetAuditLog_ReturnsPermissionDeniedForSecretaryAndTeacher(StaffRole role)
    {
        using var fixture = CreateSut(role);

        var result = await fixture.Dispatcher.QueryAsync<GetAuditLogQuery, AuditPageDto>(
            new GetAuditLogQuery(null, null, null, null, null, null, 50), CancellationToken.None);

        Assert.True(result.IsFailure);
        Assert.Equal("auth.permission_denied", result.Error!.Code);
        Assert.Equal(ErrorKind.Forbidden, result.Error.Kind);
    }

    [Fact]
    public async Task QueryAsync_GetAuditLog_CentrelessActor_ReturnsTenantNotSelected()
    {
        using var fixture = CreateSut(role: null);

        var result = await fixture.Dispatcher.QueryAsync<GetAuditLogQuery, AuditPageDto>(
            new GetAuditLogQuery(null, null, null, null, null, null, 50), CancellationToken.None);

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
            .AddSingleton<IAuditReadService>(new FakeAuditReadService())
            .AddScoped<IQueryHandler<GetAuditLogQuery, AuditPageDto>, GetAuditLogHandler>()
            .AddScoped<IValidator<GetAuditLogQuery>, GetAuditLogValidator>()
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
