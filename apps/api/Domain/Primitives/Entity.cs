namespace Domain.Primitives;

/// <summary>
/// Base class for all domain entities.
/// Centralizes Guid identity, value-equality semantics,
/// and the private parameterless constructor required by EF Core.
/// </summary>
public abstract class Entity
{
    public Guid Id { get; protected init; } = Guid.NewGuid();

    // Required by EF Core — parameterless constructor must be accessible to it
    protected Entity() { }

    public override bool Equals(object? obj)
        => obj is Entity other && GetType() == other.GetType() && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();

    public static bool operator ==(Entity? left, Entity? right) => Equals(left, right);
    public static bool operator !=(Entity? left, Entity? right) => !Equals(left, right);
}
