using TutoringCentre.Api.Http;
using TutoringCentre.Application.Attendance;
using TutoringCentre.Application.Attendance.Commands.IssueGroupCards;
using TutoringCentre.Application.Attendance.Commands.IssueStudentCard;
using TutoringCentre.Application.Attendance.Commands.RevokeStudentCard;
using TutoringCentre.Application.Attendance.Queries.GetStudentCard;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Api.Endpoints;

public static class StudentCardEndpoints
{
    public static RouteGroupBuilder MapStudentCardEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);
        api.MapGet("/students/{studentId:guid}/card", GetAsync).WithName("getStudentCard")
            .Produces<StudentCardResponse>().ProducesProblem(400).ProducesProblem(401).ProducesProblem(403).ProducesProblem(404);
        api.MapPost("/students/{studentId:guid}/card", IssueAsync).WithName("issueStudentCard")
            .Produces<IssuedCard>(201).ProducesProblem(400).ProducesProblem(401).ProducesProblem(403)
            .ProducesProblem(404).ProducesProblem(409).ProducesProblem(422);
        api.MapPost("/groups/{groupId:guid}/cards", IssueGroupAsync).WithName("issueGroupCards")
            .Produces<IssuedGroupCards>().ProducesValidationProblem().ProducesProblem(401).ProducesProblem(403)
            .ProducesProblem(404).ProducesProblem(409).ProducesProblem(422);
        api.MapPost("/student-cards/{cardId:guid}/revoke", RevokeAsync).WithName("revokeStudentCard")
            .Produces(204).ProducesValidationProblem().ProducesProblem(401).ProducesProblem(403)
            .ProducesProblem(404).ProducesProblem(409).ProducesProblem(422);
        return api;
    }
    private static async Task<IResult> GetAsync(Guid studentId, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.QueryAsync<GetStudentCardQuery, StudentCardDto?>(new(studentId), ct);
        return result.ToHttpResult(card => Results.Ok(new StudentCardResponse(card)));
    }
    private static async Task<IResult> IssueAsync(Guid studentId, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<IssueStudentCardCommand, IssuedCard>(new(studentId), ct);
        return result.ToCreatedHttpResult(card => ($"/api/students/{card.StudentId}/card", card));
    }
    private static async Task<IResult> IssueGroupAsync(Guid groupId, IssueGroupCardsRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<IssueGroupCardsCommand, IssuedGroupCards>(new(groupId, request.StudentIds), ct);
        return result.ToHttpResult(Results.Ok);
    }
    private static async Task<IResult> RevokeAsync(Guid cardId, RevokeStudentCardRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<RevokeStudentCardCommand, Unit>(new(cardId, request.Reason, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }
}
public sealed record StudentCardResponse(StudentCardDto? Card);
public sealed record IssueGroupCardsRequest
{
    public required IReadOnlyList<Guid> StudentIds { get; init; }
}
public sealed record RevokeStudentCardRequest
{
    public required CardRevocationReason Reason { get; init; }
    public required uint Version { get; init; }
}
