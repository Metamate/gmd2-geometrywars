using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class ExhaustFireComponent : IComponent
{
    private MovementComponent _movement;
    private TransformComponent _transform;

    public void OnAdded(Entity owner)
    {
        _movement = owner.Movement;
        _transform = owner.Transform;
    }

    public void Update(Entity owner)
    {
        if (owner is not PlayerShip player || player.IsDead || _movement == null || _transform == null)
            return;

        if (_movement.Velocity.LengthSquared() <= 0.1f)
            return;

        Quaternion rot = Quaternion.CreateFromYawPitchRoll(0f, 0f, _transform.Orientation);
        double t = FrameContext.TotalSeconds;

        Vector2 baseVel = _movement.Velocity.ScaleTo(-3);
        Vector2 perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));
        Color sideColor = new Color(200, 38, 9);
        Color midColor = new Color(255, 187, 30);
        Vector2 pos = _transform.Position + Vector2.Transform(new Vector2(-25, 0), rot);
        const float alpha = 0.7f;

        Vector2 velMid = baseVel + Random.Shared.NextVector2(0, 1);
        GameServices.Particles.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(velMid, ParticleType.Enemy));
        GameServices.Particles.CreateParticle(Art.Glow, pos, midColor * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(velMid, ParticleType.Enemy));

        Vector2 vel1 = baseVel + perpVel + Random.Shared.NextVector2(0, 0.3f);
        Vector2 vel2 = baseVel - perpVel + Random.Shared.NextVector2(0, 0.3f);
        GameServices.Particles.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel1, ParticleType.Enemy));
        GameServices.Particles.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel2, ParticleType.Enemy));
        GameServices.Particles.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel1, ParticleType.Enemy));
        GameServices.Particles.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel2, ParticleType.Enemy));
    }
}
