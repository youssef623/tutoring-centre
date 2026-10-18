using TutoringCentre.Application.Academics.Teachers.Commands.CreateTeacher;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class CreateTeacherValidatorTests
{
    [Fact]
    public void Validate_EmptyName_ReportsErrorOnFullName()
    {
        var result = new CreateTeacherValidator().Validate(new CreateTeacherCommand("", null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_NameOver120Characters_ReportsErrorOnFullName()
    {
        var result = new CreateTeacherValidator().Validate(new CreateTeacherCommand(new string('a', 121), null));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "FullName");
    }

    [Fact]
    public void Validate_PhoneOverRawLength_ReportsErrorOnPhone()
    {
        var result = new CreateTeacherValidator().Validate(new CreateTeacherCommand("Amina Hassan", new string('1', 31)));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Phone");
    }

    [Fact]
    public void Validate_ValidWithNoPhone_HasNoErrors()
    {
        var result = new CreateTeacherValidator().Validate(new CreateTeacherCommand("Amina Hassan", null));

        Assert.True(result.IsValid);
    }
}
