namespace GeometryWars.Components;

/// <summary>
/// Base class for all collision volumes.
/// Data-only: defines the dimensions of a shape. 
/// The type of the class itself identifies the shape to the CollisionRegistry.
/// </summary>
public abstract class ColliderComponent : IComponent
{
    public bool IsActive { get; set; } = true;

    public void Update(Entity owner) { }
}
