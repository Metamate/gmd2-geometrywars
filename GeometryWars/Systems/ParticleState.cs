using System;
using System.Collections.Generic;
using GeometryWars.Entities;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

public enum ParticleType { None, Enemy, Bullet, IgnoreGravity }

public record struct ParticleState(Vector2 Velocity, ParticleType Type, float LengthMultiplier = 1f)
{
    public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle, IReadOnlyList<BlackHole> blackHoles, FrameInfo frame)
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
        if (particle.State.Type == ParticleType.Bullet)
            scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.1f * speed + 0.1f), alpha);
        else
            scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);
        particle.Scale = scale;

        var pos = particle.Position;
        int width = (int)frame.ScreenSize.X;
        int height = (int)frame.ScreenSize.Y;

        if (pos.X < 0) vel.X = Math.Abs(vel.X);
        else if (pos.X > width) vel.X = -Math.Abs(vel.X);
        if (pos.Y < 0) vel.Y = Math.Abs(vel.Y);
        else if (pos.Y > height) vel.Y = -Math.Abs(vel.Y);

        if (particle.State.Type != ParticleType.IgnoreGravity)
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
        else if (particle.State.Type == ParticleType.Enemy)
            vel *= GameSettings.Physics.ParticleEnemyDamping;
        else
            vel *= GameSettings.Physics.ParticleDefaultDamping + Math.Abs(pos.X) % 0.04f;

        particle.State = particle.State with { Velocity = vel };
    }
}
