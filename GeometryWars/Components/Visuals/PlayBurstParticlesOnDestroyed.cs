using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Entities;
using GeometryWars.Systems;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Visuals;

// Plays a colorful burst of particles when the owner is destroyed.
public sealed class PlayBurstParticlesOnDestroyed : Component
{
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly Texture2D _lineParticle;
    private readonly int _particleCount;
    private readonly ParticleType _particleType;
    private Destroyable _destroyable;

    public PlayBurstParticlesOnDestroyed(IParticleSystem<ParticleState> particles, Texture2D lineParticle, int particleCount, ParticleType particleType)
    {
        _particles = particles;
        _lineParticle = lineParticle;
        _particleCount = particleCount;
        _particleType = particleType;
    }

    public override void OnStart(Entity owner)
    {
        if (_destroyable != null)
            _destroyable.Destroyed -= OnDestroyed;

        _destroyable = owner.RequireComponent<Destroyable>();
        _destroyable.Destroyed += OnDestroyed;
    }

    private void OnDestroyed(Entity owner)
    {
        var pos = owner.Transform.Position;

        float hue1 = Random.Shared.NextFloat(0, 6);
        float hue2 = (hue1 + Random.Shared.NextFloat(0, 2)) % 6f;
        Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
        Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);
        for (int i = 0; i < _particleCount; i++)
        {
            float speed = GameSettings.Visuals.DeathParticleSpeed * (1f - 1 / Random.Shared.NextFloat(1f, 10f));
            var state = new ParticleState
            {
                Velocity = Random.Shared.NextVector2(speed, speed),
                Type = _particleType,
                LengthMultiplier = 1f
            };
            Color color = Color.Lerp(color1, color2, Random.Shared.NextFloat(0, 1));
            _particles.CreateParticle(_lineParticle, pos, color, GameSettings.Visuals.DeathParticleLife, GameSettings.Visuals.DeathParticleSize, state);
        }
    }
}
