using TutoringCentre.Application.Academics.Teachers.Commands.SetTeacherAccount;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class SetTeacherAccountValidatorTests
{
    [Fact]
    public void Validate_EmptyTeacherId_ReportsErrorOnTeacherId()
    {
        var result = new SetTeacherAccountValidator().Validate(new SetTeacherAccountCommand(Guid.Empty, Guid.CreateVersion7(), 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "TeacherId");
    }

    [Fact]
    public void Validate_NullUserId_HasNoErrors()
    {
        var result = new SetTeacherAccountValidator().Validate(new SetTeacherAccountCommand(Guid.CreateVersion7(), null, 0));

        Assert.True(result.IsValid);
    }
}
