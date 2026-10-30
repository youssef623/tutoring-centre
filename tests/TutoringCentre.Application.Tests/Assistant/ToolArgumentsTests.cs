using System.Text.Json;
using TutoringCentre.Application.Assistant.Tools;
namespace TutoringCentre.Application.Tests.Assistant;
public sealed class ToolArgumentsTests
{
    internal static JsonElement Schema => JsonDocument.Parse("""{"type":"object","additionalProperties":false,"properties":{"child":{"type":"string","maxLength":20,"pattern":"^(child_[0-9]+|me)$"},"count":{"type":"integer"},"date":{"type":"string","format":"date"},"kind":{"type":"string","enum":["present","absent"]}},"required":["child"]}""").RootElement.Clone();
    [Theory]
    [InlineData("{\"child\":\"child_1\"}", true)]
    [InlineData("{\"child\":\"child_1\",\"studentId\":\"x\"}", false)]
    [InlineData("{}", false)]
    [InlineData("{\"child\":1}", false)]
    [InlineData("{\"child\":\"child_111111111111111111111111111\"}", false)]
    [InlineData("{\"child\":\"child_1\",\"child\":\"child_2\"}", false)]
    [InlineData("{\"child\":", false)]
    [InlineData("{\"child\":{\"a\":{\"b\":{\"c\":{}}}}}", false)]
    [InlineData("{\"child\":\"me\",\"count\":1.5}", false)]
    [InlineData("{\"child\":\"me\",\"date\":\"2026-02-30\"}", false)]
    [InlineData("{\"child\":\"me\",\"kind\":\"other\"}", false)]
    [InlineData("{\"child\":\"me\",\"date\":\"2026-02-28\",\"kind\":\"present\",\"count\":2}", true)]
    public void StrictParseReturnsCleanResult(string json, bool success) => Assert.Equal(success, ToolArguments.Parse(Schema, json).IsSuccess);
}
