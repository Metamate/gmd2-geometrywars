using System;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public enum ParticleType { None, Enemy, Bullet, IgnoreGravity }
public struct ParticleState(Vector2 velocity, ParticleType type, float lengthMultiplier = 1f)
{
    public Vector2 Velocity = velocity;
    public ParticleType Type = type;
    public float LengthMultiplier = lengthMultiplier;

    public static void UpdateParticle(ParticleManager<ParticleState>.Particle particle)
    {
        var vel = particle.State.Velocity;
        particle.Position += vel;
        particle.Orientation = vel.ToAngle();

        float speed = vel.Length();
        float alpha = Math.Min(1, Math.Min(particle.PercentLife * 2, speed * 1f));
        alpha *= alpha;
        particle.Tint.A = (byte)(255 * alpha);

        // the length of bullet particles will be less dependent on their speed than other particles
        if (particle.State.Type == ParticleType.Bullet)
            particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.1f * speed + 0.1f), alpha);
        else
            particle.Scale.X = particle.State.LengthMultiplier * Math.Min(Math.Min(1f, 0.2f * speed + 0.1f), alpha);

        var pos = particle.Position;
        int width = (int)Game1.ScreenSize.X;
        int height = (int)Game1.ScreenSize.Y;
        // collide with the edges of the screen 
        if (pos.X < 0) vel.X = Math.Abs(vel.X);
        else if (pos.X > width)
            vel.X = -Math.Abs(vel.X);
        if (pos.Y < 0) vel.Y = Math.Abs(vel.Y);
        else if (pos.Y > height)
            vel.Y = -Math.Abs(vel.Y);

        if (particle.State.Type != ParticleType.IgnoreGravity)
        {
            foreach (var blackHole in EntityManager.BlackHoles)
            {
                var dPos = blackHole.Position - pos;
                float distance = dPos.Length();
                var n = dPos / distance;
                vel += 10000 * n / (distance * distance + 10000);
                // add tangential acceleration for nearby particles 
                if (distance < 400)
                    vel += 45 * new Vector2(n.Y, -n.X) / (distance + 100);
            }
        }

        if (Math.Abs(vel.X) + Math.Abs(vel.Y) < 0.00000000001f) // denormalized floats cause significant performance issues
            vel = Vector2.Zero;
        else if (particle.State.Type == ParticleType.Enemy)
            vel *= 0.94f;
        else
            vel *= 0.96f + Math.Abs(pos.X) % 0.04f; // rand.Next() isn't thread-safe, so use the position for pseudo-randomness

        particle.State.Velocity = vel;
    }
}