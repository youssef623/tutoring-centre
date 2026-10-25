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
        Guid groupId, AddScheduleRuleRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<AddScheduleRuleCommand, AddScheduleRuleResult>(
            new AddScheduleRuleCommand(
                groupId, request.DayOfWeek, request.StartTime, request.DurationMinutes, request.Room, request.EffectiveFrom),
            ct);
        return result.ToCreatedHttpResult(value =>
            ($"/api/groups/{groupId}/schedule-rules", new AddScheduleRuleResponse(value.ScheduleRuleId, value.SessionsCreated)));
    }

    private static async Task<IResult> EndScheduleRuleAsync(Guid id, EndScheduleRuleRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<EndScheduleRuleCommand, Unit>(new EndScheduleRuleCommand(id, request.LastDate, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    // 200, not 201: one centre-wide action that may create many sessions (or none), so there is no single Location.
    private static async Task<IResult> ExtendScheduleAsync(ExtendScheduleRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ExtendScheduleCommand, ExtendScheduleResult>(new ExtendScheduleCommand(request.Through), ct);
        return result.ToHttpResult(value => Results.Ok(new ExtendScheduleResponse(value.SessionsCreated)));
    }

    // weekStart is required, but bound as optional so that a missing value reaches the validator and comes back as
    // 400 validation on errors.weekStart; the server never defaults it from its own clock. Any weekday is accepted:
    // which day begins a week (Saturday in Egypt) is the client's choice.
    private static async Task<IResult> GetTimetableAsync(
        Dispatcher dispatcher, CancellationToken ct, DateOnly? weekStart = null, Guid? groupId = null, Guid? teacherId = null)
    {
        var result = await dispatcher.QueryAsync<GetTimetableQuery, TimetableDto>(
            new GetTimetableQuery(weekStart ?? default, groupId, teacherId), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> CancelSessionAsync(Guid id, SessionVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<CancelSessionCommand, Unit>(new CancelSessionCommand(id, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }
}

/// <summary>Collections are wrapped in an object (never a bare array) so the shape can grow without a breaking change.</summary>
public sealed record ScheduleRuleListResponse(IReadOnlyList<ScheduleRuleDto> Items);

/// <summary>
/// Exactly the five local values of a weekly rule. No teacher (the group's teacher teaches it), no centre (the
/// actor's), no offset or instant: a teacherId here is an unknown field and a 400. `required` on the value types:
/// an absent weekday, time, duration or date would otherwise silently default instead of producing a 400. Room is
/// optional.
/// </summary>
public sealed record AddScheduleRuleRequest
{
    public required DayOfWeek DayOfWeek { get; init; }

    public required TimeOnly StartTime { get; init; }

    public required int DurationMinutes { get; init; }

    public string? Room { get; init; }

    public required DateOnly EffectiveFrom { get; init; }
}

public sealed record AddScheduleRuleResponse(Guid Id, int SessionsCreated);

/// <summary>The rule's last local date plus version. Sessions after that date are cancelled by the handler.</summary>
public sealed record EndScheduleRuleRequest
{
    public required DateOnly LastDate { get; init; }

    public required uint Version { get; init; }
}

/// <summary>The local date to generate through; the handler bounds the range.</summary>
public sealed record ExtendScheduleRequest
{
    public required DateOnly Through { get; init; }
}

public sealed record ExtendScheduleResponse(int SessionsCreated);

/// <summary>Version only: cancelling is a version-guarded state transition with no other input.</summary>
public sealed record SessionVersionRequest
{
    public required uint Version { get; init; }
}
