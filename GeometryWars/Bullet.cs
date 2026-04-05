using System;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public class Bullet : Entity
{
    private static readonly Random rand = new();

    public Bullet(Vector2 position, Vector2 velocity)
    {
        image = Art.Bullet;
        Position = position;
        Velocity = velocity;
        Orientation = Velocity.ToAngle();
        Radius = 8;
    }
    public override void Update()
    {
        if (Velocity.LengthSquared() > 0)
            Orientation = Velocity.ToAngle();
        Position += Velocity;
        // delete bullets that go off-screen 
        if (!Game1.Viewport.Bounds.Contains(Position.ToPoint()))
        {
            IsExpired = true;
            for (int i = 0; i < 30; i++)
                Game1.ParticleManager.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, 1,
                    new ParticleState() { Velocity = rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
        }
        Game1.Grid.ApplyExplosiveForce(0.5f * Velocity.Length(), Position, 80);
    }
}