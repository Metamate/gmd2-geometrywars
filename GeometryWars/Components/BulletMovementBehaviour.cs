using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

/// <summary>
/// Handles movement and spatial effects (particles/grid) for Bullets.
/// </summary>
public sealed class BulletMovementBehaviour : IComponent
{
    public void Update(Entity owner)
    {
        // 1. Movement
        if (owner.Velocity.LengthSquared() > 0)
            owner.Orientation = owner.Velocity.ToAngle();

        owner.Position += owner.Velocity;

        // 2. Out-of-bounds cleanup and effects
        if (!FrameContext.Viewport.Bounds.Contains(owner.Position.ToPoint()))
        {
            owner.IsExpired = true;
            for (int i = 0; i < GameSettings.Visuals.BulletDeathParticles; i++)
            {
                GameServices.Particles.CreateParticle(Art.LineParticle, owner.Position, Color.LightBlue, 50, 1,
                    new ParticleState { Velocity = Random.Shared.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
            }
        }

        // 3. Grid distortion
        GameServices.Grid.ApplyExplosiveForce(GameSettings.Physics.BulletGridForce * owner.Velocity.Length(), owner.Position, GameSettings.Physics.BulletGridRadius);
    }
}
