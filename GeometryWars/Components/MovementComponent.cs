using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

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

    public void OnAdded(Entity owner) { }

    public void Update(Entity owner)
    {
        owner.Position += Velocity;

        if (_clampToScreen)
        {
            owner.Position = Vector2.Clamp(owner.Position,
                owner.Size / 2,
                FrameContext.ScreenSize - owner.Size / 2);
        }

        Velocity *= _damping;
    }
}
