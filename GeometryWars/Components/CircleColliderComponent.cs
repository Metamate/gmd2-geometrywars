namespace GeometryWars.Components;

public sealed class CircleColliderComponent : ColliderComponent
{
    public float Radius { get; set; }

    public CircleColliderComponent(float radius)
    {
        Radius = radius;
    }
}
