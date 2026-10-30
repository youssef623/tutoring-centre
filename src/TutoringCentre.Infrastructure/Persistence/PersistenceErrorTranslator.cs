using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using TutoringCentre.Domain.Common;
using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Turns the three expected families of save-time database failure into a <see cref="Result"/>: a stale
/// optimistic-concurrency version, a unique-constraint violation, and an exclusion-constraint violation (both via
/// <see cref="ConstraintErrorCatalog"/>). Matches on SQLSTATE and constraint name only, never on message text.
/// Everything else — foreign-key (23503), check (23514), a unique violation the catalogue deliberately excludes
/// (Task 38.7), an exclusion violation on a constraint the catalogue does not list, and any other failure — is
/// left untranslated (null), so the caller rethrows it (Task 25.4).
/// </summary>
internal static class PersistenceErrorTranslator
{
    private const string UniqueViolation = "23505";
    private const string ExclusionViolation = "23P01";

    [SuppressMessage(
        "Performance",
        "CA1848:Use the LoggerMessage delegates",
        Justification = "One warning log with this exact template is the manual's required format; a source-generated delegate would be premature for a single call site.")]
    public static Result? TryTranslate(Exception exception, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(exception);
        ArgumentNullException.ThrowIfNull(logger);

        // Checked before DbUpdateException: DbUpdateConcurrencyException derives from it, and EF raises this one
        // itself — from zero rows affected by the UPDATE — with no inner PostgresException to inspect.
        if (exception is DbUpdateConcurrencyException)
        {
            return Result.Failure(Error.Conflict("concurrency.stale", "This record was changed by someone else. Reload and try again."));
        }

        if (exception is DbUpdateException { InnerException: PostgresException { SqlState: UniqueViolation } postgresException })
        {
            if (ConstraintErrorCatalog.IsDeliberatelyExcluded(postgresException.ConstraintName))
            {
                return null;
            }

            // Portal and staff creation share memberships' unique keys. Preserve each use case's stable
            // conflict code even when concurrent creates lose on membership insertion before link insertion.
            var constraintName = postgresException.ConstraintName;
            if (constraintName is "ux_memberships_centre_user" or "ak_memberships_centre_user" or "ux_memberships_centre_user_role"
                && ((DbUpdateException)exception).Entries.Any(entry => entry.Entity is Membership { IsPortal: true }))
                constraintName = "ux_portal_links_centre_user";
            var mapped = ConstraintErrorCatalog.FindUnique(constraintName);
            if (mapped is not null)
            {
                return Result.Failure(mapped);
            }

            logger.LogWarning("Unique violation on unregistered constraint {ConstraintName}", postgresException.ConstraintName);
            return Result.Failure(Error.Conflict("conflict.duplicate", "This conflicts with existing data."));
        }

        // No generic fallback here, unlike unique violations: every exclusion constraint is listed, so an
        // unlisted one is a bug and must stay loud.
        if (exception is DbUpdateException { InnerException: PostgresException { SqlState: ExclusionViolation } exclusionException })
        {
            var mapped = ConstraintErrorCatalog.FindExclusion(exclusionException.ConstraintName);
            return mapped is null ? null : Result.Failure(mapped);
        }

        return null;
    }
}
