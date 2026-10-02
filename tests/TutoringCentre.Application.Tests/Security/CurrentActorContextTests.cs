using TutoringCentre.Application.Common.Security;

namespace TutoringCentre.Application.Tests.Security;

public sealed class CurrentActorContextTests
{
    [Fact]
    public void Actor_ByDefault_IsAnonymous()
    {
        var context = new CurrentActorContext();

        Assert.IsType<AnonymousActor>(context.Actor);
    }

    [Fact]
    public void Set_ThenActor_ReturnsTheSetActor()
    {
        // Arrange
        var context = new CurrentActorContext();
        var systemActor = new SystemActor(Guid.CreateVersion7());

        // Act
        context.Set(systemActor);

        // Assert
        Assert.Same(systemActor, context.Actor);
    }

    [Fact]
    public void Set_CalledTwice_ThrowsInvalidOperationException()
    {
        var context = new CurrentActorContext();
        context.Set(new SystemActor(null));

        Assert.Throws<InvalidOperationException>(() => context.Set(new SystemActor(Guid.CreateVersion7())));
    }
}
