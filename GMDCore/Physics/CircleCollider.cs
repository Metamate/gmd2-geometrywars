namespace GMDCore.Physics;

// Circle-shaped collider. Collision dispatch is handled by CollisionRegistry,
// which maps (Type, Type) pairs to handler delegates.
public sealed class CircleCollider : Collider
{
    public float Radius { get; set; }

    public CircleCollider(float radius)
    {
        Radius = radius;
    }
}

