using System;
using System.Collections.Generic;
using GMDCore.ECS;
using GMDCore.Particles;
using GeometryWars.Services;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

public record struct ParticleState
{
    public ParticleState(
        Vector2 velocity,
        float lengthMultiplier = 1f,
        float stretchFromSpeed = 0.2f,
        bool affectedByGravity = true,
        float damping = GameSettings.Physics.ParticleDefaultDamping,
        bool addPositionDampingJitter = true)
    {
        Velocity = velocity;
        LengthMultiplier = lengthMultiplier;
        StretchFromSpeed = stretchFromSpeed;
        AffectedByGravity = affectedByGravity;
        Damping = damping;
        AddPositionDampingJitter = addPositionDampingJitter;
    }

    public Vector2 Velocity { get; init; }
    public float LengthMultiplier { get; init; }
    public float StretchFromSpeed { get; init; }
    public bool AffectedByGravity { get; init; }
    public float Damping { get; init; }
    public bool AddPositionDampingJitter { get; init; }

    // Smooth trail that stays coherent while still being affected by gravity forces.
    public static ParticleState StableTrail(Vector2 velocity, float lengthMultiplier = 1f)
        => new(velocity, lengthMultiplier, stretchFromSpeed: 0.2f, affectedByGravity: true, damping: GameSettings.Physics.ParticleEnemyDamping, addPositionDampingJitter: false);

    // Slightly noisier trail that keeps more visual flicker as it slows down.
    public static ParticleState JitteredTrail(Vector2 velocity, float lengthMultiplier = 1f)
        => new(velocity, lengthMultiplier, stretchFromSpeed: 0.1f);

    // General-purpose burst particle that drifts, damps, and responds to gravity.
    public static ParticleState DriftingBurst(Vector2 velocity, float lengthMultiplier = 1f)
        => new(velocity, lengthMultiplier);

    // Burst particle that ignores gravity forces and simply flies outward.
    public static ParticleState UnboundBurst(Vector2 velocity, float lengthMultiplier = 1f)
        => new(velocity, lengthMultiplier, affectedByGravity: false);

    public static void UpdateParticle(ParticleInstance<ParticleState> particle, IReadOnlyList<Entity> blackHoles, FrameInfo frame)
    {
        var vel = particle.State.Velocity;
        particle.Position += vel;
        particle.Orientation = vel.ToAngle();

        float speed = vel.Length();
        float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
        alpha *= alpha;

        var tint = particle.Tint;
        tint.A = (byte)(255 * alpha);
        particle.Tint = tint;

        var scale = particle.Scale;
        scale.X = particle.State.LengthMultiplier
            * Math.Min(Math.Min(1f, particle.State.StretchFromSpeed * speed + 0.1f), alpha);
        particle.Scale = scale;

        var pos = particle.Position;
        int width = (int)frame.ScreenSize.X;
        int height = (int)frame.ScreenSize.Y;

        if (pos.X < 0) vel.X = Math.Abs(vel.X);
        else if (pos.X > width) vel.X = -Math.Abs(vel.X);
        if (pos.Y < 0) vel.Y = Math.Abs(vel.Y);
        else if (pos.Y > height) vel.Y = -Math.Abs(vel.Y);

        if (particle.State.AffectedByGravity)
        {
            for (int i = 0; i < blackHoles.Count; i++)
            {
                var blackHole = blackHoles[i];
                var dPos = blackHole.Transform.Position - pos;
                float distance = dPos.Length();
                if (distance < 0.001f)
                    continue;

                var n = dPos / distance;
                vel += GameSettings.Physics.ParticleGravityForce * n / (distance * distance + GameSettings.Physics.ParticleGravityForce);
                if (distance < GameSettings.Physics.ParticleOrbitalRange)
                    vel += GameSettings.Physics.ParticleOrbitalForce * new Vector2(n.Y, -n.X) / (distance + GameSettings.Physics.ParticleOrbitalDamping);
            }
        }

        if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.00000000001f)
            vel = Vector2.Zero;
        else
            vel *= particle.State.Damping + (particle.State.AddPositionDampingJitter ? Math.Abs(pos.X) % 0.04f : 0f);

        particle.State = particle.State with { Velocity = vel };
    }
}

