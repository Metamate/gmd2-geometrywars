using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class BulletMovementBehaviour : IComponent
{
    private MovementComponent _movement;

    public void OnAdded(Entity owner)
    {
        _movement = owner.Movement;
    }

    public void Update(Entity owner)
    {
        if (_movement == null) return;

        if (_movement.Velocity.LengthSquared() > 0.01f)
            _movement.Orientation = _movement.Velocity.ToAngle();

        if (!FrameContext.Viewport.Bounds.Contains(owner.Position.ToPoint()))
        {
            owner.IsExpired = true;
            for (int i = 0; i < GameSettings.Visuals.BulletDeathParticles; i++)
            {
                GameServices.Particles.CreateParticle(Art.LineParticle, owner.Position, Color.LightBlue, 50, 1,
                    new ParticleState { Velocity = Random.Shared.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
            }
        }

        GameServices.Grid.ApplyExplosiveForce(GameSettings.Physics.BulletGridForce * _movement.Velocity.Length(), owner.Position, GameSettings.Physics.BulletGridRadius);
    }
}
