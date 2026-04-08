using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public enum ParticleType { None, Enemy, Bullet, IgnoreGravity }

public record struct ParticleState(Vector2 Velocity, ParticleType Type, float LengthMultiplier = 1f)
{
    public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle)
    {
        var vel = particle.State.Velocity;
        particle.Position    += vel;
        particle.Orientation  = vel.ToAngle();

        float speed = vel.Length();
        float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
        alpha *= alpha;
        particle.Tint.A = (byte)(255 * alpha);

        if (particle.State.Type == ParticleType.Bullet)
            particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.1f * speed + 0.1f), alpha);
        else
            particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);

        var pos    = particle.Position;
        int width  = (int)FrameContext.ScreenSize.X;
        int height = (int)FrameContext.ScreenSize.Y;

        if (pos.X < 0)       vel.X =  Math.Abs(vel.X);
        else if (pos.X > width)  vel.X = -Math.Abs(vel.X);
        if (pos.Y < 0)       vel.Y =  Math.Abs(vel.Y);
        else if (pos.Y > height) vel.Y = -Math.Abs(vel.Y);

        if (particle.State.Type != ParticleType.IgnoreGravity)
        {
            foreach (var blackHole in EntityManager.BlackHoles)
            {
                var dPos     = blackHole.Position - pos;
                float distance = dPos.Length();
                var n        = dPos / distance;
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

        particle.State.Velocity = vel;
    }

    public static void UpdateParticleOptimized(ref ParticleManagerOptimized<ParticleState>.Particle particle)
    {
        var vel = particle.State.Velocity;
        particle.Position    += vel;
        particle.Orientation  = vel.ToAngle();

        float speed = vel.Length();
        float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
        alpha *= alpha;
        particle.Tint.A = (byte)(255 * alpha);

        if (particle.State.Type == ParticleType.Bullet)
            particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.1f * speed + 0.1f), alpha);
        else
            particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);

        var pos    = particle.Position;
        int width  = (int)FrameContext.ScreenSize.X;
        int height = (int)FrameContext.ScreenSize.Y;

        if (pos.X < 0)           vel.X =  Math.Abs(vel.X);
        else if (pos.X > width)  vel.X = -Math.Abs(vel.X);
        if (pos.Y < 0)           vel.Y =  Math.Abs(vel.Y);
        else if (pos.Y > height) vel.Y = -Math.Abs(vel.Y);

        if (particle.State.Type != ParticleType.IgnoreGravity)
        {
            foreach (var blackHole in EntityManager.BlackHoles)
            {
                var dPos     = blackHole.Position - pos;
                float distance = dPos.Length();
                var n        = dPos / distance;
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

        particle.State.Velocity = vel;
    }
}
