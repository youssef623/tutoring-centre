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
        services.AddScoped<TutoringCentre.Application.Common.Execution.IActorScopeRunner, TutoringCentre.Application.Common.Execution.ActorScopeRunner>();
        services.AddScoped<TutoringCentre.Application.Assistant.PortalActorFactory>();
        TutoringCentre.Application.Assistant.Tools.ToolRegistration.AddTool<TutoringCentre.Application.Assistant.Tools.GetAttendance.GetAttendanceTool>(services);
        services.AddScoped<TutoringCentre.Application.Assistant.Contracts.IAssistantCalendar, TutoringCentre.Application.Assistant.AssistantCalendar>();
        services.AddScoped<TutoringCentre.Application.Assistant.Tools.ToolRegistry>();
        services.AddScoped<TutoringCentre.Application.Assistant.Tools.ToolGateway>();

        services.AddCqrsHandlers(typeof(AssemblyMarker).Assembly);
        services.AddScoped<Dispatcher>();
        services.AddScoped<ICommandTimeoutPolicy, TutoringCentre.Application.Assistant.AgentCommandTimeoutPolicy>();
        services.AddOutbox();
        services.AddOutboxMessage<TutoringCentre.Application.Academics.Contracts.EnrollmentStarted, TutoringCentre.Application.Billing.Commands.CreateEnrollmentCharge.CreateEnrollmentChargeCommand>(message => new(message.EnrollmentId));
        services.AddOutboxMessage<InboundMessageReceived, TutoringCentre.Application.Conversations.Inbound.Commands.ProcessInboundMessage.ProcessInboundMessageCommand>(message => new(message.MessageId));
        services.AddOutboxMessage<TutoringCentre.Application.Assistant.Commands.RunAgent.RunAgentRequested, TutoringCentre.Application.Assistant.Commands.RunAgent.RunAgentCommand>(message => new(message.InboundMessageId), "assistant.conversation_busy");
        services.AddOutboxMessage<SendOutboundMessageRequested, SendOutboundMessageCommand>(message => new(message.OutboundMessageId));
        services.AddOutboxMessage<DeliveryStatusRecorded, RecordDeliveryStatusCommand>(message => new(message.OutboundMessageId, message.Status));
        services.AddScoped<ChannelSenderRegistry>();
        services.AddScoped<TutoringCentre.Application.Conversations.Contracts.IAssistantConversationGateway, AssistantConversationGateway>();
        services.AddScoped<ITeachingScope, TeachingScopeAdapter>();

        return services;
    }
}
