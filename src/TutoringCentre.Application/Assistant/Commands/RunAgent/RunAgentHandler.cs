using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Assistant.Context;
using TutoringCentre.Application.Centres.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Ports;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Application.Common.Time;
using TutoringCentre.Application.Conversations.Contracts;
using TutoringCentre.Application.Assistant.Models;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Application.Common.Execution;
using TutoringCentre.Application.Assistant.Handles;
using TutoringCentre.Application.Assistant.Tools;
using TutoringCentre.Domain.Common;
namespace TutoringCentre.Application.Assistant.Commands.RunAgent;
[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class RunAgentHandler(IAssistantConversationGateway conversations, IAgentRunRepository runs,
    IPortalDirectory directory, ICentreTimeZoneProvider zones, IClock clock, ICurrentActor actor,
    AgentLoop loop, AssistantOptions options, PortalActorFactory actors, IActorScopeRunner scopes) : ICommandHandler<RunAgentCommand, RunAgentResult>
{
    public async Task<Result<RunAgentResult>> HandleAsync(RunAgentCommand command, CancellationToken cancellationToken)
    {
        if (actor.Actor is not SystemActor) return Result<RunAgentResult>.Failure(Error.Forbidden("assistant.system_only", "Only the system may run the assistant."));
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        // Reserve up to one second of the dispatcher budget for the fallback, save and commit.
        var workBudget = options.TotalRunTimeout - TimeSpan.FromTicks(Math.Min(TimeSpan.TicksPerSecond, options.TotalRunTimeout.Ticks / 10));
        timeout.CancelAfter(workBudget);
        var started = Stopwatch.GetTimestamp();
        var ct = timeout.Token;
        var inbound = await conversations.LoadAsync(command.InboundMessageId, ct);
        if (inbound is null) return Result<RunAgentResult>.Failure(Error.NotFound("assistant.inbound_not_found", "Message not found."));
        if (!inbound.CanReply)
            return Result<RunAgentResult>.Failure(Error.Rule("assistant.conversation_unavailable", "The conversation cannot receive assistant replies."));
        var run = await runs.TryStartAsync(inbound.ConversationId, command.InboundMessageId, clock.UtcNow, ct);
        if (run is null) return Result<RunAgentResult>.Success(new(true));
        var portalActor = await actors.ForConversationAsync(inbound, ct);
        if (portalActor.IsFailure) return Result<RunAgentResult>.Failure(portalActor.Error!);
        var handles = await scopes.RunAsync(portalActor.Value, provider => provider.GetRequiredService<Dispatcher>()
            .QueryAsync<GetRunHandlesQuery, HandleTable>(new(), ct));
        if (handles.IsFailure) return Result<RunAgentResult>.Failure(handles.Error!);
        run.SetPromptVersion(AssistantPrompt.Version);
        var identity = inbound.ParentId is Guid parent ? await directory.GetParentAsync(parent, ct)
            : inbound.StudentId is Guid student ? await directory.GetStudentAsync(student, ct) : null;
        if (identity is null || !identity.IsActive) return Result<RunAgentResult>.Failure(Error.Rule("assistant.identity_unavailable", "Portal identity is unavailable."));
        var zone = await zones.GetForCurrentCentreAsync(ct);
        var history = await conversations.GetHistoryAsync(command.InboundMessageId, options.HistoryMessages, ct);
        var context = ContextBuilder.Build(new(identity.DisplayName, inbound.ParentId is not null ? "parent" : "student",
            CentreCalendar.Today(clock, zone), zone.Id, history.Select(x => new ModelMessage(x.IsAssistant ? ModelRole.Assistant : ModelRole.User, x.Text)).ToArray(), inbound.Text, handles.Value), options.HistoryMessages);
        var outcome = await loop.RunWithinBudgetAsync(context, workBudget - Stopwatch.GetElapsedTime(started), cancellationToken,
            new ToolContext(run.CentreId, run.Id, command.InboundMessageId, handles.Value));
        cancellationToken.ThrowIfCancellationRequested();
        var reply = outcome.Reply ?? (IsArabic(inbound.Text) ? "\u0639\u0630\u0631\u0627\u064b\u060c \u0644\u0627 \u0623\u0633\u062a\u0637\u064a\u0639 \u0627\u0644\u0631\u062f \u0627\u0644\u0622\u0646. \u062d\u0627\u0648\u0644 \u0645\u0631\u0629 \u0623\u062e\u0631\u0649 \u0642\u0631\u064a\u0628\u0627\u064b." : "Sorry, I cannot reply right now. Please try again soon.");
        var finished = outcome.FailureCode is null
            ? run.Complete(outcome.Steps, (int)outcome.Usage.InputTokens, (int)outcome.Usage.OutputTokens, clock.UtcNow, outcome.GuardReason)
            : run.Fail(outcome.FailureCode, outcome.Steps, (int)outcome.Usage.InputTokens, (int)outcome.Usage.OutputTokens, clock.UtcNow);
        if (finished.IsFailure) return Result<RunAgentResult>.Failure(finished.Error!);
        var queued = await conversations.QueueReplyAsync(command.InboundMessageId, reply, cancellationToken);
        if (queued.IsFailure) return Result<RunAgentResult>.Failure(queued.Error!);
        return Result<RunAgentResult>.Success(new(false));
    }
    private static bool IsArabic(string? text) => text?.Any(c => c is >= '\u0600' and <= '\u06ff') == true;
}
