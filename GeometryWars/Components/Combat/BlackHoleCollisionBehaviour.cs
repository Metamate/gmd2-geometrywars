using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Combat;

// Handles damage and effects when a black hole is hit.
public sealed class BlackHoleCollisionBehaviour : Component
{
    private int _hitpoints;
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly GameRuntime _runtime;
    private TransformComponent _transform;

    public BlackHoleCollisionBehaviour(int hitpoints, IParticleSystem<ParticleState> particles, GameRuntime runtime)
    {
        _hitpoints = hitpoints;
        _particles = particles;
        _runtime = runtime;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void OnCollision(Entity owner, Entity other)
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

        float hue = (float)(3 * _runtime.Frame.TotalSeconds % 6);
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
            _particles.CreateParticle(_runtime.Assets.LineParticle, _transform.Position + 2f * sprayVel, color,
                GameSettings.Visuals.DeathParticleLife, GameSettings.Visuals.DeathParticleSize, state);
        }
    }
}
