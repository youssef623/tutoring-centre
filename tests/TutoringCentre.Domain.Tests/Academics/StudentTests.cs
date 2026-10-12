using TutoringCentre.Domain.Academics;
using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Academics;

public sealed class StudentTests
{
    private static readonly Guid CentreId = Guid.NewGuid();
    private static readonly PhoneNumber Phone = PhoneNumber.Create("01012345678").Value;

    // Checked after every operation, successful or not: the invariant itself, not one operation's own effect.
    private static void AssertInvariant(Student student)
    {
        Assert.InRange(student.Parents.Count, Student.MinParents, Student.MaxParents);
        Assert.Equal(student.Parents.Count, student.Parents.Select(p => p.ParentId).Distinct().Count());
        Assert.Equal(1, student.Parents.Count(p => p.IsPrimary));
    }

    private static Student RegisterValid(Guid? primaryParentId = null)
    {
        var student = Student.Register(
            CentreId, "Omar Hassan", Phone, "Grade 10", primaryParentId ?? Guid.NewGuid(), ParentRelationship.Father).Value;
        AssertInvariant(student);
        return student;
    }

    [Fact]
    public void Register_WithValidInput_SucceedsActiveWithOnePrimaryLinkAndZeroRevision()
    {
        var parentId = Guid.NewGuid();

        var result = Student.Register(CentreId, "Omar Hassan", Phone, "Grade 10", parentId, ParentRelationship.Father);

        Assert.True(result.IsSuccess);
        var student = result.Value;
        AssertInvariant(student);
        Assert.Equal(StudentStatus.Active, student.Status);
        Assert.Equal(0, student.LinksRevision);
        var link = student.Parents.Single();
        Assert.True(link.IsPrimary);
        Assert.Equal(parentId, link.ParentId);
        Assert.Equal(CentreId, link.CentreId);
        Assert.Equal(ParentRelationship.Father, link.Relationship);
    }

    [Fact]
    public void Register_WithEmptyCentre_ReturnsCentreRequired()
    {
        var result = Student.Register(Guid.Empty, "Omar Hassan", Phone, "Grade 10", Guid.NewGuid(), ParentRelationship.Father);

        Assert.True(result.IsFailure);
        Assert.Equal("student.centre_required", result.Error!.Code);
        Assert.Equal(ErrorKind.Validation, result.Error.Kind);
    }

    [Fact]
    public void Register_With1CharacterName_ReturnsNameInvalid()
    {
        var result = Student.Register(CentreId, "O", Phone, "Grade 10", Guid.NewGuid(), ParentRelationship.Father);

        Assert.True(result.IsFailure);
        Assert.Equal("student.name_invalid", result.Error!.Code);
        Assert.Contains("fullName", result.Error.Fields!.Keys);
    }

    [Fact]
    public void Register_With121CharacterName_ReturnsNameInvalid()
    {
        var result = Student.Register(
            CentreId, new string('a', 121), Phone, "Grade 10", Guid.NewGuid(), ParentRelationship.Father);

        Assert.True(result.IsFailure);
        Assert.Equal("student.name_invalid", result.Error!.Code);
    }

    [Fact]
    public void Register_With41CharacterSchoolYear_ReturnsSchoolYearInvalid()
    {
        var result = Student.Register(
            CentreId, "Omar Hassan", Phone, new string('9', 41), Guid.NewGuid(), ParentRelationship.Father);

        Assert.True(result.IsFailure);
        Assert.Equal("student.school_year_invalid", result.Error!.Code);
        Assert.Contains("schoolYear", result.Error.Fields!.Keys);
    }

    [Fact]
    public void Register_WithEmptyParentId_ReturnsParentRequired()
    {
        var result = Student.Register(CentreId, "Omar Hassan", Phone, "Grade 10", Guid.Empty, ParentRelationship.Father);

        Assert.True(result.IsFailure);
        Assert.Equal("student.parent_required", result.Error!.Code);
    }

    [Fact]
    public void Register_WithNullPhoneAndSchoolYear_Succeeds()
    {
        var result = Student.Register(CentreId, "Omar Hassan", null, null, Guid.NewGuid(), ParentRelationship.Father);

        Assert.True(result.IsSuccess);
        AssertInvariant(result.Value);
        Assert.Null(result.Value.Phone);
        Assert.Null(result.Value.SchoolYear);
    }

    [Fact]
    public void AddParent_SecondParent_IsNotPrimaryAndRevisionIncreases()
    {
        var student = RegisterValid();
        var newParentId = Guid.NewGuid();

        var result = student.AddParent(newParentId, ParentRelationship.Mother);
        AssertInvariant(student);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, student.LinksRevision);
        var link = student.Parents.Single(p => p.ParentId == newParentId);
        Assert.False(link.IsPrimary);
    }

    [Fact]
    public void AddParent_SameParentTwice_ReturnsParentAlreadyLinked()
    {
        var parentId = Guid.NewGuid();
        var student = RegisterValid(parentId);

        var result = student.AddParent(parentId, ParentRelationship.Mother);
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.parent_already_linked", result.Error!.Code);
        Assert.Equal(ErrorKind.Conflict, result.Error.Kind);
        Assert.Equal(0, student.LinksRevision);
    }

    [Fact]
    public void AddParent_AFifthParent_ReturnsTooManyParents()
    {
        var student = RegisterValid();
        student.AddParent(Guid.NewGuid(), ParentRelationship.Mother);
        student.AddParent(Guid.NewGuid(), ParentRelationship.Guardian);
        student.AddParent(Guid.NewGuid(), ParentRelationship.Other);
        AssertInvariant(student);

        var result = student.AddParent(Guid.NewGuid(), ParentRelationship.Other);
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.too_many_parents", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(3, student.LinksRevision);
    }

    [Fact]
    public void RemoveParent_Unknown_ReturnsParentNotLinked()
    {
        var student = RegisterValid();

        var result = student.RemoveParent(Guid.NewGuid());
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.parent_not_linked", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
        Assert.Equal(0, student.LinksRevision);
    }

    [Fact]
    public void RemoveParent_TheOnlyParent_ReturnsLastParent()
    {
        var parentId = Guid.NewGuid();
        var student = RegisterValid(parentId);

        var result = student.RemoveParent(parentId);
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.last_parent", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(0, student.LinksRevision);
    }

    [Fact]
    public void RemoveParent_ThePrimaryWhileOthersRemain_ReturnsPrimaryParentRequired()
    {
        var primaryId = Guid.NewGuid();
        var student = RegisterValid(primaryId);
        student.AddParent(Guid.NewGuid(), ParentRelationship.Mother);

        var result = student.RemoveParent(primaryId);
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.primary_parent_required", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(1, student.LinksRevision);
    }

    [Fact]
    public void RemoveParent_ANonPrimaryParent_IsRemovedAndRevisionIncreases()
    {
        var student = RegisterValid();
        var secondParentId = Guid.NewGuid();
        student.AddParent(secondParentId, ParentRelationship.Mother);

        var result = student.RemoveParent(secondParentId);
        AssertInvariant(student);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, student.LinksRevision);
        Assert.DoesNotContain(student.Parents, p => p.ParentId == secondParentId);
    }

    [Fact]
    public void SetPrimaryParent_Unknown_ReturnsParentNotLinked()
    {
        var student = RegisterValid();

        var result = student.SetPrimaryParent(Guid.NewGuid());
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.parent_not_linked", result.Error!.Code);
        Assert.Equal(ErrorKind.NotFound, result.Error.Kind);
        Assert.Equal(0, student.LinksRevision);
    }

    [Fact]
    public void SetPrimaryParent_AlreadyPrimary_ReturnsAlreadyPrimary()
    {
        var primaryId = Guid.NewGuid();
        var student = RegisterValid(primaryId);

        var result = student.SetPrimaryParent(primaryId);
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.already_primary", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(0, student.LinksRevision);
    }

    [Fact]
    public void SetPrimaryParent_Valid_ExactlyOnePrimaryAndRevisionIncreases()
    {
        var primaryId = Guid.NewGuid();
        var student = RegisterValid(primaryId);
        var secondParentId = Guid.NewGuid();
        student.AddParent(secondParentId, ParentRelationship.Mother);

        var result = student.SetPrimaryParent(secondParentId);
        AssertInvariant(student);

        Assert.True(result.IsSuccess);
        Assert.Equal(2, student.LinksRevision);
        Assert.True(student.Parents.Single(p => p.ParentId == secondParentId).IsPrimary);
        Assert.False(student.Parents.Single(p => p.ParentId == primaryId).IsPrimary);
    }

    [Fact]
    public void Archive_Twice_ReturnsAlreadyArchived()
    {
        var student = RegisterValid();
        Assert.True(student.Archive().IsSuccess);
        AssertInvariant(student);

        var result = student.Archive();
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.already_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public void Restore_WhenActive_ReturnsNotArchived()
    {
        var student = RegisterValid();

        var result = student.Restore();
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.not_archived", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public void Restore_WhileArchived_ReturnsToActive()
    {
        var student = RegisterValid();
        student.Archive();

        var result = student.Restore();
        AssertInvariant(student);

        Assert.True(result.IsSuccess);
        Assert.Equal(StudentStatus.Active, student.Status);
    }

    [Fact]
    public void UpdateDetails_WhileArchived_ReturnsArchivedReadOnly()
    {
        var student = RegisterValid();
        student.Archive();

        var result = student.UpdateDetails("New Name", Phone, "Grade 11");
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.archived_read_only", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
    }

    [Fact]
    public void AddParent_WhileArchived_ReturnsArchivedReadOnly()
    {
        var student = RegisterValid();
        student.Archive();

        var result = student.AddParent(Guid.NewGuid(), ParentRelationship.Mother);
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.archived_read_only", result.Error!.Code);
        Assert.Equal(ErrorKind.Rule, result.Error.Kind);
        Assert.Equal(0, student.LinksRevision);
    }

    [Fact]
    public void RemoveParent_WhileArchived_ReturnsArchivedReadOnly()
    {
        var primaryId = Guid.NewGuid();
        var student = RegisterValid(primaryId);
        student.AddParent(Guid.NewGuid(), ParentRelationship.Mother);
        student.Archive();

        var result = student.RemoveParent(primaryId);
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.archived_read_only", result.Error!.Code);
    }

    [Fact]
    public void SetPrimaryParent_WhileArchived_ReturnsArchivedReadOnly()
    {
        var primaryId = Guid.NewGuid();
        var student = RegisterValid(primaryId);
        var secondParentId = Guid.NewGuid();
        student.AddParent(secondParentId, ParentRelationship.Mother);
        student.Archive();

        var result = student.SetPrimaryParent(secondParentId);
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.archived_read_only", result.Error!.Code);
    }

    [Fact]
    public void UpdateDetails_WithInvalidName_FailsAndLeavesStateUnchanged()
    {
        var student = RegisterValid();

        var result = student.UpdateDetails("", Phone, "Grade 11");
        AssertInvariant(student);

        Assert.True(result.IsFailure);
        Assert.Equal("student.name_invalid", result.Error!.Code);
        Assert.Equal("Omar Hassan", student.FullName);
        Assert.Equal("Grade 10", student.SchoolYear);
    }
}
