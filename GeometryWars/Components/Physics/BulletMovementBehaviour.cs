using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Physics;

// Handles bullet-specific movement, screen bounds, and grid effects.
public sealed class BulletMovementBehaviour : Component
{
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly IGridField _grid;
    private readonly FrameInfo _frame;
    private readonly Texture2D _lineParticle;
    private TransformComponent _transform;
    private RigidbodyComponent _rigidbody;

    public BulletMovementBehaviour(IParticleSystem<ParticleState> particles, IGridField grid, FrameInfo frame, Texture2D lineParticle)
    {
        _particles = particles;
        _grid = grid;
        _frame = frame;
        _lineParticle = lineParticle;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
    }

    public override void PostUpdate(Entity owner)
    {
        if (_rigidbody.Velocity.LengthSquared() > 0.01f)
            _transform.Orientation = _rigidbody.Velocity.ToAngle();

        if (!_frame.Viewport.Bounds.Contains(_transform.Position.ToPoint()))
        {
            owner.IsExpired = true;
            for (int i = 0; i < GameSettings.Visuals.BulletDeathParticles; i++)
            {
                _particles.CreateParticle(_lineParticle, _transform.Position, Color.LightBlue, 50, 1,
                    new ParticleState { Velocity = Random.Shared.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
            }
        }

        _grid.ApplyExplosiveForce(GameSettings.Physics.BulletGridForce * _rigidbody.Velocity.Length(), _transform.Position, GameSettings.Physics.BulletGridRadius);
    }
}
