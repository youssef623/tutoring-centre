using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using TutoringCentre.Api.Tests.Fixtures;
using TutoringCentre.Api.Tests.Support;
using TutoringCentre.Application.Assistant;
using TutoringCentre.Application.Assistant.Handles;
using TutoringCentre.Application.Assistant.Queries.GetConversationTimeline;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;
using TutoringCentre.Evaluation.Cases;
using TutoringCentre.Evaluation.Host;
using TutoringCentre.Evaluation.Models;
using TutoringCentre.Evaluation.World;
using TutoringCentre.Infrastructure.Messaging;
namespace TutoringCentre.Evaluation.Runner;

public sealed class CaseRunner(EvalHost host, ObservedLanguageModel observed)
{
    public async Task<Transcript> RunAsync(EvalCase @case)
    {
        await host.ResetAsync();
        var world = new EvalWorld(host); await world.SeedAsync();
        // Fixture event delivery is setup, outside the measured assistant conversation.
        await DriveRelayTurnAsync();
        var persona = world.Personas[@case.Persona];
        using var parent = await SignInHelper.SignInAsync(host, persona.Email, password: persona.TemporaryPassword);
        using (var changed = await AntiforgeryTestHelper.PostAsJsonAsync(parent.Client, new Uri("/api/auth/change-password", UriKind.Relative), new { currentPassword = persona.TemporaryPassword, newPassword = SharedApiFactory.TestSeedPassword })) changed.EnsureSuccessStatusCode();
        await parent.SwitchCentreAsync(@case.Persona == "maadi" ? "eval-maadi" : "eval-nile");
        // All setup/authentication precedes the measured conversation boundary.
        host.AttendanceLogs.Clear(); host.ToolOutputs.Clear(); observed.Outputs.Clear(); observed.Errors.Clear(); observed.Requests.Clear();
        var before = await DatabaseSnapshot.CaptureAsync(host.SuperuserConnectionString);
        var outputs = new List<string>(); var replies = new List<string>(); var calls = new List<ObservedCall>(); var handles = new List<HandleAssignment>(); var errors = new List<string>();
        var seen = new HashSet<Guid>();
        var runCount = 0;
        var guards = new List<string>();
        for (var turn = 0; turn < @case.Turns.Length; turn++)
        {
            host.Time.FixAt(EvalTime.Now.AddSeconds(turn));
            await using (var scope = host.Services.CreateAsyncScope())
            {
                scope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new ParentActor(persona.UserId, persona.CentreId, persona.ParentId));
                var assigned = await scope.ServiceProvider.GetRequiredService<Dispatcher>().QueryAsync<GetRunHandlesQuery, HandleTable>(new(), CancellationToken.None);
                if (assigned.IsFailure) throw new InvalidOperationException(assigned.Error!.Code);
                foreach (var h in assigned.Value.Children) handles.Add(new(turn, persona.Children.Single(x => x.Value == h.Value.Id).Key, h.Value.Id, h.Key));
            }
            using var sent = await AntiforgeryTestHelper.PostAsJsonAsync(parent.Client, new Uri("/api/portal/chat/messages", UriKind.Relative), new { clientMessageId = Guid.NewGuid(), text = @case.Turns[turn].Text });
            outputs.Add(await sent.Content.ReadAsStringAsync()); sent.EnsureSuccessStatusCode();
            await DriveRelayTurnAsync();
            using var timelineResponse = await parent.Client.GetAsync(new Uri("/api/portal/chat/messages", UriKind.Relative));
            outputs.Add(await timelineResponse.Content.ReadAsStringAsync()); timelineResponse.EnsureSuccessStatusCode();
            var conversation = await host.ScalarAsync<Guid>("select id from conversations.conversations order by created_at desc limit 1");
            await using var staffScope = host.Services.CreateAsyncScope();
            staffScope.ServiceProvider.GetRequiredService<CurrentActorContext>().Set(new StaffActor(world.Staff[persona.CentreId], persona.CentreId, StaffRole.Owner));
            var timeline = await staffScope.ServiceProvider.GetRequiredService<Dispatcher>().QueryAsync<GetConversationTimelineQuery, ConversationTimelinePage>(new(conversation, null, 100), CancellationToken.None);
            if (timeline.IsFailure) throw new InvalidOperationException(timeline.Error!.Code);
            outputs.Add(JsonSerializer.Serialize(timeline.Value));
            foreach (var item in timeline.Value.Items.Where(i => seen.Add(i.Id)))
            {
                if (item.Kind == "run") runCount++;
                if (item.GuardReason is not null) guards.Add(turn.ToString(System.Globalization.CultureInfo.InvariantCulture) + ":" + item.GuardReason);
                if (item.Kind == "outbound" && item.Text is not null)
                {
                    replies.Add(item.Text);
                    if (item.Status != "sent") errors.Add("eval.reply_not_sent");
                }
                if (item.FailureCode is not null) errors.Add(item.FailureCode);
                foreach (var c in item.ToolCalls ?? []) { calls.Add(new(turn, c.ToolName, c.ArgumentSummary.StartsWith("handles=", StringComparison.Ordinal) ? c.ArgumentSummary[8..] : null, c.Decision, c.ReasonCode)); outputs.Add(c.ResultSummary ?? ""); }
            }
        }
        outputs.AddRange(observed.Outputs);
        var toolOutputs = host.ToolOutputs.Read();
        var logs = LogCapture.Read(host.AttendanceLogs);
        outputs.AddRange(toolOutputs);
        outputs.AddRange(logs);
        errors.AddRange(observed.Errors);
        var after = await DatabaseSnapshot.CaptureAsync(host.SuperuserConnectionString);
        var captureComplete = replies.Count == @case.Turns.Length && runCount == @case.Turns.Length
            && toolOutputs.Length == calls.Count(c => c.Decision == "allowed") && logs.Length > 0 && observed.Requests.Count > 0;
        var transcript = new Transcript(@case.Id, replies.ToArray(), calls.ToArray(), handles.ToArray(), outputs.ToArray(), EvalWorld.Canaries, before, after, captureComplete, errors.ToArray());
        return transcript with { ContentFingerprint = TranscriptFingerprint.Create(transcript, toolOutputs, guards) };
    }
    private async Task DriveRelayTurnAsync()
    {
        var relay = host.Services.GetRequiredService<OutboxRelayService>();
        // Fixed production stages: inbound -> agent -> outbound. No timer or polling.
        for (var stage = 0; stage < 3; stage++) await relay.ProcessBatchAsync();
    }
}
