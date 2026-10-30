using FluentValidation;

namespace TutoringCentre.Application.Conversations.Commands.RecordInboundMessage;

internal sealed class RecordInboundMessageValidator : AbstractValidator<RecordInboundMessageCommand>
{
    public RecordInboundMessageValidator()
    {
        RuleFor(x => x.ChannelAccountId).NotEmpty().WithErrorCode("conversations.message_invalid");
        RuleFor(x => x.CounterpartAddress).NotEmpty().WithErrorCode("conversations.message_invalid")
            .Matches("^[0-9]{1,20}\\z").WithErrorCode("conversations.message_invalid");
        RuleFor(x => x.ProviderMessageId).NotEmpty().WithErrorCode("conversations.message_invalid")
            .Matches("^[ -~]{1,128}\\z").WithErrorCode("conversations.message_invalid");
        RuleFor(x => x.Type).IsInEnum().WithErrorCode("conversations.message_invalid");
        // Text rules belong to InboundMessage.Receive, including Unicode character length.
    }
}
