using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class BulletMovementBehaviour : IComponent
{
    private TransformComponent _transform;
    private MovementComponent _movement;

    public void OnAdded(Entity owner)
    {
        _transform = owner.GetComponent<TransformComponent>();
        _movement = owner.GetComponent<MovementComponent>();
    }

    public void Update(Entity owner)
    {
        if (_transform == null || _movement == null) return;

        if (_movement.Velocity.LengthSquared() > 0.01f)
            _transform.Orientation = _movement.Velocity.ToAngle();

        if (!FrameContext.Viewport.Bounds.Contains(_transform.Position.ToPoint()))
        {
            owner.IsExpired = true;
            for (int i = 0; i < GameSettings.Visuals.BulletDeathParticles; i++)
            {
                GameServices.Particles.CreateParticle(Art.LineParticle, _transform.Position, Color.LightBlue, 50, 1,
                    new ParticleState { Velocity = Random.Shared.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
            }
        }

        GameServices.Grid.ApplyExplosiveForce(GameSettings.Physics.BulletGridForce * _movement.Velocity.Length(), _transform.Position, GameSettings.Physics.BulletGridRadius);
    }
}
