using GeometryWars.Components.Core;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Physics;

// Handles velocity integration and damping.
public sealed class Rigidbody : Component
{
    public Vector2 Velocity { get; set; }
    
    private Transform _transform;
    private readonly float _damping;

    public Rigidbody(float damping = 1f)
    {
        _damping = damping;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public void AddForce(Vector2 force)
    {
        if (!IsActive) return;
        Velocity += force;
    }

    public override void Simulate(Entity owner)
    {
        _transform.Position += Velocity;
        Velocity *= _damping;
    }
}
