using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

/// <summary>
/// Component that steers an entity in random directions.
/// Translates the original coroutine logic into a simple state-based component.
/// </summary>
public sealed class WanderBehaviour : IComponent
{
    private float _direction;
    private int _stepCounter;

    public WanderBehaviour()
    {
        _direction = Random.Shared.NextFloat(0, MathHelper.TwoPi);
    }

    public void Update(Entity owner)
    {
        if (owner is not Enemy enemy || !enemy.IsActive)
            return;

        // Update direction after N steps.
        if (_stepCounter-- <= 0)
        {
            _stepCounter = GameSettings.Enemy.WandererStepsPerTick;

            _direction += Random.Shared.NextFloat(-GameSettings.Enemy.WandererTurnRate, GameSettings.Enemy.WandererTurnRate);
            _direction = MathHelper.WrapAngle(_direction);

            var bounds = FrameContext.Viewport.Bounds;
            bounds.Inflate(-owner.Size.X, -owner.Size.Y);
            
            // Turn back towards center if hitting screen boundaries
            if (!bounds.Contains(owner.Position.ToPoint()))
                _direction = (FrameContext.ScreenSize / 2 - owner.Position).ToAngle() + 
                             Random.Shared.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
        }

        owner.Velocity += MathUtil.FromPolar(_direction, GameSettings.Enemy.WandererVelocity);
        owner.Orientation -= GameSettings.Enemy.WandererOrientationDecay;
    }
}
