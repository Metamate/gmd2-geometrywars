using System;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class SeekBehaviour : IComponent
{
    private readonly Func<Vector2> _getTargetPosition;
    private readonly float _acceleration;
    
    private TransformComponent _transform;
    private MovementComponent _movement;

    public SeekBehaviour(Func<Vector2> getTargetPosition, float acceleration)
    {
        _getTargetPosition = getTargetPosition;
        _acceleration = acceleration;
    }

    public void OnAdded(Entity owner)
    {
        _transform = owner.GetComponent<TransformComponent>();
        _movement = owner.GetComponent<MovementComponent>();
    }

    public void Update(Entity owner)
    {
        if (owner is not Enemy enemy || !enemy.IsActive || _movement == null || _transform == null)
            return;

        _movement.Velocity += (_getTargetPosition() - _transform.Position).ScaleTo(_acceleration);

        if (_movement.Velocity.LengthSquared() > 0.01f)
            _transform.Orientation = _movement.Velocity.ToAngle();
    }
}
