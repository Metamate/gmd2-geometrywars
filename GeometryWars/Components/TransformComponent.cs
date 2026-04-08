using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class TransformComponent : IComponent
{
    public Vector2 Position { get; set; }
    public float Orientation { get; set; }
    public float Scale { get; set; } = 1f;

    public TransformComponent(Vector2 position, float orientation = 0f, float scale = 1f)
    {
        Position = position;
        Orientation = orientation;
        Scale = scale;
    }

    public void OnAdded(Entity owner) { }
    public void Update(Entity owner) { }
}
