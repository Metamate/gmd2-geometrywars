using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public class Bullet : Entity
{
    public Bullet()
    {
        Image    = Art.Bullet;
        Collider = new CircleCollider(GameSettings.BulletColliderRadius);
    }

    public void Reset(Vector2 position, Vector2 velocity)
    {
        Position   = position;
        Velocity   = velocity;
        Orientation = velocity.ToAngle();
        IsExpired  = false;
    }

    // Collision response: expire on contact with an enemy or black hole.
    // The other entity handles its own side of the collision (e.g. Enemy.OnCollision
    // will call WasShot(); BlackHole.OnCollision will call WasShot()).
    public override void OnCollision(Entity other)
    {
        if (other is Enemy || other is BlackHole)
            IsExpired = true;
    }

    protected override void OnUpdate()
    {
        // Bullets manage their own movement (instead of using VelocityMover) because
        // they need to handle the screen-boundary side-effect of spawning particles.
        if (Velocity.LengthSquared() > 0)
            Orientation = Velocity.ToAngle();

        Position += Velocity;

        if (!GameServices.Viewport.Bounds.Contains(Position.ToPoint()))
        {
            IsExpired = true;
            for (int i = 0; i < GameSettings.BulletDeathParticles; i++)
                GameServices.Particles.CreateParticle(Art.LineParticle, Position, Color.LightBlue, 50, 1,
                    new ParticleState { Velocity = Random.Shared.NextVector2(0, 9), Type = ParticleType.Bullet, LengthMultiplier = 1 });
        }

        GameServices.Grid.ApplyExplosiveForce(GameSettings.BulletGridForce * Velocity.Length(), Position, GameSettings.BulletGridRadius);
    }
}
