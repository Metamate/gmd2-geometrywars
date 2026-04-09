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
    private readonly FrameInfo _frame;

    public ScreenClampBehaviour(Vector2 size, FrameInfo frame)
    {
        _size = size;
        _frame = frame;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void PostUpdate(Entity owner)
    {
        _transform.Position = Vector2.Clamp(_transform.Position,
            _size / 2,
            _frame.ScreenSize - _size / 2);
    }
}
