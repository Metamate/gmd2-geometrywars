using GeometryWars.Components.Core;
using GeometryWars.Entities;

namespace GeometryWars.Components.Physics;

// Rotates an entity to face its current velocity.
public sealed class FaceVelocityComponent : Component
{
    private TransformComponent _transform;
    private RigidbodyComponent _rigidbody;

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
    }

    public override void PostUpdate(Entity owner)
    {
        if (_rigidbody.Velocity.LengthSquared() > 0.01f)
            _transform.Orientation = _rigidbody.Velocity.ToAngle();
    }
}
