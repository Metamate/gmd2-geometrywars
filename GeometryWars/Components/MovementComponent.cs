using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

/// <summary>
/// Component that handles physics integration (Velocity).
/// Interacts with the TransformComponent to update Position.
/// </summary>
public sealed class MovementComponent : IComponent
{
    public Vector2 Velocity;
    
    private TransformComponent _transform; // Cached
    private readonly float _damping;

    public MovementComponent(float damping = 1f)
    {
        _damping = damping;
    }

    public void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    public void Update(Entity owner)
    {
        _transform ??= owner.Transform;
        if (_transform == null) return;

        // 1. Integration
        _transform.Position += Velocity;

        // 2. Friction
        Velocity *= _damping;
    }
}
