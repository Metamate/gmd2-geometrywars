using System;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class SeekBehaviour : IComponent
{
    private readonly Func<Vector2> _getTargetPosition;
    private readonly float _acceleration;
    private MovementComponent _movement;

    public SeekBehaviour(Func<Vector2> getTargetPosition, float acceleration)
    {
        _getTargetPosition = getTargetPosition;
        _acceleration = acceleration;
    }

    public void OnAdded(Entity owner)
    {
        _movement = owner.Movement;
    }

    public void Update(Entity owner)
    {
        if (owner is not Enemy enemy || !enemy.IsActive || _movement == null)
            return;

        _movement.Velocity += (_getTargetPosition() - owner.Position).ScaleTo(_acceleration);
    }
}
