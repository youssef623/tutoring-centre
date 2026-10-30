using TutoringCentre.Domain.Identity;

namespace TutoringCentre.Domain.Tests.Identity;

public sealed class PortalLinkTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void FactorySetsExactlyOneSubject(bool parent)
    {
        var user = Guid.NewGuid();
        var centre = Guid.NewGuid();
        var subject = Guid.NewGuid();
        var creator = Guid.NewGuid();
        var result = parent ? PortalLink.CreateForParent(user, centre, subject, creator)
            : PortalLink.CreateForStudent(user, centre, subject, creator);
        var link = result.Value;
        Assert.Equal(user, link.UserId);
        Assert.Equal(centre, link.CentreId);
        Assert.Equal(creator, link.CreatedBy);
        Assert.Equal(parent ? "parent" : "student", link.RoleKey);
        Assert.Equal(parent ? subject : (Guid?)null, link.ParentId);
        Assert.Equal(parent ? null : (Guid?)subject, link.StudentId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void BothFactoriesRejectEveryEmptyIdentifier(int position)
    {
        var ids = Enumerable.Range(0, 4).Select(_ => Guid.NewGuid()).ToArray();
        ids[position] = Guid.Empty;
        Assert.True(PortalLink.CreateForParent(ids[0], ids[1], ids[2], ids[3]).IsFailure);
        Assert.True(PortalLink.CreateForStudent(ids[0], ids[1], ids[2], ids[3]).IsFailure);
    }
}
