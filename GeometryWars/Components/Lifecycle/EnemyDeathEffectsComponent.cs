using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Lifecycle;

// Plays enemy death VFX/SFX when the owning enemy is destroyed.
public sealed class EnemyDeathEffectsComponent : Component
{
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly Action _playExplosionSound;
    private readonly Texture2D _lineParticle;

    public EnemyDeathEffectsComponent(IParticleSystem<ParticleState> particles, Action playExplosionSound, Texture2D lineParticle)
    {
        _particles = particles;
        _playExplosionSound = playExplosionSound;
        _lineParticle = lineParticle;
    }

    public void Play(Enemy enemy)
    {
        var pos = enemy.Transform.Position;

        float hue1 = Random.Shared.NextFloat(0, 6);
        float hue2 = (hue1 + Random.Shared.NextFloat(0, 2)) % 6f;
        Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
        Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);
        for (int i = 0; i < GameSettings.Visuals.EnemyDeathParticles; i++)
        {
            float speed = GameSettings.Visuals.DeathParticleSpeed * (1f - 1 / Random.Shared.NextFloat(1f, 10f));
            var state = new ParticleState
            {
                Velocity = Random.Shared.NextVector2(speed, speed),
                Type = ParticleType.Enemy,
                LengthMultiplier = 1f
            };
            Color color = Color.Lerp(color1, color2, Random.Shared.NextFloat(0, 1));
            _particles.CreateParticle(_lineParticle, pos, color, GameSettings.Visuals.DeathParticleLife, GameSettings.Visuals.DeathParticleSize, state);
        }

        _playExplosionSound();
    }
}
