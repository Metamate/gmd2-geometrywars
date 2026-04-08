using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.AI;

/// <summary>
/// AI component that steers an entity in random directions.
/// </summary>
public sealed class WanderBehaviour : Component
{
    private float _direction;
    private int _stepCounter;
    
    private TransformComponent _transform;
    private RigidbodyComponent _rigidbody;

    public WanderBehaviour()
    {
        _direction = Random.Shared.NextFloat(0, MathHelper.TwoPi);
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

        if (_stepCounter-- <= 0)
        {
            _stepCounter = GameSettings.Enemy.WandererStepsPerTick;

            _direction += Random.Shared.NextFloat(-GameSettings.Enemy.WandererTurnRate, GameSettings.Enemy.WandererTurnRate);
            _direction = MathHelper.WrapAngle(_direction);

            var bounds = FrameContext.Viewport.Bounds;
            bounds.Inflate(-Art.Wanderer.Width, -Art.Wanderer.Height);
            
            if (!bounds.Contains(_transform.Position.ToPoint()))
            {
                _direction = (FrameContext.ScreenSize / 2 - _transform.Position).ToAngle() + 
                             Random.Shared.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
            }
        }

        _rigidbody.AddForce(MathUtil.FromPolar(_direction, GameSettings.Enemy.WandererVelocity));
        _transform.Orientation -= GameSettings.Enemy.WandererOrientationDecay;
    }
}
