using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Visuals;

// Plays a particle burst whenever the owner is hit by a bullet.
public sealed class PlayHitParticlesOnBulletCollisionComponent : Component
{
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly FrameInfo _frame;
    private readonly Texture2D _lineParticle;
    private TransformComponent _transform;

    public PlayHitParticlesOnBulletCollisionComponent(IParticleSystem<ParticleState> particles, FrameInfo frame, Texture2D lineParticle)
    {
        _particles = particles;
        _frame = frame;
        _lineParticle = lineParticle;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void OnCollision(Entity owner, Entity other)
    {
        if (other is not Bullet)
            return;

        float hue = (float)(3 * _frame.TotalSeconds % 6);
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
            _particles.CreateParticle(_lineParticle, _transform.Position + 2f * sprayVel, color,
                GameSettings.Visuals.DeathParticleLife, GameSettings.Visuals.DeathParticleSize, state);
        }
    }
}
