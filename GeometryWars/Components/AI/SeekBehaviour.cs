using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.AI;

// AI component that steers an entity towards a target position.
public sealed class SeekBehaviour : Component
{
    private readonly Func<Vector2> _getTargetPosition;
    private readonly float _acceleration;
    
    private TransformComponent _transform;
    private RigidbodyComponent _rigidbody;

    public SeekBehaviour(Func<Vector2> getTargetPosition, float acceleration)
    {
        _getTargetPosition = getTargetPosition;
        _acceleration = acceleration;
    }

    public override void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
    }

    public override void Update(Entity owner)
    {
        if (owner is not Enemy enemy || !enemy.IsActive)
            return;

        _rigidbody.AddForce((_getTargetPosition() - _transform.Position).ScaleTo(_acceleration));

        if (_rigidbody.Velocity.LengthSquared() > 0.01f)
            _transform.Orientation = _rigidbody.Velocity.ToAngle();
    }
}
