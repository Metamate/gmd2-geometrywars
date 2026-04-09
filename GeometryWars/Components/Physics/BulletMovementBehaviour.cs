using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Physics;

// Handles bullet-specific movement, screen bounds, and grid effects.
public sealed class BulletMovementBehaviour : Component
{
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly IGridField _grid;
    private TransformComponent _transform;
    private RigidbodyComponent _rigidbody;

    public BulletMovementBehaviour(IParticleSystem<ParticleState> particles, IGridField grid)
    {
        _particles = particles;
        _grid = grid;
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

        if (!FrameContext.Viewport.Bounds.Contains(_transform.Position.ToPoint()))
        {
            owner.IsExpired = true;
            for (int i = 0; i < GameSettings.Visuals.BulletDeathParticles; i++)
            {
                _particles.CreateParticle(Art.LineParticle, _transform.Position, Color.LightBlue, 50, 1,
                    new ParticleState { Velocity = Random.Shared.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
            }
        }

        _grid.ApplyExplosiveForce(GameSettings.Physics.BulletGridForce * _rigidbody.Velocity.Length(), _transform.Position, GameSettings.Physics.BulletGridRadius);
    }
}
