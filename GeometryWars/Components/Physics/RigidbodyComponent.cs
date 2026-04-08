using GeometryWars.Components.Core;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Physics;

/// <summary>
/// Component that handles physics integration by applying velocity to a Transform.
/// </summary>
public sealed class RigidbodyComponent : Component
{
    public Vector2 Velocity { get; set; }
    
    private TransformComponent _transform;
    private readonly float _damping;

    public RigidbodyComponent(float damping = 1f)
    {
        _damping = damping;
    }

    public override void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    /// <summary>
    /// Adds velocity to the body. Only works if the component is active.
    /// </summary>
    public void AddForce(Vector2 force)
    {
        if (!IsActive) return;
        Velocity += force;
    }

    public override void Update(Entity owner)
    {
        _transform.Position += Velocity;
        Velocity *= _damping;
    }
}
