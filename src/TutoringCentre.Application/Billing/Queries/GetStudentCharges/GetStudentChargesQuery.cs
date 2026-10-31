using FluentValidation;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;
namespace TutoringCentre.Application.Billing.Queries.GetStudentCharges;
public sealed record GetStudentChargesQuery(Guid StudentId, string? Cursor = null, int PageSize = 25) : IQuery<StudentChargesPage>, ITenantScoped, IRequirePermission
{ public string RequiredPermission => Permissions.BillingView; }
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class GetStudentChargesValidator : AbstractValidator<GetStudentChargesQuery>
{
    public GetStudentChargesValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty(); RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
        RuleFor(x => x.Cursor).Must(x => x is null || ChargeCursor.TryDecode(x, out _)).WithMessage("The cursor is invalid.");
    }
}
[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class GetStudentChargesHandler(IBillingRosterSource roster, IBillingReadService read) : IQueryHandler<GetStudentChargesQuery, StudentChargesPage>
{
    public async Task<Result<StudentChargesPage>> HandleAsync(GetStudentChargesQuery query, CancellationToken cancellationToken)
    {
        if (!await roster.StudentExistsAsync(query.StudentId, cancellationToken)) return Result<StudentChargesPage>.Failure(Error.NotFound("student.not_found", "Student not found."));
        return Result<StudentChargesPage>.Success(await read.GetAsync(query.StudentId, query.Cursor, query.PageSize, cancellationToken));
    }
}
