using FluentValidation;
using TutoringCentre.Application.Audit;

namespace TutoringCentre.Application.Audit.Queries.GetAuditLog;

/// <summary>Shape checks only, per the Day 32 contract. The cursor's value is decoded here to validate it, then decoded again (never trusted) by the read service that actually uses it.</summary>
internal sealed class GetAuditLogValidator : AbstractValidator<GetAuditLogQuery>
{
    private static readonly string[] KnownEntityTypes = ["subject", "membership", "centre"];

    public GetAuditLogValidator()
    {
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);

        RuleFor(query => query.EntityType)
            .Must(entityType => entityType is null || KnownEntityTypes.Contains(entityType))
            .WithMessage("Entity type is not recognised.");

        RuleFor(query => query.EntityType)
            .NotNull()
            .When(query => query.EntityId is not null)
            .WithMessage("entityId requires entityType.");

        RuleFor(query => query.From)
            .Must((query, from) => from is null || query.To is null || from <= query.To)
            .WithMessage("'from' must be at or before 'to'.");

        RuleFor(query => query.Cursor)
            .Must(cursor => cursor is null || AuditCursor.TryDecode(cursor, out _))
            .WithMessage("Cursor is invalid.");
    }
}
