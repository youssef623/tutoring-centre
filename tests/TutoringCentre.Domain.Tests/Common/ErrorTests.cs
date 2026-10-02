using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Common;

public sealed class ErrorTests
{
    [Fact]
    public void Validation_WithFields_ExposesTheFields()
    {
        // Arrange
        var fields = new Dictionary<string, string[]> { ["slug"] = ["Slug is invalid."] };

        // Act
        var error = Error.Validation("validation.failed", "One or more fields are invalid.", fields);

        // Assert
        Assert.Equal(ErrorKind.Validation, error.Kind);
        Assert.NotNull(error.Fields);
        Assert.Equal(["Slug is invalid."], error.Fields["slug"]);
    }

    [Fact]
    public void Validation_WithoutFields_HasNullFields()
    {
        // Act
        var error = Error.Validation("centre.name_required", "Centre name is required.");

        // Assert
        Assert.Null(error.Fields);
    }
}
