namespace TutoringCentre.Domain.Common;

/// <summary>Base class for entities: identity is a UUIDv7 assigned in the application at construction.</summary>
public abstract class Entity
{
    protected Entity() => Id = Guid.CreateVersion7();

    /// <summary>Private setter: only the entity itself (and EF Core when loading, Day 8) can set it.</summary>
    public Guid Id { get; private set; }
}
