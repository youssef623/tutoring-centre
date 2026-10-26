using FluentValidation;

namespace TutoringCentre.Application.Academics.Scheduling.Queries.GetTimetable;

internal sealed class GetTimetableValidator : AbstractValidator<GetTimetableQuery>
{
    public GetTimetableValidator()
    {
        RuleFor(query => query.WeekStart).NotEqual(default(DateOnly));
    }
}
