using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Conversations;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;

public sealed record RegisterChannelAccountCommand(ChannelProvider Provider, string ExternalAccountId, string DisplayPhone)
    : ICommand<Guid>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.ConversationsIngest;
}
