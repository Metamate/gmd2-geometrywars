using System;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public class Bullet : Entity
{
    private static readonly Random rand = new();

    public Bullet()
    {
        image = Art.Bullet;

        // Expires when it leaves the screen; bullets don't need VelocityMover since
        // they move at constant velocity and have off-screen side-effects to handle.
        AddComponent(new CircleCollider(GameSettings.BulletColliderRadius, other =>
        {
            // Bullet response: expire on contact with enemy or black hole.
            // The other entity handles its own side of the collision.
            if (other is Enemy || other is BlackHole)
                IsExpired = true;
        }));
    }

    public void Reset(Vector2 position, Vector2 velocity)
    {
        Position = position;
        Velocity = velocity;
        Orientation = velocity.ToAngle();
        IsExpired = false;
    }

    protected override void OnPreUpdate()
    {
        if (Velocity.LengthSquared() > 0)
            Orientation = Velocity.ToAngle();

        Position += Velocity;

        if (!GameServices.Viewport.Bounds.Contains(Position.ToPoint()))
        {
            IsExpired = true;
            for (int i = 0; i < GameSettings.BulletDeathParticles; i++)
                GameServices.Particles.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, 1,
                    new ParticleState { Velocity = rand.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
        }

        GameServices.Grid.ApplyExplosiveForce(GameSettings.BulletGridForce * Velocity.Length(), Position, GameSettings.BulletGridRadius);
    }
}
