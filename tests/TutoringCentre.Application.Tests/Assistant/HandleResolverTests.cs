using TutoringCentre.Application.Academics.Contracts;
using TutoringCentre.Application.Assistant.Handles;
namespace TutoringCentre.Application.Tests.Assistant;
public sealed class HandleResolverTests
{
    [Fact] public void AssignmentIsStableAndDuplicateNamesStayDistinct()
    {
        PortalChild[] children = [new(Guid.Parse("00000000-0000-0000-0000-000000000002"), "Ahmed"), new(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Ahmed")];
        var table = HandleResolver.Mint(children);
        Assert.Equal(children[1].Id, table.Resolve("child_1").Value);
        Assert.Equal(children[0].Id, table.Resolve("child_2").Value);
        Assert.Equal(table.Children, HandleResolver.Mint(children.Reverse().ToArray()).Children);
        Assert.Equal(children[1].Id, table.Resolve(" CHILD_1 ").Value);
        var raw = table.Resolve(children[1].Id.ToString());
        Assert.True(raw.IsFailure);
        Assert.Equal("tool.raw_id_rejected", raw.Error!.Code);
        foreach (var value in new[] { "child_0", "child_3", "", new string('x', 500), "Ahmed", "child_" }) Assert.Equal("tool.handle_unknown", table.Resolve(value).Error!.Code);
    }
    [Fact] public void StudentResolvesMeOnly()
    {
        var id = Guid.NewGuid(); var table = HandleResolver.Mint([new(id, "أحمد")], true);
        Assert.Equal(id, table.Resolve("me").Value); Assert.True(table.Resolve("child_1").IsFailure);
    }
}
