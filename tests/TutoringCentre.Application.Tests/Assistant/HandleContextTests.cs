using System.Text.Json;
using TutoringCentre.Application.Assistant.Context;
using TutoringCentre.Application.Assistant.Handles;
namespace TutoringCentre.Application.Tests.Assistant;
public sealed class HandleContextTests
{
    [Theory] [InlineData(false)] [InlineData(true)]
    public void NamesAndHandlesSurviveButIdentifiersNeverEnterOutput(bool student)
    {
        var id = Guid.NewGuid(); var name = "أحمد محمود";
        var context = ContextBuilder.Build(new("Parent " + id, student ? "student" : "parent", new(2026,10,30), "Africa/Cairo", [], id.ToString(), HandleResolver.Mint([new(id,name)],student)), 10);
        Assert.Contains((student ? "me" : "child_1") + ": " + name, context.System, StringComparison.Ordinal);
        Assert.DoesNotMatch(@"(?i)[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}", JsonSerializer.Serialize(context));
    }
    [Fact] public void EmptySetExplainsNoChildrenLinked()
    {
        var context = ContextBuilder.Build(new("Parent", "parent", new(2026,10,30), "Africa/Cairo", [], "hello", HandleResolver.Mint([])), 10);
        Assert.Contains("No children linked", context.System, StringComparison.Ordinal);
        AssertNoIds(context);
    }
    [Fact] public void ChildrenAreListedInMintedOrder()
    {
        var context = ContextBuilder.Build(new("Parent", "parent", new(2026,10,30), "Africa/Cairo", [], "hello", HandleResolver.Mint([new(Guid.NewGuid(),"Salma"),new(Guid.NewGuid(),"Ahmed")])), 10);
        Assert.Contains("child_1: Ahmed\nchild_2: Salma",context.System,StringComparison.Ordinal);
        AssertNoIds(context);
    }
    private static void AssertNoIds(ContextOutput context) => Assert.DoesNotMatch(@"(?i)[0-9a-f]{8}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{4}-[0-9a-f]{12}",JsonSerializer.Serialize(context));
}
