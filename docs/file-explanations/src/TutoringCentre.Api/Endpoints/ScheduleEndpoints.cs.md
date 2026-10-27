# `src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs`

## Purpose

Source summary (intent, not independent execution proof): Schedule endpoints (Day 53 contract). Each endpoint: bind → dispatch → map the result. No rule, repository, entity, tenant, role or scope logic: the handlers resolve every ID inside the actor's centre and apply the actor's teacher scope to the two reads, so one route serves owners, secretaries and teachers alike. Rules travel as local values only — a weekday, an "HH:mm" time, a duration and "YYYY-MM-DD" dates in the centre's own calendar — and sessions come back as UTC instants with the zone to show them in. A rule carries no teacher: its sessions are taught by the group's teacher. Ending a rule and cancelling a session are version-guarded actions, never an HTTP DELETE: both stay as history. Protected by the authorization fallback policy; none is anonymous.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 176 lines and 9,100 bytes. Source hash: `4f2f000accce6975b4f4777f24c4684536ab267861b1803dd3d91f14972cd42f`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using TutoringCentre.Api.Http;`
- Line 2: `using TutoringCentre.Application.Academics.Scheduling;`
- Line 3: `using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;`
- Line 4: `using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;`
- Line 5: `using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;`
- Line 6: `using TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;`
- Line 7: `using TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;`
- Line 8: `using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;`
- Line 9: `using TutoringCentre.Application.Common.Cqrs;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 23](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L23): `public static class ScheduleEndpoints`
- [Line 25](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L25): `public static RouteGroupBuilder MapScheduleEndpoints(this RouteGroupBuilder api)`
- [Line 81](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L81): `private static async Task<IResult> ListGroupScheduleRulesAsync(Guid groupId, Dispatcher dispatcher, CancellationToken ct)`
- [Line 90](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L90): `private static async Task<IResult> AddScheduleRuleAsync(`
- [Line 101](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L101): `private static async Task<IResult> EndScheduleRuleAsync(Guid id, EndScheduleRuleRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 108](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L108): `private static async Task<IResult> ExtendScheduleAsync(ExtendScheduleRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 117](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L117): `private static async Task<IResult> GetTimetableAsync(`
- [Line 125](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L125): `private static async Task<IResult> CancelSessionAsync(Guid id, SessionVersionRequest request, Dispatcher dispatcher, CancellationToken ct)`
- [Line 133](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L133): `public sealed record ScheduleRuleListResponse(IReadOnlyList<ScheduleRuleDto> Items);`
- [Line 141](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L141): `public sealed record AddScheduleRuleRequest`
- [Line 154](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L154): `public sealed record AddScheduleRuleResponse(Guid Id, int SessionsCreated);`
- [Line 157](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L157): `public sealed record EndScheduleRuleRequest`
- [Line 165](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L165): `public sealed record ExtendScheduleRequest`
- [Line 170](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L170): `public sealed record ExtendScheduleResponse(int SessionsCreated);`
- [Line 173](../../../../../src/TutoringCentre.Api/Endpoints/ScheduleEndpoints.cs#L173): `public sealed record SessionVersionRequest`

## Used by: verified name references

No unique declared-name consumer was established by this extractor. Configuration and generated artifacts can be consumed through framework/tool conventions; consult their documented registration rather than inferring that they are unused.

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using TutoringCentre.Api.Http;
using TutoringCentre.Application.Academics.Scheduling;
using TutoringCentre.Application.Academics.Scheduling.Commands.AddScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Commands.CancelSession;
using TutoringCentre.Application.Academics.Scheduling.Commands.EndScheduleRule;
using TutoringCentre.Application.Academics.Scheduling.Commands.ExtendSchedule;
using TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;
using TutoringCentre.Application.Academics.Scheduling.Queries.ListGroupScheduleRules;
using TutoringCentre.Application.Common.Cqrs;

namespace TutoringCentre.Api.Endpoints;

/// <summary>
/// Schedule endpoints (Day 53 contract). Each endpoint: bind → dispatch → map the result. No rule, repository,
/// entity, tenant, role or scope logic: the handlers resolve every ID inside the actor's centre and apply the
/// actor's teacher scope to the two reads, so one route serves owners, secretaries and teachers alike.
/// Rules travel as local values only — a weekday, an "HH:mm" time, a duration and "YYYY-MM-DD" dates in the
/// centre's own calendar — and sessions come back as UTC instants with the zone to show them in. A rule carries no
/// teacher: its sessions are taught by the group's teacher. Ending a rule and cancelling a session are
/// version-guarded actions, never an HTTP DELETE: both stay as history.
/// Protected by the authorization fallback policy; none is anonymous.
/// </summary>
public static class ScheduleEndpoints
{
    public static RouteGroupBuilder MapScheduleEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);

        api.MapGet("/groups/{groupId:guid}/schedule-rules", ListGroupScheduleRulesAsync)
            .WithName("listGroupScheduleRules")
            .Produces<ScheduleRuleListResponse>()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound);

        api.MapPost("/groups/{groupId:guid}/schedule-rules", AddScheduleRuleAsync)
            .WithName("addScheduleRule")
            .Produces<AddScheduleRuleResponse>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/schedule-rules/{id:guid}/end", EndScheduleRuleAsync)
            .WithName("endScheduleRule")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapPost("/schedule/extend", ExtendScheduleAsync)
            .WithName("extendSchedule")
            .Produces<ExtendScheduleResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        api.MapGet("/timetable", GetTimetableAsync)
            .WithName("getTimetable")
            .Produces<TimetableDto>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status403Forbidden);

        api.MapPost("/sessions/{id:guid}/cancel", CancelSessionAsync)
            .WithName("cancelSession")
            .Produces(StatusCodes.Status204NoContent)
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict)
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);

        return api;
    }

    private static async Task<IResult> ListGroupScheduleRulesAsync(Guid groupId, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<ListGroupScheduleRulesQuery, IReadOnlyList<ScheduleRuleDto>>(
            new ListGroupScheduleRulesQuery(groupId), ct);
        return result.ToHttpResult(items => Results.Ok(new ScheduleRuleListResponse(items)));
    }

    // A rule has no page of its own, so Location points at the group's rules, where it is listed. groupId only
    // reaches the header on success, after the handler resolved that exact group inside the actor's centre.
    private static async Task<IResult> AddScheduleRuleAsync(
```

The excerpt stops at line 90; use the original file for the remaining 86 lines.

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
