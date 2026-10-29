using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Conversations;

namespace TutoringCentre.Application.Conversations.Commands.RegisterChannelAccount;

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class RegisterChannelAccountHandler(IChannelAccountRepository accounts, ICurrentActor actor)
    : ICommandHandler<RegisterChannelAccountCommand, Guid>
{
    public Task<Result<Guid>> HandleAsync(RegisterChannelAccountCommand command, CancellationToken cancellationToken)
    {
        var phone = PhoneNumber.Create(command.DisplayPhone);
        if (phone.IsFailure) return Task.FromResult(Result<Guid>.Failure(phone.Error!));
        var account = ChannelAccount.Register(actor.Actor.CentreId!.Value, command.Provider, command.ExternalAccountId, phone.Value);
        if (account.IsFailure) return Task.FromResult(Result<Guid>.Failure(account.Error!));
        accounts.Add(account.Value);
        return Task.FromResult(Result<Guid>.Success(account.Value.Id));
    }
}
