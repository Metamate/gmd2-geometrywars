using System;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class SeekBehaviour : IComponent
{
    private readonly Func<Vector2> _getTargetPosition;
    private readonly float _acceleration;

    public SeekBehaviour(Func<Vector2> getTargetPosition, float acceleration)
    {
        _getTargetPosition = getTargetPosition;
        _acceleration = acceleration;
    }

    public void Update(Entity owner)
    {
        if (owner is not Enemy enemy || !enemy.IsActive)
            return;

        // PERFORMANCE: Direct cached access
        var movement = owner.Movement;
        if (movement == null) return;

        movement.Velocity += (_getTargetPosition() - owner.Position).ScaleTo(_acceleration);
        
        if (movement.Velocity.LengthSquared() > 0.01f)
            movement.Orientation = movement.Velocity.ToAngle();
    }
}
