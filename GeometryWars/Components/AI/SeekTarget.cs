using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.AI;

// Steers the owner toward a target position.
public sealed class SeekTarget : Component
{
    private readonly Func<Vector2> _getTargetPosition;
    private readonly float _acceleration;
    
    private Transform _transform;
    private Rigidbody _rigidbody;

    public SeekTarget(Func<Vector2> getTargetPosition, float acceleration)
    {
        _getTargetPosition = getTargetPosition;
        _acceleration = acceleration;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _rigidbody = owner.GetComponent<Rigidbody>();
    }

    public override void Update(Entity owner)
    {
        _rigidbody.AddForce((_getTargetPosition() - _transform.Position).ScaleTo(_acceleration));

        if (_rigidbody.Velocity.LengthSquared() > 0.01f)
            _transform.Orientation = _rigidbody.Velocity.ToAngle();
    }
}
