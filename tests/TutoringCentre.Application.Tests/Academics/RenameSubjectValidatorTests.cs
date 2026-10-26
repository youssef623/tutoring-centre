using TutoringCentre.Application.Academics.Subjects.Commands.RenameSubject;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RenameSubjectValidatorTests
{
    [Fact]
    public void Validate_EmptyId_ReportsErrorOnSubjectId()
    {
        var result = new RenameSubjectValidator().Validate(new RenameSubjectCommand(Guid.Empty, "Mathematics", 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "SubjectId");
    }

    [Fact]
    public void Validate_NameOverEightyCharacters_ReportsErrorOnName()
    {
        var result = new RenameSubjectValidator().Validate(new RenameSubjectCommand(Guid.CreateVersion7(), new string('a', 81), 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Name");
    }

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = new RenameSubjectValidator().Validate(new RenameSubjectCommand(Guid.CreateVersion7(), "Mathematics", 0));

        Assert.True(result.IsValid);
    }
}
