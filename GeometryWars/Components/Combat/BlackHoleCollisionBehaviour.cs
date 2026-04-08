using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Combat;

// Handles damage and effects when a black hole is hit.
public sealed class BlackHoleCollisionBehaviour : Component, ICollisionComponent
{
    private int _hitpoints;
    private TransformComponent _transform;

    public BlackHoleCollisionBehaviour(int hitpoints)
    {
        _hitpoints = hitpoints;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void Update(Entity owner) { }

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
