using System.Text.Json;
using TutoringCentre.Application.Assistant.Context;
using TutoringCentre.Application.Assistant.Models;
using TutoringCentre.Application.Tests.Assistant;
using TutoringCentre.Evaluation.Models;
namespace TutoringCentre.Evaluation.Tests;

public sealed class CassetteTests
{
    [Fact]
    public async Task Recording_EmptyCassetteIsRefused()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        await Assert.ThrowsAsync<InvalidDataException>(() => (Cassette with { Entries = [] }).WriteAsync(path, []));
        Assert.False(File.Exists(path));
    }
    public static ModelRequest Request => new("child_1: Ahmed\nchild_2: Salma", [new(ModelRole.User, "Hello")], [new("get_attendance", "description", JsonSerializer.SerializeToElement(new { type = "object" }))], 100);
    public static ModelResponse Response => new(null, [new("a", "get_attendance", JsonSerializer.SerializeToElement(new { childHandle = "child_1" }))], new(1, 2), ModelStopReason.ToolCalls);
    public static Cassette Cassette => new("E1", AssistantPrompt.Version, "synthetic-test", [new(CassetteKey.Create(AssistantPrompt.Version, Request), Request, Response)]);
    [Fact]
    public async Task Recording_UsesExplicitRequestHashAndRoundTrips()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        try
        {
            await Cassette.WriteAsync(path, []);
            using var json = JsonDocument.Parse(await File.ReadAllTextAsync(path));
            var entry = json.RootElement.GetProperty("entries")[0];
            Assert.Equal(Cassette.Entries[0].RequestHash, entry.GetProperty("requestHash").GetString());
            Assert.False(entry.TryGetProperty("key", out _));
            Assert.Equal(Cassette.Entries[0].RequestHash, Cassette.Read(path).Entries[0].RequestHash);
        }
        finally { File.Delete(path); }
    }
    [Fact] public void Key_SameRequestSameKey() => Assert.Equal(CassetteKey.Create("v2", Request), CassetteKey.Create("v2", Request));
    [Theory]
    [InlineData("version")]
    [InlineData("system")]
    [InlineData("messages")]
    [InlineData("tools")]
    [InlineData("order")]
    [InlineData("budget")]
    [InlineData("schema")]
    public void Key_EveryRequiredFieldChangesKey(string field)
    {
        var r = field switch { "system" => Request with { System = Request.System + "!" }, "messages" => Request with { Messages = [new(ModelRole.User, "Hello!")] }, "tools" => Request with { Tools = [Request.Tools[0] with { Name = "other" }] }, "order" => Request with { System = "child_2: Salma\nchild_1: Ahmed" }, "budget" => Request with { MaxOutputTokens = 99 }, "schema" => Request with { Tools = [Request.Tools[0] with { Parameters = JsonSerializer.SerializeToElement(new { type = "string" }) }] }, _ => Request };
        Assert.NotEqual(CassetteKey.Create("v2", Request), CassetteKey.Create(field == "version" ? "v3" : "v2", r));
    }
    [Fact]
    public void Key_OrderedMessagesAndToolsMatter()
    {
        var request = Request with { Messages = [new(ModelRole.User, "first"), new(ModelRole.Assistant, "second")], Tools = [Request.Tools[0], Request.Tools[0] with { Name = "second_tool" }] };
        var key = CassetteKey.Create("v2", request);
        Assert.NotEqual(key, CassetteKey.Create("v2", request with { Messages = request.Messages.Reverse().ToArray() }));
        Assert.NotEqual(key, CassetteKey.Create("v2", request with { Tools = request.Tools.Reverse().ToArray() }));
    }
    [Fact] public async Task Replay_ExactToolResponse() { var r = await new ReplayLanguageModel(Cassette).GenerateAsync(Request, CancellationToken.None); Assert.True(r.IsSuccess); Assert.Equal(JsonSerializer.Serialize(Response), JsonSerializer.Serialize(r.Value)); }
    [Fact] public async Task Replay_ExactTextResponse() { var response = Response with { Text = "safe", ToolCalls = [], StopReason = ModelStopReason.Completed }; var c = Cassette with { Entries = [Cassette.Entries[0] with { Response = response }] }; Assert.Equal(response, (await new ReplayLanguageModel(c).GenerateAsync(Request, CancellationToken.None)).Value); }
    [Fact] public async Task Replay_MissingKeyNamesCase() { var r = await new ReplayLanguageModel(Cassette).GenerateAsync(Request with { System = "changed" }, CancellationToken.None); Assert.True(r.IsFailure); Assert.Equal("eval.cassette_missing", r.Error!.Code); Assert.Contains("E1", r.Error.Message, StringComparison.Ordinal); }
    [Fact]
    public async Task Recording_RoundTripPreservesRequest()
    {
        var model = new RecordingLanguageModel(new ScriptedProbeModel()); await model.GenerateAsync(Request, CancellationToken.None);
        Assert.Single(model.Entries); Assert.Equal(CassetteKey.Create("v2", Request), model.Entries[0].RequestHash);
    }
    [Theory]
    [InlineData("sk-abcdefghijklmnop")]
    [InlineData("configured-secret")]
    public async Task Recording_RefusesSecrets(string secret)
    {
        var c = Cassette with { Entries = [Cassette.Entries[0] with { Response = Response with { Text = secret } }] };
        var file = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        await Assert.ThrowsAsync<InvalidDataException>(() => c.WriteAsync(file, ["configured-secret"])); Assert.False(File.Exists(file));
    }
}
public sealed class ReplayContractTests : ModelContractTests { protected override ILanguageModel CreateModel() => new ReplayLanguageModel(CassetteTests.Cassette); }
public sealed class RecordingContractTests : ModelContractTests { protected override ILanguageModel CreateModel() => new RecordingLanguageModel(new ScriptedProbeModel()); }
