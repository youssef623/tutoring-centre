using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Infrastructure.Persistence;

/// <summary>
/// Turns the two expected families of save-time database failure into a <see cref="Result"/>: a stale
/// optimistic-concurrency version, and a unique-constraint violation (via <see cref="UniqueConstraintCatalogue"/>).
/// Matches on SQLSTATE and constraint name only, never on message text. Everything else — foreign-key (23503),
/// check (23514) and any other failure — is left untranslated (null), so the caller rethrows it (Task 25.4).
/// </summary>
internal static class PersistenceErrorTranslator
{
    private const string UniqueViolation = "23505";

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
            var mapped = UniqueConstraintCatalogue.Find(postgresException.ConstraintName);
            if (mapped is not null)
            {
                return Result.Failure(mapped);
            }

            logger.LogWarning("Unique violation on unregistered constraint {ConstraintName}", postgresException.ConstraintName);
            return Result.Failure(Error.Conflict("conflict.duplicate", "This conflicts with existing data."));
        }

        return null;
    }
}
