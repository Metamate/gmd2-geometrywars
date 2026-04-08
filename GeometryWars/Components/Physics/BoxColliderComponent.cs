using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Physics;

// Axis-aligned box collider. Collision dispatch is handled by CollisionRegistry,
// which maps (Type, Type) pairs to handler delegates.
public sealed class BoxColliderComponent : ColliderComponent
{
    public Vector2 Size { get; set; }

    public BoxColliderComponent(Vector2 size)
    {
        Size = size;
    }
}
