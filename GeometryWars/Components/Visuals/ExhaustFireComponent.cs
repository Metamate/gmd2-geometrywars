using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Visuals;

// Spawns fire particles behind a moving entity.
public sealed class ExhaustFireComponent : Component
{
    private readonly IParticleSystem<ParticleState> _particles;
    private readonly GameRuntime _runtime;
    private PlayerShip _player;
    private RigidbodyComponent _rigidbody;
    private TransformComponent _transform;

    public ExhaustFireComponent(IParticleSystem<ParticleState> particles, GameRuntime runtime)
    {
        _particles = particles;
        _runtime = runtime;
    }

    public override void OnStart(Entity owner)
    {
        _player    = owner as PlayerShip;
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
        _transform = owner.Transform;
    }

    public override void PostUpdate(Entity owner)
    {
        if (_player == null || _player.IsDead)
            return;

        if (_rigidbody.Velocity.LengthSquared() <= 0.1f)
            return;

        Quaternion rot = Quaternion.CreateFromYawPitchRoll(0f, 0f, _transform.Orientation);
        double t = _runtime.Frame.TotalSeconds;

        Vector2 baseVel = _rigidbody.Velocity.ScaleTo(-3);
        Vector2 perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));
        Color sideColor = new Color(200, 38, 9);
        Color midColor  = new Color(255, 187, 30);
        Vector2 pos = _transform.Position + Vector2.Transform(new Vector2(-25, 0), rot);
        const float alpha = 0.7f;

        Vector2 velMid = baseVel + Random.Shared.NextVector2(0, 1);
        _particles.CreateParticle(_runtime.Assets.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(velMid, ParticleType.Enemy));
        _particles.CreateParticle(_runtime.Assets.Glow, pos, midColor * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(velMid, ParticleType.Enemy));

        Vector2 vel1 = baseVel + perpVel + Random.Shared.NextVector2(0, 0.3f);
        Vector2 vel2 = baseVel - perpVel + Random.Shared.NextVector2(0, 0.3f);
        _particles.CreateParticle(_runtime.Assets.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel1, ParticleType.Enemy));
        _particles.CreateParticle(_runtime.Assets.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel2, ParticleType.Enemy));
        _particles.CreateParticle(_runtime.Assets.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel1, ParticleType.Enemy));
        _particles.CreateParticle(_runtime.Assets.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel2, ParticleType.Enemy));
    }
}
