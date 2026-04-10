using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Utils;

namespace GeometryWars.Components.Physics;

// Rotates an entity to face its current velocity.
public sealed class FaceVelocity : Component
{
    private Transform _transform;
    private Rigidbody _rigidbody;

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _rigidbody = owner.RequireComponent<Rigidbody>();
    }

    public override void PostUpdate(Entity owner)
    {
        if (_rigidbody.Velocity.LengthSquared() > 0.01f)
            _transform.Orientation = _rigidbody.Velocity.ToAngle();
    }
}
