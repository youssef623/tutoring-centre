# `src/TutoringCentre.Infrastructure/Persistence/PersistenceErrorTranslator.cs`

## Purpose

The translator distinguishes optimistic concurrency, unique constraint, and exclusion constraint failures using types, SQLSTATE, and constraint names. Known normal conflicts become Results; deliberately excluded invariants and other database faults remain exceptions. Unknown unique names receive a generic conflict plus warning, while unknown exclusion names remain loud.

## Where it fits

Category: **source**. [Open current file](../../../../../src/TutoringCentre.Infrastructure/Persistence/PersistenceErrorTranslator.cs). [Complete file index](../../../INDEX.md). [Current teaching overview](../../../../PROJECT_OVERVIEW3.md).

The reviewed file has 65 lines and 3,188 bytes. Source hash: `41f2b531bd2f1268bacf5a20dca1b5c27b7fbb76b8013e0ddd0fc1dda16711b0`. This structural guide was generated from current bytes; it does not claim that every branch has been executed.

## Dependencies visible in the file

These are literal imports and project/package references. An import can be unused; this list is not a runtime call graph.

- Line 1: `using System.Diagnostics.CodeAnalysis;`
- Line 2: `using Microsoft.EntityFrameworkCore;`
- Line 3: `using Microsoft.Extensions.Logging;`
- Line 4: `using Npgsql;`
- Line 5: `using TutoringCentre.Domain.Common;`

## Walkthrough and declaration map

Open each declaration at the cited line and follow the calls inside its body. Constructor parameters identify collaborators; return types distinguish data, results and asynchronous work.

- [Line 17](../../../../../src/TutoringCentre.Infrastructure/Persistence/PersistenceErrorTranslator.cs#L17): `internal static class PersistenceErrorTranslator`
- [Line 26](../../../../../src/TutoringCentre.Infrastructure/Persistence/PersistenceErrorTranslator.cs#L26): `public static Result? TryTranslate(Exception exception, ILogger logger)`
- [Line 35](../../../../../src/TutoringCentre.Infrastructure/Persistence/PersistenceErrorTranslator.cs#L35): `return Result.Failure(Error.Conflict("concurrency.stale", "This record was changed by someone else. Reload and try again."));`

## Used by: verified name references

These locations reference a declaration name unique to this file. This lexical evidence includes type usage and imports; it is not proof of a particular runtime call.

- [src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs:79](../../../../../src/TutoringCentre.Infrastructure/Persistence/UnitOfWork.cs#L79)
- [tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs:16](../../../../../tests/TutoringCentre.Infrastructure.Tests/Persistence/PersistenceErrorTranslatorTests.cs#L16)

## Current source excerpt

The following excerpt is verbatim current source, not a hypothetical example. Line numbers in the declaration map refer to the original file.

```csharp
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using TutoringCentre.Domain.Common;

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

            var mapped = ConstraintErrorCatalog.FindUnique(postgresException.ConstraintName);
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
```

## Before modifying this file

Check the current callers, contracts and tests before changing a declaration. Imported names show dependencies, but only call sites and registrations establish actual use. Preserve generated-file ownership: change a generator input and regenerate artifacts when that is the documented workflow. Read the corresponding concept and execution trace in the current overview before altering a shared boundary.
