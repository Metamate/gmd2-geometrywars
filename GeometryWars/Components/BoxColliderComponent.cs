using Microsoft.Xna.Framework;

namespace GeometryWars;

public sealed class BoxColliderComponent : ColliderComponent
{
    public Vector2 Size { get; set; }

    public BoxColliderComponent(Vector2 size)
    {
        Size = size;
    }
}
