using System;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public class BlackHole : Entity
{
    private static Random rand = new();
    private int hitpoints = 10;
    private float sprayAngle = 0;

    public BlackHole(Vector2 position)
    {
        image = Art.BlackHole;
        Position = position;

        // Bullets weaken the black hole; everything else is handled by their own colliders.
        AddComponent(new CircleCollider(image.Width / 2f, other =>
        {
            if (other is Bullet)
                WasShot();
        }));
    }

    public void WasShot()
    {
        if (--hitpoints <= 0)
            IsExpired = true;

        float hue = (float)(3 * GameServices.TotalSeconds % 6);
        Color color = ColorUtil.HSVToColor(hue, 0.25f, 1);
        const int numParticles = 150;
        float startOffset = rand.NextFloat(0, MathHelper.TwoPi / numParticles);
        for (int i = 0; i < numParticles; i++)
        {
            Vector2 sprayVel = MathUtil.FromPolar(MathHelper.TwoPi * i / numParticles + startOffset, rand.NextFloat(8, 16));
            var state = new ParticleState
            {
                Velocity = sprayVel,
                LengthMultiplier = 1,
                Type = ParticleType.IgnoreGravity
            };
            GameServices.Particles.CreateParticle(Art.LineParticle, Position + 2f * sprayVel, color, 90, 1.5f, state);
        }
    }

    public void Kill()
    {
        hitpoints = 0;
        WasShot();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float scale = 1 + 0.1f * (float)Math.Sin(10 * GameServices.TotalSeconds);
        spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, scale, 0, 0);
        base.Draw(spriteBatch);
    }

    protected override void OnPreUpdate()
    {
        // Apply gravity and repulsion to nearby entities
        foreach (var entity in EntityManager.GetNearbyEntities(Position, 250))
        {
            if (entity is Enemy enemy && !enemy.IsActive)
                continue;

            if (entity is Bullet)
                entity.Velocity += (entity.Position - Position).ScaleTo(0.3f);
            else
            {
                var dPos = Position - entity.Position;
                entity.Velocity += dPos.ScaleTo(MathHelper.Lerp(2, 0, dPos.Length() / 250f));
            }
        }

        // Orbital particle spray toggles every quarter second
        if ((GameServices.Time.TotalGameTime.Milliseconds / 250) % 2 == 0)
        {
            Vector2 sprayVel = MathUtil.FromPolar(sprayAngle, rand.NextFloat(12, 15));
            Color color = ColorUtil.HSVToColor(5, 0.5f, 0.8f);
            Vector2 pos = Position + 2f * new Vector2(sprayVel.Y, -sprayVel.X) + rand.NextVector2(4, 8);
            GameServices.Particles.CreateParticle(Art.LineParticle, pos, color, 190, 1.5f,
                new ParticleState { Velocity = sprayVel, LengthMultiplier = 1, Type = ParticleType.Enemy });
        }

        sprayAngle -= MathHelper.TwoPi / 50f;
        GameServices.Grid.ApplyImplosiveForce((float)Math.Sin(sprayAngle / 2) * 10 + 20, Position, 200);
    }
}
