using Hangfire;
using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Security;
namespace TutoringCentre.Infrastructure.Jobs;
public sealed class CreateChargesFanOutJob(ISystemScopeRunner runner, IBackgroundJobClient client)
{
    public Task ExecuteAsync(PerformContext? context, CancellationToken cancellationToken) => runner.RunAsync(null, "job:" + (context?.BackgroundJob.Id ?? "direct"), async (services, ct) =>
    {
        var centres = await services.GetRequiredService<ICentreDirectory>().ListCentreIdsAsync(ct);
        foreach (var id in centres) { ct.ThrowIfCancellationRequested(); client.Enqueue<CreateCentreChargesJob>(job => job.ExecuteAsync(id, null, CancellationToken.None)); }
        return centres.Count;
    }, cancellationToken);
}
