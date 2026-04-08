using System;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class SeekBehaviour : IComponent
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

    public void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
    }

    public void Update(Entity owner)
    {
        if (owner is not Enemy enemy || !enemy.IsActive)
            return;

        _rigidbody.Velocity += (_getTargetPosition() - _transform.Position).ScaleTo(_acceleration);

        if (_rigidbody.Velocity.LengthSquared() > 0.01f)
            _transform.Orientation = _rigidbody.Velocity.ToAngle();
    }
}
