using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Api.Cli;

/// <summary>Operator-only channel registration through the normal tenant command pipeline.</summary>
public static class ChannelsCommand
{
    public static async Task<int> RunAsync(IServiceProvider services, string[] args)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(args);
        if (args.Length != 9 || args[0] != "register") return 1;
        var options = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var i = 1; i < args.Length; i += 2)
            if (!options.TryAdd(args[i], args[i + 1])) return 1;
        if (!options.TryGetValue("--centre", out var slug) || !options.TryGetValue("--provider", out var provider)
            || provider != "whatsapp" || !options.TryGetValue("--account-id", out var accountId)
            || !options.TryGetValue("--display-phone", out var phone)) return 1;
        await using var scope = services.CreateAsyncScope();
        var centreId = await scope.ServiceProvider.GetRequiredService<AppDbContext>().Set<Centre>()
            .Where(x => x.Slug == slug).Select(x => (Guid?)x.Id).SingleOrDefaultAsync();
        if (centreId is null) return 1;
        var result = await services.GetRequiredService<ISystemScopeRunner>().RunAsync(centreId, "channels:register",
            (scopedServices, ct) => scopedServices.GetRequiredService<Dispatcher>().SendAsync<RegisterChannelAccountCommand, Guid>(
                new(ChannelProvider.Whatsapp, accountId, phone), ct));
        if (result.IsFailure) Console.Error.WriteLine(result.Error!.Code);
        return result.IsSuccess ? 0 : 1;
    }
}
