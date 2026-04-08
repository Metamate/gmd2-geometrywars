namespace GeometryWars.Components;

/// <summary>
/// Allows a component to react to collisions.
/// </summary>
public interface ICollisionComponent : IComponent
{
    void OnCollision(Entity owner, Entity other);
}
