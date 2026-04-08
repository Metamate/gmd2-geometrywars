using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

/// <summary>
/// Component that defines an entity's existence in space.
/// Unifies Position, Orientation (Rotation), and Scale.
/// </summary>
public sealed class TransformComponent : IComponent
{
    public Vector2 Position;
    public float Orientation;
    public float Scale = 1f;

    public TransformComponent(Vector2 position, float orientation = 0f, float scale = 1f)
    {
        Position = position;
        Orientation = orientation;
        Scale = scale;
    }

    public void OnAdded(Entity owner) { }
    public void Update(Entity owner) { }
}
