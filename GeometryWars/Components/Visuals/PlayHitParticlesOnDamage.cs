using System;
using GeometryWars.Components.Combat;
using GMDCore.ECS.Components;
using GeometryWars.Definitions;
using GMDCore.ECS;
using GMDCore.Particles;
using GeometryWars.Services;
using GeometryWars.Systems;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Visuals;

// Plays a particle burst whenever the owner's health component reports damage.
public sealed class PlayHitParticlesOnDamage : Component
{
    private readonly BlackHoleDefinition _definition;
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly FrameInfo _frame;
    private readonly Texture2D _lineParticle;
    private Transform _transform;
    private Health _health;

    public PlayHitParticlesOnDamage(IParticleSystem<ParticleState> particles, FrameInfo frame, Texture2D lineParticle, BlackHoleDefinition definition)
    {
        _particles = particles;
        _frame = frame;
        _lineParticle = lineParticle;
        _definition = definition;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _health = owner.RequireComponent<Health>();
        _health.Damaged += PlayHitParticles;
    }

    public override void OnRemoved(Entity owner)
    {
        if (_health != null)
            _health.Damaged -= PlayHitParticles;
    }

    private void PlayHitParticles()
    {
        float hue = (float)(3 * _frame.TotalSeconds % 6);
        Color color = ColorUtil.HSVToColor(hue, 0.25f, 1);
        float startOffset = Random.Shared.NextFloat(0, MathHelper.TwoPi / _definition.HitParticleCount);

        for (int i = 0; i < _definition.HitParticleCount; i++)
        {
            float speed = Random.Shared.NextFloat(_definition.HitParticleMinSpeed, _definition.HitParticleMaxSpeed);
            Vector2 sprayVel = MathUtil.FromPolar(MathHelper.TwoPi * i / _definition.HitParticleCount + startOffset, speed);
            var state = ParticleState.UnboundBurst(sprayVel);
            _particles.CreateParticle(_lineParticle, _transform.Position + 2f * sprayVel, color,
                GameSettings.Visuals.DeathParticleLife, GameSettings.Visuals.DeathParticleSize, state);
        }
    }
}

