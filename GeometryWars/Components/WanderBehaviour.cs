using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class WanderBehaviour : IComponent
{
    private float _direction;
    private int _stepCounter;
    
    private TransformComponent _transform;
    private MovementComponent _movement;

    public WanderBehaviour()
    {
        _direction = Random.Shared.NextFloat(0, MathHelper.TwoPi);
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

        if (_stepCounter-- <= 0)
        {
            _stepCounter = GameSettings.Enemy.WandererStepsPerTick;

            _direction += Random.Shared.NextFloat(-GameSettings.Enemy.WandererTurnRate, GameSettings.Enemy.WandererTurnRate);
            _direction = MathHelper.WrapAngle(_direction);

            var bounds = FrameContext.Viewport.Bounds;
            
            if (!bounds.Contains(_transform.Position.ToPoint()))
                _direction = (FrameContext.ScreenSize / 2 - _transform.Position).ToAngle() + 
                             Random.Shared.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
        }

        _movement.Velocity += MathUtil.FromPolar(_direction, GameSettings.Enemy.WandererVelocity);
        _transform.Orientation = _direction;
    }
}
