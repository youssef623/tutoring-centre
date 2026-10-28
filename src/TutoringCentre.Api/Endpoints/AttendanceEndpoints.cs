using TutoringCentre.Api.Http;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Commands.CorrectAttendance;
using TutoringCentre.Application.Attendance.Commands.MarkAttendance;
using TutoringCentre.Application.Attendance.Queries.GetAttendance;
using TutoringCentre.Application.Attendance.Queries.GetSessionAttendance;
using TutoringCentre.Application.Attendance.Queries.ListAttendanceSessions;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Api.Endpoints;

public static class AttendanceEndpoints
{
    public static RouteGroupBuilder MapAttendanceEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);
        api.MapGet("/attendance/sessions", ListSessionsAsync).WithName("listAttendanceSessions")
            .Produces<AttendanceSessionsDto>().ProducesValidationProblem().ProducesProblem(401).ProducesProblem(403);
        api.MapGet("/sessions/{sessionId:guid}/attendance", GetSessionAsync).WithName("getSessionAttendance")
            .Produces<SessionAttendanceDto>().ProducesProblem(400).ProducesProblem(401).ProducesProblem(403).ProducesProblem(404);
        api.MapPut("/sessions/{sessionId:guid}/attendance", MarkAsync).WithName("markAttendance")
            .Produces<MarkAttendanceResult>().ProducesValidationProblem().ProducesProblem(401).ProducesProblem(403)
            .ProducesProblem(404).ProducesProblem(409).ProducesProblem(422);
        api.MapPost("/attendance/{recordId:guid}/correct", CorrectAsync).WithName("correctAttendance")
            .Produces(204).ProducesValidationProblem().ProducesProblem(401).ProducesProblem(403)
            .ProducesProblem(404).ProducesProblem(409).ProducesProblem(422);
        api.MapGet("/attendance", GetHistoryAsync).WithName("getAttendance")
            .Produces<AttendancePageDto>().ProducesValidationProblem().ProducesProblem(401).ProducesProblem(403);
        return api;
    }

    private static async Task<IResult> ListSessionsAsync(Dispatcher dispatcher, CancellationToken ct, DateOnly? date = null)
    {
        var result = await dispatcher.QueryAsync<ListAttendanceSessionsQuery, AttendanceSessionsDto>(new(date), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> GetSessionAsync(Guid sessionId, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetSessionAttendanceQuery, SessionAttendanceDto>(new(sessionId), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> MarkAsync(Guid sessionId, MarkAttendanceRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<MarkAttendanceCommand, MarkAttendanceResult>(new(sessionId,
            request.Marks?.Select(mark => mark is null ? null! : new AttendanceMark(mark.StudentId, mark.Status)).ToArray()!), ct);
        return result.ToHttpResult(Results.Ok);
    }

    private static async Task<IResult> CorrectAsync(Guid recordId, CorrectAttendanceRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<CorrectAttendanceCommand, Unit>(new(recordId, request.Status, request.Reason, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> GetHistoryAsync(Dispatcher dispatcher, CancellationToken ct,
        Guid? studentId = null, Guid? groupId = null, DateOnly? from = null, DateOnly? to = null, string? cursor = null, int pageSize = 50)
    {
        var result = await dispatcher.QueryAsync<GetAttendanceQuery, AttendancePageDto>(new(studentId, groupId, from ?? default, to ?? default, cursor, pageSize), ct);
        return result.ToHttpResult(Results.Ok);
    }
}

public sealed record MarkAttendanceRequest
{
    public required IReadOnlyList<AttendanceMarkRequest> Marks { get; init; }
}

public sealed record AttendanceMarkRequest
{
    public required Guid StudentId { get; init; }
    public required AttendanceStatus Status { get; init; }
}

public sealed record CorrectAttendanceRequest
{
    public required AttendanceStatus Status { get; init; }
    public required string Reason { get; init; }
    public required uint Version { get; init; }
}
