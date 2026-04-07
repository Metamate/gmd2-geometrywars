using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

// Applies velocity to position each frame, with optional screen clamping and damping.
// Damping = 0 clears velocity after moving (input-driven entities: no momentum).
// Damping = 0.8 slows the entity each frame (gradual deceleration).
// Damping = 1 preserves full velocity.
public sealed class VelocityMover : IComponent
{
    private readonly float _damping;
    private readonly bool _clampToScreen;

    public VelocityMover(float damping = 1f, bool clampToScreen = false)
    {
        _damping = damping;
        _clampToScreen = clampToScreen;
    }

    public void Update(Entity owner)
    {
        owner.Position += owner.Velocity;

        if (_clampToScreen)
            owner.Position = Vector2.Clamp(owner.Position,
                owner.Size / 2,
                FrameContext.ScreenSize - owner.Size / 2);

        owner.Velocity *= _damping;
    }
}
