using Hangfire.Server;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Billing.Commands.CreateMonthlyCharges;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
namespace TutoringCentre.Infrastructure.Jobs;
public sealed class CreateCentreChargesJob(ISystemScopeRunner runner)
{
    public async Task ExecuteAsync(Guid centreId, PerformContext? context, CancellationToken cancellationToken)
    {
        var result = await runner.RunAsync(centreId, "job:" + (context?.BackgroundJob.Id ?? "direct"), (services, ct) =>
            services.GetRequiredService<Dispatcher>().SendAsync<CreateMonthlyChargesCommand, CreateMonthlyChargesResult>(new(), ct), cancellationToken);
        if (result.IsFailure) throw new InvalidOperationException(result.Error!.Code);
    }
}
