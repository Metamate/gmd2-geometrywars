using System;
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
        Radius = image.Width / 2f;
    }

    public void WasShot()
    {
        hitpoints--;
        if (hitpoints <= 0)
            IsExpired = true;

        var ctx = GameContext.Current;
        float hue = (float)(3 * ctx.TotalSeconds % 6);
        Color color = ColorUtil.HSVToColor(hue, 0.25f, 1);
        const int numParticles = 150;
        float startOffset = rand.NextFloat(0, MathHelper.TwoPi / numParticles);
        for (int i = 0; i < numParticles; i++)
        {
            Vector2 sprayVel = MathUtil.FromPolar(MathHelper.TwoPi * i / numParticles + startOffset, rand.NextFloat(8, 16));
            Vector2 pos = Position + 2f * sprayVel;
            var state = new ParticleState()
            {
                Velocity = sprayVel,
                LengthMultiplier = 1,
                Type = ParticleType.IgnoreGravity
            };
            ctx.Particles.CreateParticle(Art.LineParticle, pos, color, 90, 1.5f, state);
        }
    }

    public void Kill()
    {
        hitpoints = 0;
        WasShot();
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        float scale = 1 + 0.1f * (float)Math.Sin(10 * GameContext.Current.TotalSeconds);
        spriteBatch.Draw(image, Position, null, color, Orientation, Size / 2f, scale, 0, 0);
        base.Draw(spriteBatch);
    }

    public override void Update(GameContext ctx)
    {
        var entities = EntityManager.GetNearbyEntities(Position, 250);
        foreach (var entity in entities)
        {
            if (entity is Enemy enemy && !enemy.IsActive)
                continue;

            if (entity is Bullet)
                entity.Velocity += (entity.Position - Position).ScaleTo(0.3f);
            else
            {
                var dPos = Position - entity.Position;
                var length = dPos.Length();
                entity.Velocity += dPos.ScaleTo(MathHelper.Lerp(2, 0, length / 250f));
            }
        }

        if ((ctx.GameTime.TotalGameTime.Milliseconds / 250) % 2 == 0)
        {
            Vector2 sprayVel = MathUtil.FromPolar(sprayAngle, rand.NextFloat(12, 15));
            Color color = ColorUtil.HSVToColor(5, 0.5f, 0.8f);
            Vector2 pos = Position + 2f * new Vector2(sprayVel.Y, -sprayVel.X) + rand.NextVector2(4, 8);
            var state = new ParticleState()
            {
                Velocity = sprayVel,
                LengthMultiplier = 1,
                Type = ParticleType.Enemy
            };
            ctx.Particles.CreateParticle(Art.LineParticle, pos, color, 190, 1.5f, state);
        }

        sprayAngle -= MathHelper.TwoPi / 50f;
        ctx.Grid.ApplyImplosiveForce((float)Math.Sin(sprayAngle / 2) * 10 + 20, Position, 200);
    }
}
