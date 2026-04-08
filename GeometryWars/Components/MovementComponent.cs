using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

/// <summary>
/// Component that handles an entity's movement state and physics integration.
/// This component "owns" the velocity and orientation data.
/// </summary>
public sealed class MovementComponent : IComponent
{
    public Vector2 Velocity;
    public float Orientation;
    
    private readonly float _damping;
    private readonly bool _clampToScreen;

    public MovementComponent(float damping = 1f, bool clampToScreen = false)
    {
        _damping = damping;
        _clampToScreen = clampToScreen;
    }

    public void Update(Entity owner)
    {
        // 1. Integration: Apply velocity to position
        owner.Position += Velocity;

        // 2. Optional: Clamp to screen boundaries
        if (_clampToScreen)
        {
            owner.Position = Vector2.Clamp(owner.Position,
                owner.Size / 2,
                FrameContext.ScreenSize - owner.Size / 2);
        }

        // 3. Friction: Apply damping
        Velocity *= _damping;
        
        // Note: Orientation is now managed by "Brain" components (AI/Input) 
        // to support twin-stick aiming logic correctly.
    }
}
