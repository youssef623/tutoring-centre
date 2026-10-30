using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Conversations.Contracts;
using TutoringCentre.Application.Identity.Contracts;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Assistant;
public sealed class PortalActorFactory(IPortalActorDirectory directory, ICurrentActor current)
{
    public async Task<Result<ICurrentActor>> ForConversationAsync(AssistantConversationInput conversation, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(conversation);
        var subject = conversation.ParentId ?? conversation.StudentId;
        if (current.CentreId is not Guid centre || conversation.PortalUserId is not Guid user || subject is not Guid id
            || !await directory.IsActiveAsync(user, id, conversation.StudentId is not null, ct))
            return Result<ICurrentActor>.Failure(Error.Forbidden("assistant.account_inactive", "The portal account is no longer active."));
        var actor = new CurrentActorContext();
        actor.Set(conversation.StudentId is not null ? new StudentActor(user, centre, id) : new ParentActor(user, centre, id));
        return Result<ICurrentActor>.Success(actor);
    }
}
