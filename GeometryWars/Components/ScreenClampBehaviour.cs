using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

/// <summary>
/// Component that prevents an entity from leaving the screen.
/// Separating this from movement allows for better reusability.
/// </summary>
public sealed class ScreenClampBehaviour : IComponent
{
    private TransformComponent _transform;
    private readonly Vector2 _size;

    public ScreenClampBehaviour(Vector2 size)
    {
        _size = size;
    }

    public void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    public void Update(Entity owner)
    {
        _transform ??= owner.Transform;
        if (_transform == null) return;

        _transform.Position = Vector2.Clamp(_transform.Position,
            _size / 2,
            FrameContext.ScreenSize - _size / 2);
    }
}
