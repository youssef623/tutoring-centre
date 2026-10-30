using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Infrastructure.Tests.Fixtures;

internal static class PortalReadScope
{
    public static async Task<T> ReadAsAsync<T>(this PostgresFixture fixture, Actor actor, Func<IServiceProvider, Task<T>> action)
    {
        await using var scope = fixture.Services.CreateAsyncScope();
        var services = scope.ServiceProvider;
        services.GetRequiredService<CurrentActorContext>().Set(actor);
        var transaction = services.GetRequiredService<IUnitOfWork>();
        await transaction.BeginAsync(readOnly: true, CancellationToken.None);
        try
        {
            var result = await action(services);
            await transaction.CommitAsync(CancellationToken.None);
            return result;
        }
        catch
        {
            await transaction.RollbackAsync(CancellationToken.None);
            throw;
        }
    }
}
