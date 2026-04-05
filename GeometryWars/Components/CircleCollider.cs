using System;

namespace GeometryWars.Components;

// Passive collision component. Stores the radius and the collision response callback.
// Detection is handled centrally by EntityManager (cheap n^2 circle test).
// The response — what to DO on collision — lives here as a lambda set in each
// entity's constructor, keeping collision rules co-located with the entity they belong to.
public sealed class CircleCollider : IComponent
{
    public float Radius { get; }
    public Action<Entity> OnCollide { get; }

    public CircleCollider(float radius, Action<Entity> onCollide = null)
    {
        Radius = radius;
        OnCollide = onCollide;
    }

    // Passive — detection and dispatch happen in EntityManager.HandleCollisions().
    public void Update(Entity owner) { }
}
