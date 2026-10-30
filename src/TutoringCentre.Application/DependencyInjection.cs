using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Academics;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Common.Messaging;
using TutoringCentre.Application.Conversations.Outbound;
using TutoringCentre.Application.Conversations.Outbound.Commands.SendOutboundMessage;
using TutoringCentre.Application.Conversations.Outbound.Commands.RecordDeliveryStatus;
using TutoringCentre.Application.Conversations;

namespace TutoringCentre.Application;

/// <summary>Registers the Application layer's services. Called once from the Api's composition root.</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        // One actor context per scope (HTTP request or job). ICurrentActor resolves to the SAME instance, read-only.
        services.AddScoped<CurrentActorContext>();
        services.AddScoped<ICurrentActor>(provider => provider.GetRequiredService<CurrentActorContext>());

        // One correlation context per scope. ICorrelationContext resolves to the SAME instance, read-only.
        services.AddScoped<CorrelationContext>();
        services.AddScoped<ICorrelationContext>(provider => provider.GetRequiredService<CorrelationContext>());
        services.AddSingleton<ISystemScopeRunner, SystemScopeRunner>();

        services.AddCqrsHandlers(typeof(AssemblyMarker).Assembly);
        services.AddScoped<Dispatcher>();
        services.AddScoped<ICommandTimeoutPolicy, TutoringCentre.Application.Assistant.AgentCommandTimeoutPolicy>();
        services.AddOutbox();
        services.AddOutboxMessage<InboundMessageReceived, TutoringCentre.Application.Conversations.Inbound.Commands.ProcessInboundMessage.ProcessInboundMessageCommand>(message => new(message.MessageId));
        services.AddOutboxMessage<TutoringCentre.Application.Assistant.Commands.RunAgent.RunAgentRequested, TutoringCentre.Application.Assistant.Commands.RunAgent.RunAgentCommand>(message => new(message.InboundMessageId));
        services.AddOutboxMessage<SendOutboundMessageRequested, SendOutboundMessageCommand>(message => new(message.OutboundMessageId));
        services.AddOutboxMessage<DeliveryStatusRecorded, RecordDeliveryStatusCommand>(message => new(message.OutboundMessageId, message.Status));
        services.AddScoped<ChannelSenderRegistry>();
        services.AddScoped<TutoringCentre.Application.Conversations.Contracts.IAssistantConversationGateway, AssistantConversationGateway>();
        services.AddScoped<ITeachingScope, TeachingScopeAdapter>();

        return services;
    }
}
