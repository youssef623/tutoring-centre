using TutoringCentre.Application.Attendance.Contracts;
using FluentValidation;
using TutoringCentre.Application.Common.Cqrs;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Application.Attendance.Queries.GetAttendance;

public sealed record GetAttendanceQuery(Guid? StudentId, Guid? GroupId, DateOnly From, DateOnly To,
    string? Cursor = null, int PageSize = 50) : IQuery<AttendancePageDto>, ITenantScoped, IRequirePermission
{
    public string RequiredPermission => Permissions.AttendanceView;
}

internal sealed class GetAttendanceValidator : AbstractValidator<GetAttendanceQuery>
{
    public GetAttendanceValidator()
    {
        RuleFor(query => query.From).NotEqual(default(DateOnly));
        RuleFor(query => query.To).NotEqual(default(DateOnly)).GreaterThanOrEqualTo(query => query.From)
            .Must((query, to) => to.DayNumber - query.From.DayNumber <= 92);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.Cursor).Must(cursor => cursor is null || AttendanceCursor.TryDecode(cursor, out _));
    }
}
