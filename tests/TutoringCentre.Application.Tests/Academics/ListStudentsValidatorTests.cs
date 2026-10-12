using TutoringCentre.Application.Academics.Students.Queries.ListStudents;

namespace TutoringCentre.Application.Tests.Academics;

public sealed class ListStudentsValidatorTests
{
    [Fact]
    public void Validate_PageSizeZero_ReportsErrorOnPageSize()
    {
        var result = new ListStudentsValidator().Validate(new ListStudentsQuery(null, false, null, 0));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_PageSizeAboveHundred_ReportsErrorOnPageSize()
    {
        var result = new ListStudentsValidator().Validate(new ListStudentsQuery(null, false, null, 101));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "PageSize");
    }

    [Fact]
    public void Validate_GarbageCursor_ReportsErrorOnCursor()
    {
        var result = new ListStudentsValidator().Validate(new ListStudentsQuery(null, false, "not-a-real-cursor", 25));

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, error => error.PropertyName == "Cursor");
    }

    [Fact]
    public void Validate_NoCursorAndValidPageSize_HasNoErrors()
    {
        var result = new ListStudentsValidator().Validate(new ListStudentsQuery("omar", true, null, 25));

        Assert.True(result.IsValid);
    }
}
