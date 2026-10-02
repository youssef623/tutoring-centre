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
}
