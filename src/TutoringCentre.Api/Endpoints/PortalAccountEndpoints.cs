using TutoringCentre.Api.Http;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Identity.Portal;
using TutoringCentre.Application.Identity.Portal.Commands.CreatePortalAccount;
using TutoringCentre.Application.Identity.Portal.Commands.DeactivatePortalAccount;
using TutoringCentre.Application.Identity.Portal.Commands.ReactivatePortalAccount;
using TutoringCentre.Application.Identity.Portal.Queries.ListPortalAccounts;
using TutoringCentre.Domain.Centres;

namespace TutoringCentre.Api.Endpoints;

public static class PortalAccountEndpoints
{
    public static RouteGroupBuilder MapPortalAccountEndpoints(this RouteGroupBuilder api)
    {
        ArgumentNullException.ThrowIfNull(api);
        api.MapGet("/portal-accounts", ListAsync).WithName("ListPortalAccounts")
            .Produces<PortalAccountListResponse>().ProducesProblem(403);
        api.MapPost("/portal-accounts", CreateAsync).WithName("CreatePortalAccount")
            .Produces<CreatePortalAccountResult>(201).ProducesValidationProblem()
            .ProducesProblem(403).ProducesProblem(404).ProducesProblem(409);
        api.MapPost("/portal-accounts/{linkId:guid}/deactivate", DeactivateAsync).WithName("DeactivatePortalAccount")
            .Produces(204).ProducesProblem(403).ProducesProblem(404).ProducesProblem(409);
        api.MapPost("/portal-accounts/{linkId:guid}/reactivate", ReactivateAsync).WithName("ReactivatePortalAccount")
            .Produces(204).ProducesProblem(403).ProducesProblem(404).ProducesProblem(409);
        return api;
    }

    private static async Task<IResult> ListAsync(string kind, Guid subjectId, Dispatcher dispatcher, CancellationToken ct)
    {
        var subjectKind = kind switch { "parent" => PortalSubjectKind.Parent, "student" => PortalSubjectKind.Student, _ => (PortalSubjectKind)(-1) };
        var result = await dispatcher.QueryAsync<ListPortalAccountsQuery, IReadOnlyList<PortalAccountDto>>(new(subjectKind, subjectId), ct);
        return result.ToHttpResult(items => Results.Ok(new PortalAccountListResponse(items)));
    }

    private static async Task<IResult> CreateAsync(CreatePortalAccountRequest request, HttpContext context, Dispatcher dispatcher, CancellationToken ct)
    {
        context.Response.Headers.CacheControl = "no-store";
        var result = await dispatcher.SendAsync<CreatePortalAccountCommand, CreatePortalAccountResult>(
            new(request.Kind, request.SubjectId, request.Email, request.PreferredLocale), ct);
        return result.ToCreatedHttpResult(value => (object)value);
    }

    private static async Task<IResult> DeactivateAsync(Guid linkId, StaffVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<DeactivatePortalAccountCommand, Unit>(new(linkId, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }

    private static async Task<IResult> ReactivateAsync(Guid linkId, StaffVersionRequest request, Dispatcher dispatcher, CancellationToken ct)
    {
        var result = await dispatcher.SendAsync<ReactivatePortalAccountCommand, Unit>(new(linkId, request.Version), ct);
        return result.ToHttpResult(_ => Results.NoContent());
    }
}

public sealed record CreatePortalAccountRequest(PortalSubjectKind Kind, Guid SubjectId, string Email, SupportedLocale PreferredLocale);
public sealed record PortalAccountListResponse(IReadOnlyList<PortalAccountDto> Items);
