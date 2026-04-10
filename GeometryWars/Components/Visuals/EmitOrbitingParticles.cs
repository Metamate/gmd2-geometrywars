using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Systems;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Visuals;

// Emits a repeating ring of orbiting particles around the owner.
public sealed class EmitOrbitingParticles : Component
{
    private float _sprayAngle;
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly FrameInfo _frame;
    private readonly Texture2D _lineParticle;
    private Transform _transform;

    public EmitOrbitingParticles(IParticleSystem<ParticleState> particles, FrameInfo frame, Texture2D lineParticle)
    {
        _particles = particles;
        _frame = frame;
        _lineParticle = lineParticle;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void Update(Entity owner)
    {
        if (((int)_frame.Time.TotalGameTime.TotalMilliseconds / 250) % 2 == 0)
        {
            Vector2 sprayVel = MathUtil.FromPolar(_sprayAngle, Random.Shared.NextFloat(12, 15));
            Color color = ColorUtil.HSVToColor(5, 0.5f, 0.8f);
            Vector2 pos = _transform.Position + 2f * new Vector2(sprayVel.Y, -sprayVel.X) + Random.Shared.NextVector2(4, 8);
            _particles.CreateParticle(_lineParticle, pos, color, 190, 1.5f,
                ParticleState.StableTrail(sprayVel));
        }

        _sprayAngle -= MathHelper.TwoPi / 50f;
    }
}
