using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

// Teleports the entity to the opposite screen edge when it crosses a boundary.
//
// This is an alternative to VelocityMover(clampToScreen: true):
//   clampToScreen → entity stops at the edge and cannot leave
//   ScreenWrap    → entity exits one side and re-enters from the other
//
// The entity visually disappears for one frame as it crosses, which is acceptable
// for fast-moving objects (bullets, certain enemies).
//
// Usage: AddComponent(new ScreenWrap());
public sealed class ScreenWrap : IComponent
{
    public void Update(Entity owner)
    {
        var screen = FrameContext.ScreenSize;
        var half   = owner.Size / 2f;
        var pos    = owner.Position;

        if (pos.X < -half.X)          pos.X += screen.X + owner.Size.X;
        else if (pos.X > screen.X + half.X) pos.X -= screen.X + owner.Size.X;

        if (pos.Y < -half.Y)          pos.Y += screen.Y + owner.Size.Y;
        else if (pos.Y > screen.Y + half.Y) pos.Y -= screen.Y + owner.Size.Y;

        owner.Position = pos;
    }
}
