using TutoringCentre.Application.Academics.Teachers.Commands.ArchiveTeacher;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ArchiveTeacherValidatorTests
{
    [Fact]
    public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()
    {
        var result = new ArchiveTeacherValidator().Validate(new ArchiveTeacherCommand(Guid.Empty, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "TeacherId");
    }

    [Fact]
    public void Validate_ValidId_HasNoErrors()
    {
        var result = new ArchiveTeacherValidator().Validate(new ArchiveTeacherCommand(Guid.CreateVersion7(), 0));

        Assert.True(result.IsValid);
    }
}
