using TutoringCentre.Application.Attendance.Commands.RecordScan;
using TutoringCentre.Domain.Attendance;

namespace TutoringCentre.Application.Tests.Attendance;

public sealed class RecordScanValidatorTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("foreign-qr")]
    [InlineData("TCM1.short")]
    [InlineData("https://example.test/scan")]
    public void InvalidToken_UsesTokenFieldAndCode(string? token)
    {
        var result = new RecordScanValidator().Validate(new RecordScanCommand(Guid.NewGuid(), token!));
        var error = Assert.Single(result.Errors);
        Assert.Equal("token", error.PropertyName);
        Assert.Equal("card.invalid_format", error.ErrorCode);
    }

    [Fact]
    public void EmptySession_IsRejected()
    {
        var result = new RecordScanValidator().Validate(new RecordScanCommand(Guid.Empty, CardToken.Generate().Reveal()));
        Assert.Equal("SessionId", Assert.Single(result.Errors).PropertyName);
    }

    [Fact]
    public void ValidToken_PassesAndCommandDisplayHidesCredential()
    {
        var token = CardToken.Generate().Reveal();
        var command = new RecordScanCommand(Guid.NewGuid(), token);
        Assert.True(new RecordScanValidator().Validate(command).IsValid);
        Assert.DoesNotContain(token, command.ToString());
    }
}
