using System;
using GMDCore.ECS.Components;
using GMDCore.Physics;
using GeometryWars.Definitions;
using GMDCore.ECS;
using GeometryWars.Services;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.AI;

// Steers the owner in semi-random directions.
public sealed class Wander : Component
{
    private readonly WanderEnemyDefinition _definition;
    private readonly FrameInfo _frame;
    private readonly Vector2 _spriteSize;
    private float _direction;
    private int _stepCounter;
    
    private Transform _transform;
    private Rigidbody _rigidbody;

    public Wander(FrameInfo frame, Vector2 spriteSize, WanderEnemyDefinition definition)
    {
        _frame = frame;
        _spriteSize = spriteSize;
        _definition = definition;
        _direction = Random.Shared.NextFloat(0, MathHelper.TwoPi);
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _rigidbody = owner.RequireComponent<Rigidbody>();
    }

    public override void Update(Entity owner)
    {
        if (_stepCounter-- <= 0)
        {
            _stepCounter = _definition.StepsPerTick;

            _direction += Random.Shared.NextFloat(-_definition.TurnRate, _definition.TurnRate);
            _direction = MathHelper.WrapAngle(_direction);

            var bounds = _frame.Viewport.Bounds;
            bounds.Inflate(-(int)_spriteSize.X, -(int)_spriteSize.Y);
            
            if (!bounds.Contains(_transform.Position.ToPoint()))
            {
                _direction = (_frame.ScreenSize / 2 - _transform.Position).ToAngle() + 
                             Random.Shared.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
            }
        }

        _rigidbody.AddForce(MathUtil.FromPolar(_direction, _definition.Velocity));
        _transform.Orientation -= _definition.OrientationDecay;
    }
}

