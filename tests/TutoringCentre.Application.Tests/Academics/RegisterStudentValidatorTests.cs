using TutoringCentre.Application.Academics.Students.Commands.RegisterStudent;
using TutoringCentre.Domain.Academics;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RegisterStudentValidatorTests
{
    [Fact]
    public void Validate_ExistingParentIdAlone_HasNoErrors()
    {
        var result = Validate(new RegisterStudentParent(Guid.CreateVersion7(), null, null));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_NewParentNameAndPhoneTogether_HasNoErrors()
    {
        var result = Validate(new RegisterStudentParent(null, "Omar Hassan", "01012345678"));

        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_NeitherFormGiven_ReportsErrorOnParent()
    {
        var result = Validate(new RegisterStudentParent(null, null, null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Parent");
    }

    [Fact]
    public void Validate_BothFormsGivenAtOnce_ReportsErrorOnParent()
    {
        var result = Validate(new RegisterStudentParent(Guid.CreateVersion7(), "Omar Hassan", "01012345678"));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Parent");
    }

    [Fact]
    public void Validate_NewParentNameOnlyMissingPhone_ReportsErrorOnParentAndParentPhone()
    {
        var result = Validate(new RegisterStudentParent(null, "Omar Hassan", null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Parent");
        Assert.Contains(result.Errors, error => error.PropertyName == "Parent.Phone");
    }

    [Fact]
    public void Validate_EmptyStudentName_ReportsErrorOnFullName()
    {
        var command = new RegisterStudentCommand("", null, null, new RegisterStudentParent(Guid.CreateVersion7(), null, null), ParentRelationship.Father);

        var result = new RegisterStudentValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_UndefinedRelationship_ReportsErrorOnRelationship()
    {
        var command = new RegisterStudentCommand(
            "Youssef Omar", null, null, new RegisterStudentParent(Guid.CreateVersion7(), null, null), (ParentRelationship)99);

        var result = new RegisterStudentValidator().Validate(command);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Relationship");
    }

    private static FluentValidation.Results.ValidationResult Validate(RegisterStudentParent parent) =>
        new RegisterStudentValidator().Validate(
            new RegisterStudentCommand("Youssef Omar", null, null, parent, ParentRelationship.Father));
}
