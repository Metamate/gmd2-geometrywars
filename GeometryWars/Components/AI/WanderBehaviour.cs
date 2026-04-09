using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.AI;

// Steers an entity in random directions.
public sealed class WanderBehaviour : Component
{
    private readonly FrameInfo _frame;
    private readonly Vector2 _spriteSize;
    private float _direction;
    private int _stepCounter;
    
    private TransformComponent _transform;
    private RigidbodyComponent _rigidbody;

    public WanderBehaviour(FrameInfo frame, Vector2 spriteSize)
    {
        _frame = frame;
        _spriteSize = spriteSize;
        _direction = Random.Shared.NextFloat(0, MathHelper.TwoPi);
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
    }

    public override void Update(Entity owner)
    {
        if (_stepCounter-- <= 0)
        {
            _stepCounter = GameSettings.Enemy.WandererStepsPerTick;

            _direction += Random.Shared.NextFloat(-GameSettings.Enemy.WandererTurnRate, GameSettings.Enemy.WandererTurnRate);
            _direction = MathHelper.WrapAngle(_direction);

            var bounds = _frame.Viewport.Bounds;
            bounds.Inflate(-(int)_spriteSize.X, -(int)_spriteSize.Y);
            
            if (!bounds.Contains(_transform.Position.ToPoint()))
            {
                _direction = (_frame.ScreenSize / 2 - _transform.Position).ToAngle() + 
                             Random.Shared.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
            }
        }

        _rigidbody.AddForce(MathUtil.FromPolar(_direction, GameSettings.Enemy.WandererVelocity));
        _transform.Orientation -= GameSettings.Enemy.WandererOrientationDecay;
    }
}
