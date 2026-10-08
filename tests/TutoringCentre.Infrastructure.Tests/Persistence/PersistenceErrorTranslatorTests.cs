using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using TutoringCentre.Infrastructure.Persistence;

namespace TutoringCentre.Infrastructure.Tests.Persistence;

public sealed class PersistenceErrorTranslatorTests
{
    [Fact]
    public void TryTranslate_UniqueViolationOnRegisteredConstraint_ReturnsMappedConflict()
    {
        var exception = UniqueViolation("ux_centres_slug");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("centre.slug_taken", result.Error!.Code);
    }

    [Fact]
    public void TryTranslate_UniqueViolationOnUnregisteredConstraint_ReturnsGenericDuplicate()
    {
        var exception = UniqueViolation("ux_something_nobody_registered");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("conflict.duplicate", result.Error!.Code);
    }

    [Fact]
    public void TryTranslate_ForeignKeyViolation_IsNotTranslated()
    {
        var postgresException = new PostgresException("fk violation", "ERROR", "ERROR", "23503");
        var exception = new DbUpdateException("update failed", postgresException);

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.Null(result);
    }

    [Fact]
    public void TryTranslate_CheckViolation_IsNotTranslated()
    {
        var postgresException = new PostgresException("check violation", "ERROR", "ERROR", "23514");
        var exception = new DbUpdateException("update failed", postgresException);

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.Null(result);
    }

    [Fact]
    public void TryTranslate_ConcurrencyException_ReturnsConcurrencyStale()
    {
        var exception = new DbUpdateConcurrencyException("stale");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("concurrency.stale", result.Error!.Code);
    }

    [Fact]
    public void TryTranslate_UnrelatedException_IsNotTranslated()
    {
        var exception = new InvalidOperationException("unrelated");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.Null(result);
    }

    private static DbUpdateException UniqueViolation(string constraintName)
    {
        var postgresException = new PostgresException(
            messageText: "duplicate key value violates unique constraint",
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: "23505",
            detail: null,
            hint: null,
            position: 0,
            internalPosition: 0,
            internalQuery: null,
            where: null,
            schemaName: null,
            tableName: null,
            columnName: null,
            dataTypeName: null,
            constraintName: constraintName,
            file: null,
            line: null,
            routine: null);
        return new DbUpdateException("update failed", postgresException);
    }
}
