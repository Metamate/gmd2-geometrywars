using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Physics;

// Prevents an entity from leaving the screen.
public sealed class ScreenClampBehaviour : Component
{
    private TransformComponent _transform;
    private readonly Vector2 _size;

    public ScreenClampBehaviour(Vector2 size)
    {
        _size = size;
    }

    public override void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void Update(Entity owner)
    {
        _transform.Position = Vector2.Clamp(_transform.Position,
            _size / 2,
            FrameContext.ScreenSize - _size / 2);
    }
}
