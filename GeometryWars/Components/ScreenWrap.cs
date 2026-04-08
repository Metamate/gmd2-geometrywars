using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

/// <summary>
/// Component that wraps an entity around screen edges if it leaves the viewport.
/// </summary>
public sealed class ScreenWrap : IComponent
{
    public void OnAdded(Entity owner) { }

    public void Update(Entity owner)
    {
        var pos = owner.Position;
        var size = FrameContext.ScreenSize;

        if (pos.X < 0) pos.X += size.X;
        else if (pos.X > size.X) pos.X -= size.X;

        if (pos.Y < 0) pos.Y += size.Y;
        else if (pos.Y > size.Y) pos.Y -= size.Y;

        owner.Position = pos;
    }
}
