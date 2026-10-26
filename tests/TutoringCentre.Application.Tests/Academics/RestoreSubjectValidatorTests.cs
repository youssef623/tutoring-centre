using TutoringCentre.Application.Academics.Subjects.Commands.RestoreSubject;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class RestoreSubjectValidatorTests
{
    [Fact]
    public void Validate_EmptyId_ReportsErrorOnSubjectId()
    {
        var result = new RestoreSubjectValidator().Validate(new RestoreSubjectCommand(Guid.Empty, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "SubjectId");
    }

    [Fact]
    public void Validate_ValidCommand_HasNoErrors()
    {
        var result = new RestoreSubjectValidator().Validate(new RestoreSubjectCommand(Guid.CreateVersion7(), 0));

        Assert.True(result.IsValid);
    }
}
