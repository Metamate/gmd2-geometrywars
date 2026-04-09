using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Components.Lifecycle;

// Expires an entity when it leaves the viewport and spawns an exit burst.
public sealed class ExpireOutsideViewportWithParticlesComponent : Component
{
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly FrameInfo _frame;
    private readonly Texture2D _lineParticle;
    private TransformComponent _transform;

    public ExpireOutsideViewportWithParticlesComponent(IParticleSystem<ParticleState> particles, FrameInfo frame, Texture2D lineParticle)
    {
        _particles = particles;
        _frame = frame;
        _lineParticle = lineParticle;
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
        for (int i = 0; i < GameSettings.Visuals.BulletDeathParticles; i++)
        {
            _particles.CreateParticle(_lineParticle, _transform.Position, Color.LightBlue, 50, 1,
                new ParticleState { Velocity = Random.Shared.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
        }
    }
}
