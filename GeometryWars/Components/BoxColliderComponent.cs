using Microsoft.Xna.Framework;

namespace GeometryWars;

public sealed class BoxColliderComponent : ColliderComponent
{
    public override ColliderShape Shape => ColliderShape.Box;
    
    // Width and Height of the box
    public Vector2 Size { get; set; }

    public BoxColliderComponent(Vector2 size)
    {
        Size = size;
    }
}
