using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.AI;

// Steers an entity in random directions.
public sealed class WanderBehaviour : Component
{
    private readonly GameRuntime _runtime;
    private float _direction;
    private int _stepCounter;
    
    private TransformComponent _transform;
    private RigidbodyComponent _rigidbody;

    public WanderBehaviour(GameRuntime runtime)
    {
        _runtime = runtime;
        _direction = Random.Shared.NextFloat(0, MathHelper.TwoPi);
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
    }

    public override void Update(Entity owner)
    {
        if (owner is not Enemy enemy)
            return;

        if (_stepCounter-- <= 0)
        {
            _stepCounter = GameSettings.Enemy.WandererStepsPerTick;

            _direction += Random.Shared.NextFloat(-GameSettings.Enemy.WandererTurnRate, GameSettings.Enemy.WandererTurnRate);
            _direction = MathHelper.WrapAngle(_direction);

            var bounds = _runtime.Frame.Viewport.Bounds;
            bounds.Inflate(-_runtime.Assets.Wanderer.Width, -_runtime.Assets.Wanderer.Height);
            
            if (!bounds.Contains(_transform.Position.ToPoint()))
            {
                _direction = (_runtime.Frame.ScreenSize / 2 - _transform.Position).ToAngle() + 
                             Random.Shared.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
            }
        }

        _rigidbody.AddForce(MathUtil.FromPolar(_direction, GameSettings.Enemy.WandererVelocity));
        _transform.Orientation -= GameSettings.Enemy.WandererOrientationDecay;
    }
}
