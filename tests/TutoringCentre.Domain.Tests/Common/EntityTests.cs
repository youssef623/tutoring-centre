using TutoringCentre.Domain.Common;

namespace TutoringCentre.Domain.Tests.Common;

public sealed class EntityTests
{
    // A minimal concrete entity, only for testing the abstract base class.
    private sealed class TestEntity : Entity;

    [Fact]
    public void NewEntities_GetDifferentIds()
    {
        var first = new TestEntity();
        var second = new TestEntity();

        Assert.NotEqual(first.Id, second.Id);
    }

    [Fact]
    public void Id_IsVersion7Uuid()
    {
        var entity = new TestEntity();

        Assert.Equal(7, entity.Id.Version);
    }
}
