namespace GeometryWars.Components;

// Data class describing an entity's collision shape.
// Detection is handled centrally by EntityManager (O(n²) circle-pair test).
// When two colliders overlap, EntityManager calls OnCollision(other) on both entities.
// Each entity defines its own response by overriding Entity.OnCollision().
public sealed class CircleCollider
{
    public float Radius { get; }

    public CircleCollider(float radius)
    {
        Radius = radius;
    }
}
