namespace GeometryWars.Components.Physics;

// Circle-shaped collider. Collision dispatch is handled by CollisionRegistry,
// which maps (Type, Type) pairs to handler delegates.
public sealed class CircleColliderComponent : ColliderComponent
{
    public float Radius { get; set; }

    public CircleColliderComponent(float radius)
    {
        Radius = radius;
    }
}
