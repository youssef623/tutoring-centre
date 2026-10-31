using TutoringCentre.Api.Http;
using TutoringCentre.Application.Billing;
using TutoringCentre.Application.Billing.Queries.GetStudentCharges;
using TutoringCentre.Application.Common.Cqrs;
namespace TutoringCentre.Api.Endpoints;
public static class StudentChargesEndpoints
{
    public static RouteGroupBuilder MapStudentChargesEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);
        api.MapGet("/students/{studentId}/charges", async (Guid studentId, Dispatcher dispatcher, CancellationToken ct, string? cursor = null, int pageSize = 25) =>
            (await dispatcher.QueryAsync<GetStudentChargesQuery, StudentChargesPage>(new(studentId, cursor, pageSize), ct)).ToHttpResult(Results.Ok))
            .WithName("GetStudentCharges").Produces<StudentChargesPage>().ProducesValidationProblem().ProducesProblem(403).ProducesProblem(404);
        return api;
    }
}
