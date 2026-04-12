using System;
using GMDCore.ECS.Components;
using GeometryWars.Definitions;
using GMDCore.ECS;
using GMDCore.Particles;
using GeometryWars.Services;
using GeometryWars.Systems;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Lifecycle;

// Expires an entity when it leaves the viewport and spawns an exit burst.
public sealed class ExpireOutsideViewportWithParticles : Component
{
    private readonly BulletDefinition _definition;
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly FrameInfo _frame;
    private readonly Texture2D _lineParticle;
    private Transform _transform;

    public ExpireOutsideViewportWithParticles(IParticleSystem<ParticleState> particles, FrameInfo frame, Texture2D lineParticle, BulletDefinition definition)
    {
        _particles = particles;
        _frame = frame;
        _lineParticle = lineParticle;
        _definition = definition;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void PostUpdate(Entity owner)
    {
        if (owner.IsExpired || _frame.Viewport.Bounds.Contains(_transform.Position.ToPoint()))
            return;

        owner.IsExpired = true;
        for (int i = 0; i < _definition.ExitParticleCount; i++)
        {
            _particles.CreateParticle(_lineParticle, _transform.Position, Color.LightBlue, _definition.ExitParticleLifetime, _definition.ExitParticleScale,
                ParticleState.JitteredTrail(Random.Shared.NextVector2(0, _definition.ExitParticleSpeed)));
        }
    }
}

