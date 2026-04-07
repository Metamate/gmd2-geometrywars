using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public class BlackHole : Entity
{
    private int hitpoints = GameSettings.BlackHoleHitpoints;
    private float sprayAngle = 0;

    public BlackHole(Vector2 position)
    {
        Image    = Art.BlackHole;
        Position = position;
        Collider = new CircleCollider(Image.Width / 2f);
    }

    // Collision response: only bullets damage the black hole.
    // Enemy and player collisions are handled entirely on their own side.
    public override void OnCollision(Entity other)
    {
        if (other is Bullet)
            WasShot();
    }

    public void WasShot()
    {
        if (--hitpoints <= 0)
            IsExpired = true;

        float hue = (float)(3 * GameServices.TotalSeconds % 6);
        Color color = ColorUtil.HSVToColor(hue, 0.25f, 1);
        float startOffset = Random.Shared.NextFloat(0, MathHelper.TwoPi / GameSettings.BlackHoleHitParticles);
        for (int i = 0; i < GameSettings.BlackHoleHitParticles; i++)
        {
            float speed = Random.Shared.NextFloat(GameSettings.BlackHoleHitParticleMinSpeed, GameSettings.BlackHoleHitParticleMaxSpeed);
            Vector2 sprayVel = MathUtil.FromPolar(MathHelper.TwoPi * i / GameSettings.BlackHoleHitParticles + startOffset, speed);
            var state = new ParticleState
            {
                Velocity = sprayVel,
                LengthMultiplier = 1,
                Type = ParticleType.IgnoreGravity
            };
            GameServices.Particles.CreateParticle(Art.LineParticle, Position + 2f * sprayVel, color,
                GameSettings.BlackHoleHitParticleLife, GameSettings.BlackHoleHitParticleSize, state);
        }
    }

    public void Kill() => IsExpired = true;

    public override void Draw(SpriteBatch spriteBatch)
    {
        float scale = 1 + 0.1f * (float)Math.Sin(10 * GameServices.TotalSeconds);
        spriteBatch.Draw(Image, Position, null, Tint, Orientation, Size / 2f, scale, 0, 0);
        base.Draw(spriteBatch);
    }

    protected override void OnUpdate()
    {
        // Apply gravity and repulsion to nearby entities
        foreach (var entity in EntityManager.GetNearbyEntities(Position, GameSettings.BlackHoleGravityRange))
        {
            if (entity is Enemy enemy && !enemy.IsActive)
                continue;

            if (entity is Bullet)
                entity.Velocity += (entity.Position - Position).ScaleTo(0.3f);
            else
            {
                var dPos = Position - entity.Position;
                entity.Velocity += dPos.ScaleTo(MathHelper.Lerp(GameSettings.BlackHoleGravityForce, 0, dPos.Length() / GameSettings.BlackHoleGravityRange));
            }
        }

        // Orbital particle spray toggles every quarter second
        if ((GameServices.Time.TotalGameTime.Milliseconds / 250) % 2 == 0)
        {
            Vector2 sprayVel = MathUtil.FromPolar(sprayAngle, Random.Shared.NextFloat(12, 15));
            Color color = ColorUtil.HSVToColor(5, 0.5f, 0.8f);
            Vector2 pos = Position + 2f * new Vector2(sprayVel.Y, -sprayVel.X) + Random.Shared.NextVector2(4, 8);
            GameServices.Particles.CreateParticle(Art.LineParticle, pos, color, 190, 1.5f,
                new ParticleState { Velocity = sprayVel, LengthMultiplier = 1, Type = ParticleType.Enemy });
        }

        sprayAngle -= MathHelper.TwoPi / 50f;
        GameServices.Grid.ApplyImplosiveForce((float)Math.Sin(sprayAngle / 2) * 10 + 20, Position, GameSettings.BlackHoleGridRange);
    }
}
