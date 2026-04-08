namespace GeometryWars;

public sealed class CircleColliderComponent : ColliderComponent
{
    public override ColliderShape Shape => ColliderShape.Circle;
    public float Radius { get; set; }

    public CircleColliderComponent(float radius)
    {
        Radius = radius;
    }
}
