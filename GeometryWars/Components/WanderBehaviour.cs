using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class WanderBehaviour : IComponent
{
    private float _direction;
    private int _stepCounter;
    
    private TransformComponent _transform;
    private RigidbodyComponent _rigidbody;

    public WanderBehaviour()
    {
        _direction = Random.Shared.NextFloat(0, MathHelper.TwoPi);
    }

    public void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
    }

    public void Update(Entity owner)
    {
        _transform ??= owner.Transform;
        _rigidbody ??= owner.GetComponent<RigidbodyComponent>();

        if (owner is not Enemy enemy || !enemy.IsActive || _rigidbody == null || _transform == null)
            return;

        if (_stepCounter-- <= 0)
        {
            _stepCounter = GameSettings.Enemy.WandererStepsPerTick;

            _direction += Random.Shared.NextFloat(-GameSettings.Enemy.WandererTurnRate, GameSettings.Enemy.WandererTurnRate);
            _direction = MathHelper.WrapAngle(_direction);

            var bounds = FrameContext.Viewport.Bounds;
            // Use logical buffer to turn around before the physical clamp
            bounds.Inflate(-Art.Wanderer.Width, -Art.Wanderer.Height);
            
            if (!bounds.Contains(_transform.Position.ToPoint()))
            {
                _direction = (FrameContext.ScreenSize / 2 - _transform.Position).ToAngle() + 
                             Random.Shared.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
            }
        }

        // Apply movement force
        _rigidbody.Velocity += MathUtil.FromPolar(_direction, GameSettings.Enemy.WandererVelocity);
        
        // RESTORED: Continuous smooth spin independent of movement direction
        _transform.Orientation -= GameSettings.Enemy.WandererOrientationDecay;
    }
}
