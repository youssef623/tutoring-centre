using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Npgsql;
using TutoringCentre.Domain.Common;
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
    public void TryTranslate_UniqueViolationOnParentPhoneConstraint_ReturnsParentPhoneTaken()
    {
        var exception = UniqueViolation("ux_parents_centre_phone");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("parent.phone_taken", result.Error!.Code);
    }

    [Fact]
    public void TryTranslate_UniqueViolationOnStudentParentConstraint_ReturnsParentAlreadyLinked()
    {
        var exception = UniqueViolation("ux_student_parents_student_parent");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("student.parent_already_linked", result.Error!.Code);
    }

    [Fact]
    public void TryTranslate_UniqueViolationOnActiveEnrollmentIndex_ReturnsAlreadyEnrolledConflict()
    {
        var exception = UniqueViolation("ux_enrollments_active_student_group");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("enrollment.already_enrolled", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
    }

    [Fact]
    public void TryTranslate_UniqueViolationOnOnePrimaryIndex_IsNotTranslated()
    {
        var exception = UniqueViolation("ux_student_parents_one_primary");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.Null(result);
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

    [Fact]
    public void TryTranslate_ExclusionViolationOnTeacherOverlap_ReturnsTeacherDoubleBookedConflict()
    {
        var exception = ExclusionViolation("ex_class_sessions_teacher_overlap");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("schedule.teacher_double_booked", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.DoesNotContain("ex_class_sessions", result.Error.Message, StringComparison.Ordinal);
    }

    [Fact]
    public void TryTranslate_ExclusionViolationOnGroupOverlap_ReturnsGroupDoubleBookedConflict()
    {
        var exception = ExclusionViolation("ex_class_sessions_group_overlap");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.NotNull(result);
        Assert.True(result.IsFailure);
        Assert.Equal("schedule.group_double_booked", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
    }

    [Fact]
    public void TryTranslate_ExclusionViolationOnUnlistedConstraint_IsNotTranslated()
    {
        var exception = ExclusionViolation("ex_something_nobody_registered");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.Null(result);
    }

    [Fact]
    public void TryTranslate_UniqueViolationOnSessionRuleDate_IsNotTranslated()
    {
        var exception = UniqueViolation("ux_class_sessions_rule_date");

        var result = PersistenceErrorTranslator.TryTranslate(exception, NullLogger.Instance);

        Assert.Null(result);
    }

    private static DbUpdateException ExclusionViolation(string constraintName) =>
        ConstraintViolation("23P01", "conflicting key value violates exclusion constraint", constraintName);

    private static DbUpdateException UniqueViolation(string constraintName) =>
        ConstraintViolation("23505", "duplicate key value violates unique constraint", constraintName);

    private static DbUpdateException ConstraintViolation(string sqlState, string messageText, string constraintName)
    {
        var postgresException = new PostgresException(
            messageText: messageText,
            severity: "ERROR",
            invariantSeverity: "ERROR",
            sqlState: sqlState,
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
