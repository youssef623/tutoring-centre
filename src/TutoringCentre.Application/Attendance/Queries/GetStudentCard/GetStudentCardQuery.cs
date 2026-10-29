using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Attendance.Queries.GetStudentCard;

public sealed record GetStudentCardQuery(Guid StudentId) : IQuery<StudentCardDto?>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AttendanceCardsManage;
}

internal sealed class GetStudentCardValidator : AbstractValidator<GetStudentCardQuery>
{
    public GetStudentCardValidator() => RuleFor(query => query.StudentId).NotEmpty();
}

[SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Instantiated by DI.")]
internal sealed class GetStudentCardHandler(IStudentCardReadService cards, ICurrentActor actor, IAttendanceRosterSource roster)
    : IQueryHandler<GetStudentCardQuery, StudentCardDto?>
{
    public async Task<Result<StudentCardDto?>> HandleAsync(GetStudentCardQuery query, CancellationToken cancellationToken)
    {
        if (actor.Actor is not StaffActor)
            return Result<StudentCardDto?>.Failure(Error.Forbidden("auth.permission_denied", "Card management requires a staff user."));
        if (await roster.GetCardStudentAsync(query.StudentId, cancellationToken) is null)
            return Result<StudentCardDto?>.Failure(Error.NotFound("student.not_found", "The student was not found."));
        return Result<StudentCardDto?>.Success(await cards.GetMostRecentAsync(query.StudentId, cancellationToken));
    }
}
