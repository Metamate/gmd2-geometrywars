using System;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public class Bullet : Entity
{
    private static readonly Random rand = new();

    // Parameterless constructor for ObjectPool<Bullet>
    public Bullet()
    {
        image = Art.Bullet;
        Radius = 8;
    }

    // Called by EntityManager.GetBullet() to reinitialise a pooled instance
    public void Reset(Vector2 position, Vector2 velocity)
    {
        Position = position;
        Velocity = velocity;
        Orientation = velocity.ToAngle();
        IsExpired = false;
    }

    public override void Update(GameContext ctx)
    {
        if (Velocity.LengthSquared() > 0)
            Orientation = Velocity.ToAngle();

        Position += Velocity;

        if (!ctx.Viewport.Bounds.Contains(Position.ToPoint()))
        {
            IsExpired = true;
            for (int i = 0; i < 30; i++)
                ctx.Particles.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, 1,
                    new ParticleState() { Velocity = rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
        }

        ctx.Grid.ApplyExplosiveForce(0.5f * Velocity.Length(), Position, 80);
    }
}
