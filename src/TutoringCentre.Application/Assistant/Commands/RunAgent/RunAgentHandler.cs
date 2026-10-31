using System.Diagnostics;
using TutoringCentre.Application.Assistant.Guards;
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
    AgentLoop loop, AssistantOptions options, PortalActorFactory actors, IActorScopeRunner scopes, IConversationLock conversationLock) : ICommandHandler<RunAgentCommand, RunAgentResult>
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
        if (actor.CentreId is not Guid centre || !await conversationLock.TryAcquireAsync(centre, inbound.ConversationId, ct))
            return Result<RunAgentResult>.Failure(Error.Conflict("assistant.conversation_busy", "The conversation is busy. Retry shortly."));
        inbound = await conversations.LoadAsync(command.InboundMessageId, ct) ?? inbound;
        if (inbound.HasEarlierPending)
            return Result<RunAgentResult>.Failure(Error.Conflict("assistant.conversation_busy", "An earlier message is still pending."));
        if (!inbound.CanReply)
            return Result<RunAgentResult>.Failure(Error.Rule("assistant.conversation_unavailable", "The conversation cannot receive assistant replies."));
        var run = await runs.TryStartAsync(inbound.ConversationId, command.InboundMessageId, clock.UtcNow, ct);
        if (run is null) return Result<RunAgentResult>.Success(new(true));
        var portalActor = await actors.ForConversationAsync(inbound, ct);
        if (portalActor.IsFailure)
        {
            var failed = run.Fail("assistant.account_inactive", 0, 0, 0, clock.UtcNow);
            return failed.IsFailure ? Result<RunAgentResult>.Failure(failed.Error!) : Result<RunAgentResult>.Success(new(false));
        }
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
        var toolContext = new ToolContext(run.CentreId, run.Id, command.InboundMessageId, handles.Value);
        var outcome = await loop.RunWithinBudgetAsync(context, workBudget - Stopwatch.GetElapsedTime(started), cancellationToken,
            toolContext);
        cancellationToken.ThrowIfCancellationRequested();
        var reply = outcome.Reply ?? FallbackReplies.For(inbound.Text);
        var names = await directory.ListStudentNamesAsync(cancellationToken);
        var verdict = OutputGuards.Check(reply, inbound.Text, handles.Value.Children.Values.Select(child => child.Name).ToArray(),
            names.ToArray(), toolContext.HasAttendance, toolContext.ReturnedFacts, options.MaxReplyCharacters);
        if (!verdict.Allowed)
        {
            reply = FallbackReplies.For(inbound.Text);
            outcome = outcome with { GuardReason = verdict.Reason };
        }
        var finished = outcome.FailureCode is null
            ? run.Complete(outcome.Steps, (int)outcome.Usage.InputTokens, (int)outcome.Usage.OutputTokens, clock.UtcNow, outcome.GuardReason)
            : run.Fail(outcome.FailureCode, outcome.Steps, (int)outcome.Usage.InputTokens, (int)outcome.Usage.OutputTokens, clock.UtcNow);
        if (finished.IsFailure) return Result<RunAgentResult>.Failure(finished.Error!);
        var queued = await conversations.QueueReplyAsync(command.InboundMessageId, reply, cancellationToken);
        if (queued.IsFailure) return Result<RunAgentResult>.Failure(queued.Error!);
        return Result<RunAgentResult>.Success(new(false));
    }
}
