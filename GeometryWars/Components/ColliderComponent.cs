using GeometryWars.Components;

namespace GeometryWars;

public enum ColliderShape { Circle, Box }

/// <summary>
/// Base class for all collision volumes.
/// Data-only: defines the shape type but not the collision logic.
/// </summary>
public abstract class ColliderComponent : IComponent
{
    public abstract ColliderShape Shape { get; }
    public bool IsActive { get; set; } = true;

    public void Update(Entity owner) { }
}
