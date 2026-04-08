using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class ScreenWrap : IComponent
{
    private TransformComponent _transform;

    public void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    public void Update(Entity owner)
    {
        if (_transform == null) return;

        var pos = _transform.Position;
        var size = FrameContext.ScreenSize;

        if (pos.X < 0) pos.X += size.X;
        else if (pos.X > size.X) pos.X -= size.X;

        if (pos.Y < 0) pos.Y += size.Y;
        else if (pos.Y > size.Y) pos.Y -= size.Y;

        _transform.Position = pos;
    }
}
