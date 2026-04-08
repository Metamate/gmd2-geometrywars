using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Physics;

public sealed class ScreenWrap : Component
{
    private TransformComponent _transform;

    public override void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void Update(Entity owner)
    {
        var pos = _transform.Position;
        var size = FrameContext.ScreenSize;

        if (pos.X < 0) pos.X += size.X;
        else if (pos.X > size.X) pos.X -= size.X;

        if (pos.Y < 0) pos.Y += size.Y;
        else if (pos.Y > size.Y) pos.Y -= size.Y;

        _transform.Position = pos;
    }
}
