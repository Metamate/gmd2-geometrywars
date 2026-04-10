using GMDCore.ECS;
using Microsoft.Xna.Framework;

namespace GMDCore.ECS.Components;

// Defines an entity's position, rotation, and scale.
public sealed class Transform : Component
{
    public Vector2 Position { get; set; }
    public float Orientation { get; set; }
    public float Scale { get; set; } = 1f;

    public Transform(Vector2 position, float orientation = 0f, float scale = 1f)
    {
        Position = position;
        Orientation = orientation;
        Scale = scale;
    }
}

