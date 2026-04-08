using System;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

/// <summary>
/// Component that steers an entity towards a target position.
/// Demonstrates how AI logic can be encapsulated into a reusable component.
/// </summary>
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
        // Gate logic: only update if the enemy has finished spawning
        if (owner is not Enemy enemy || !enemy.IsActive)
            return;

        owner.Velocity += (_getTargetPosition() - owner.Position).ScaleTo(_acceleration);
        
        if (owner.Velocity != Vector2.Zero)
            owner.Orientation = owner.Velocity.ToAngle();
    }
}
