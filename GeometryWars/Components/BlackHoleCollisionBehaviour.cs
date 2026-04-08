using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class BlackHoleCollisionBehaviour : ICollisionComponent
{
    private int _hitpoints;
    private TransformComponent _transform;

    public BlackHoleCollisionBehaviour(int hitpoints)
    {
        _hitpoints = hitpoints;
    }

    public void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    public void Update(Entity owner) { }

    public void OnCollision(Entity owner, Entity other)
    {
        if (other is Bullet)
        {
            WasShot(owner);
        }
    }

    private void WasShot(Entity owner)
    {
        if (--_hitpoints <= 0)
            owner.IsExpired = true;

        float hue = (float)(3 * FrameContext.TotalSeconds % 6);
        Color color = ColorUtil.HSVToColor(hue, 0.25f, 1);
        float startOffset = Random.Shared.NextFloat(0, MathHelper.TwoPi / GameSettings.Visuals.BlackHoleHitParticles);
        
        for (int i = 0; i < GameSettings.Visuals.BlackHoleHitParticles; i++)
        {
            float speed = Random.Shared.NextFloat(GameSettings.Visuals.BlackHoleHitParticleMinSpeed, GameSettings.Visuals.BlackHoleHitParticleMaxSpeed);
            Vector2 sprayVel = MathUtil.FromPolar(MathHelper.TwoPi * i / GameSettings.Visuals.BlackHoleHitParticles + startOffset, speed);
            var state = new ParticleState
            {
                Velocity = sprayVel,
                LengthMultiplier = 1,
                Type = ParticleType.IgnoreGravity
            };
            GameServices.Particles.CreateParticle(Art.LineParticle, _transform.Position + 2f * sprayVel, color,
                GameSettings.Visuals.DeathParticleLife, GameSettings.Visuals.DeathParticleSize, state);
        }
    }
}
