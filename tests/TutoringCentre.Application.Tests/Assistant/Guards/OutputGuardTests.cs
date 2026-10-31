using TutoringCentre.Application.Assistant.Guards;
namespace TutoringCentre.Application.Tests.Assistant.Guards;
public sealed class OutputGuardTests
{
    [Theory]
    [InlineData("child_1")] [InlineData("me")] [InlineData("get_attendance")]
    [InlineData("73fa5587-02ef-4307-b65e-6c3dbe559c26")] [InlineData("{ status: present }")]
    public void IdentifiersAreBlocked(string text) => Assert.Equal("guard.identifier", IdentifierGuard.Check(text).Reason);
    [Theory]
    [InlineData("Ahmed arrived.")] [InlineData("أحمد بخير")] [InlineData("Ahmed tamam")]
    public void NormalTextIsAllowed(string text) => Assert.True(IdentifierGuard.Check(text).Allowed);
    [Theory]
    [InlineData("Ahmed attended")] [InlineData("أحمد حضر")] [InlineData("Ahmed present")]
    public void AttendanceRequiresEvidence(string text)
    {
        Assert.Equal("guard.ungrounded", GroundingGuard.Check(text, "", ["Ahmed"], [], false, "").Reason);
        Assert.True(GroundingGuard.Check(text, "", ["Ahmed"], [], true, "").Allowed);
    }
    [Fact]
    public void ForeignNameHonorsUserAndToolFacts()
    {
        Assert.Equal("guard.foreign_name", GroundingGuard.Check("Omar Ali is well", "Hi", ["Ahmed"], ["Omar Ali"], false, "").Reason);
        Assert.True(GroundingGuard.Check("Omar Ali is well", "How is Omar Ali?", [], ["Omar Ali"], false, "").Allowed);
        Assert.True(GroundingGuard.Check("Omar Ali is well", "Hi", ["Omar Ali"], ["Omar Ali"], false, "").Allowed);
        Assert.True(GroundingGuard.Check("Omar Ali is well", "Hi", [], ["Omar Ali"], false, "Omar Ali").Allowed);
    }
    [Theory]
    [InlineData("")] [InlineData("<b>Hello</b>")] [InlineData("https://example.test")]
    [InlineData("**Hello**")]
    public void InvalidFormatIsBlocked(string text) => Assert.Equal("guard.format", FormatGuard.Check(text, 1000).Reason);
    [Fact] public void LengthIsBounded() => Assert.False(FormatGuard.Check(new string('a', 1001), 1000).Allowed);
    [Theory]
    [InlineData("أهلاً كيف الحال", true)] [InlineData("hello", false)] [InlineData("ana 3ayez mosa3da", false)]
    public void LanguageUsesMajorityScript(string text, bool arabic) => Assert.Equal(arabic, FallbackReplies.For(text).StartsWith("عذراً", StringComparison.Ordinal));
}
