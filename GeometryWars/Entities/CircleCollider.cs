using GeometryWars.Components;

namespace GeometryWars;

/// <summary>
/// Component that defines a circular collision volume for an entity.
/// Systems like EntityManager use this component to perform collision checks.
/// </summary>
public sealed class CircleColliderComponent : IComponent
{
    public float Radius { get; set; }
    
    // An optional flag to temporarily disable collision without removing the component.
    public bool IsActive { get; set; } = true;

    public CircleColliderComponent(float radius)
    {
        Radius = radius;
    }

    public void Update(Entity owner) { }
}
