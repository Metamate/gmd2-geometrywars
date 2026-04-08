using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

/// <summary>
/// Component that handles physics integration (Velocity).
/// Similar to Unity's Rigidbody, it manages movement forces.
/// </summary>
public sealed class RigidbodyComponent : IComponent
{
    public Vector2 Velocity { get; set; }
    
    private TransformComponent _transform;
    private readonly float _damping;

    public RigidbodyComponent(float damping = 1f)
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

        // Integration
        _transform.Position += Velocity;

        // Damping (Friction)
        Velocity *= _damping;
    }
}
