using TutoringCentre.Application.Academics.Subjects.Commands.ArchiveSubject;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveSubjectValidatorTests
{
    [Fact]
    public void Validate_EmptyId_ReportsErrorOnSubjectId()
    {
        var result = new ArchiveSubjectValidator().Validate(new ArchiveSubjectCommand(Guid.Empty, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "SubjectId");
    }

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = new ArchiveSubjectValidator().Validate(new ArchiveSubjectCommand(Guid.CreateVersion7(), 0));

        Assert.True(result.IsValid);
    }
}
