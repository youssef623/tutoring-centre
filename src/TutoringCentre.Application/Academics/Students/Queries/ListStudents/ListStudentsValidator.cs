using FluentValidation;

namespace TutoringCentre.Application.Academics.Students.Queries.ListStudents;

/// <summary>Shape checks only. The cursor's value is decoded here to validate it, then decoded again (never trusted) by the read service that actually uses it.</summary>
internal sealed class ListStudentsValidator : AbstractValidator<ListStudentsQuery>
{
    public ListStudentsValidator()
    {
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);

        RuleFor(query => query.Cursor)
            .Must(cursor => cursor is null || StudentCursor.TryDecode(cursor, out _))
            .WithMessage("Cursor is invalid.");
    }
}
