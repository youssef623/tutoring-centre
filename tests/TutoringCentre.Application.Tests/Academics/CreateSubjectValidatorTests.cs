using TutoringCentre.Application.Academics.Subjects.Commands.CreateSubject;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateSubjectValidatorTests
{
    [Fact]
    public void Validate_EmptyName_ReportsErrorOnName()
    {
        var result = new CreateSubjectValidator().Validate(new CreateSubjectCommand(""));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Name");
    }

    [Fact]
    public void Validate_NameOverEightyCharacters_ReportsErrorOnName()
    {
        var result = new CreateSubjectValidator().Validate(new CreateSubjectCommand(new string('a', 81)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ValidName_HasNoErrors()
    {
        var result = new CreateSubjectValidator().Validate(new CreateSubjectCommand("Mathematics"));

        Assert.True(result.IsValid);
    }
}
