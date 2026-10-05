using TutoringCentre.Application.Common.Security;
using TutoringCentre.Domain.Identity;

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

    [Fact]
    public void Reauthenticate_AfterSet_ReplacesTheActor()
    {
        // Arrange: mirrors a request that arrived with an existing session (ActorMiddleware already set the
        // actor from the inbound cookie) and then logs in as a different identity.
        var context = new CurrentActorContext();
        context.Set(new StaffActor(Guid.CreateVersion7(), Guid.CreateVersion7(), StaffRole.Owner));
        var reauthenticated = new StaffActor(Guid.CreateVersion7(), null, null);

        // Act
        context.Reauthenticate(reauthenticated);

        // Assert
        Assert.Same(reauthenticated, context.Actor);
    }

    [Fact]
    public void Reauthenticate_ThenSet_ThrowsInvalidOperationException()
    {
        // Reauthenticate still leaves the scope "set": nothing after login may further replace the actor.
        var context = new CurrentActorContext();
        context.Reauthenticate(new StaffActor(Guid.CreateVersion7(), null, null));

        Assert.Throws<InvalidOperationException>(() => context.Set(new SystemActor(null)));
    }
}
