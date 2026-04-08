using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Physics;

public sealed class BoxColliderComponent : ColliderComponent
{
    public Vector2 Size { get; set; }

    public BoxColliderComponent(Vector2 size)
    {
        Size = size;
    }
}
