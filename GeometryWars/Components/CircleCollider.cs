using System;

namespace GeometryWars.Components;

// Data class describing an entity's collision shape and response.
// Not a component — it has no per-frame behaviour. Detection is handled
// centrally by EntityManager (O(n²) circle-pair test); the response callback
// is set in each entity's constructor so collision rules live next to the
// entity they belong to, not in a central collision manager.
public sealed class CircleCollider
{
    public float Radius { get; }
    public Action<Entity> OnCollide { get; }

    public CircleCollider(float radius, Action<Entity> onCollide = null)
    {
        Radius = radius;
        OnCollide = onCollide;
    }
}
