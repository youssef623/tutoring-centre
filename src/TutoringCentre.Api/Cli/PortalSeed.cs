using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TutoringCentre.Application.Academics.Parents;
using TutoringCentre.Application.Academics.Parents.Queries.GetParent;
using TutoringCentre.Application.Academics.Parents.Queries.LookupParentByPhone;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;
using TutoringCentre.Application.Identity.Portal.Queries.ListPortalAccounts;
using TutoringCentre.Domain.Centres;
using TutoringCentre.Infrastructure.Identity;
using TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Api.Cli;

/// <summary>Development-only demo accounts, created through the production portal use case.</summary>
internal static class PortalSeed
{
    internal static async Task<bool> RunAsync(IServiceProvider services, IReadOnlyDictionary<string, Guid> centres)
    {
        // Testing deliberately retains its established seed shape; production can never issue demo credentials.
        if (!services.GetRequiredService<IHostEnvironment>().IsDevelopment()) return true;
        var password = services.GetRequiredService<IConfiguration>()["Seed:Password"];
        if (string.IsNullOrWhiteSpace(password) || !centres.TryGetValue("nile-centre", out var centreId)) return false;

        foreach (var (slug, webCentreId) in centres)
        {
            var registered = await services.GetRequiredService<ISystemScopeRunner>().RunAsync(webCentreId, "seed-web-channel",
                (scopeServices, ct) => scopeServices.GetRequiredService<Dispatcher>().SendAsync<RegisterChannelAccountCommand, Guid>(
                    new(ChannelProvider.Web, "web:" + slug, null), ct));
            if (registered.IsFailure && registered.Error?.Code != "conversations.channel_taken") return false;
        }

        Guid ownerId;
        await using (var lookupScope = services.CreateAsyncScope())
        {
            var users = lookupScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var owner = await users.FindByEmailAsync("owner@nile.test");
            if (owner is null) return false;
            ownerId = owner.Id;
        }

        Guid parentId;
        Guid studentId;
        await using (var subjectScope = services.CreateAsyncScope())
        {
            InitializeScope(subjectScope.ServiceProvider, centreId, ownerId);
            var dispatcher = subjectScope.ServiceProvider.GetRequiredService<Dispatcher>();
            var lookup = await dispatcher.QueryAsync<LookupParentByPhoneQuery, ParentLookupDto>(new("01000000001"), CancellationToken.None);
            if (lookup.IsFailure || lookup.Value.Parent is null) return false;
            parentId = lookup.Value.Parent.Id;
            var parent = await dispatcher.QueryAsync<GetParentQuery, ParentDetailDto>(new(parentId), CancellationToken.None);
            if (parent.IsFailure || parent.Value.Children.Count == 0) return false;
            studentId = parent.Value.Children[0].StudentId;
        }

        foreach (var (kind, subjectId, email) in new[]
        {
            (PortalSubjectKind.Parent, parentId, "parent@nile.test"),
            (PortalSubjectKind.Student, studentId, "student@nile.test"),
        })
        {
            await using var accountScope = services.CreateAsyncScope();
            InitializeScope(accountScope.ServiceProvider, centreId, ownerId);
            var dispatcher = accountScope.ServiceProvider.GetRequiredService<Dispatcher>();
            var existing = await dispatcher.QueryAsync<ListPortalAccountsQuery, IReadOnlyList<PortalAccountDto>>(new(kind, subjectId), CancellationToken.None);
            if (existing.IsFailure) return false;
            if (existing.Value.Any(account => string.Equals(account.Email, email, StringComparison.OrdinalIgnoreCase))) continue;

            var created = await dispatcher.SendAsync<CreatePortalAccountCommand, CreatePortalAccountResult>(
                new(kind, subjectId, email, SupportedLocale.Ar), CancellationToken.None);
            if (created.IsFailure)
            {
                if (created.Error?.Code == "portal.subject_already_linked") continue;
                return false;
            }

            // Only a new account gets demo credentials. Existing users and their passwords are never reset.
            if (created.Value.TemporaryPassword is not { } temporaryPassword) continue;
            var users = accountScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var user = await users.FindByIdAsync(created.Value.UserId.ToString());
            if (user is null) return false;
            var changed = await users.ChangePasswordAsync(user, temporaryPassword, password);
            if (!changed.Succeeded) return false;
            user.MustChangePassword = false;
            if (!(await users.UpdateAsync(user)).Succeeded) return false;
        }

        return true;
    }

    private static void InitializeScope(IServiceProvider services, Guid centreId, Guid ownerId)
    {
        services.GetRequiredService<CurrentActorContext>().Set(new SystemActor(centreId, ownerId));
        services.GetRequiredService<CorrelationContext>().Set("seed-portal");
    }
}
