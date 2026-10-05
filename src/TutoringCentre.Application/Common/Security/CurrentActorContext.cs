namespace TutoringCentre.Application.Common.Security;

/// <summary>
/// Holds the actor for one scope. Starts anonymous; trusted edge code (Api middleware, job runner, seed) calls
/// <see cref="Set"/> exactly once. Handlers depend on <see cref="ICurrentActor"/>, which has no setter.
/// </summary>
public sealed class CurrentActorContext : ICurrentActor
{
    private bool _isSet;

    public Actor Actor { get; private set; } = new AnonymousActor();

    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is null.</exception>
    /// <exception cref="InvalidOperationException">The actor was already set in this scope.</exception>
    public void Set(Actor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        if (_isSet)
        {
            throw new InvalidOperationException("The actor for this scope has already been set and cannot be replaced.");
        }

        Actor = actor;
        _isSet = true;
    }

    /// <summary>
    /// Replaces the actor for this scope, even if one was already set. Login is the one place allowed to do
    /// this: a request can arrive with an existing session (ActorMiddleware already set the actor from the
    /// inbound cookie) and then authenticate as a different identity, which must immediately supersede it.
    /// </summary>
    /// <exception cref="ArgumentNullException"><paramref name="actor"/> is null.</exception>
    public void Reauthenticate(StaffActor actor)
    {
        ArgumentNullException.ThrowIfNull(actor);

        Actor = actor;
        _isSet = true;
    }
}
