using System;
using System.Collections.Generic;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public class Enemy : Entity
{
    private static Random rand = new();
    private int timeUntilStart = 60;
    private List<IEnumerator<int>> behaviours = [];

    public Enemy(Texture2D image, Vector2 position)
    {
        this.image = image;
        Position = position;
        color = Color.Transparent;

        // Movement: decelerate each frame and stay on screen.
        AddComponent(new VelocityMover(damping: 0.8f, clampToScreen: true));

        // Collision response: enemies push each other apart; bullets kill them.
        // Player collision is handled entirely on the player's side.
        AddComponent(new CircleCollider(image.Width / 2f, other =>
        {
            if (other is Bullet)
                WasShot();
            else if (other is Enemy e)
                HandleCollision(e);
        }));
    }

    public bool IsActive => timeUntilStart <= 0;
    public int PointValue { get; private set; }

    public static Enemy CreateSeeker(Vector2 position)
    {
        var enemy = new Enemy(Art.Seeker, position);
        enemy.AddBehaviour(enemy.FollowPlayer());
        enemy.PointValue = 2;
        return enemy;
    }

    public static Enemy CreateWanderer(Vector2 position)
    {
        var enemy = new Enemy(Art.Wanderer, position);
        enemy.AddBehaviour(enemy.MoveRandomly());
        enemy.PointValue = 1;
        return enemy;
    }

    protected override void OnPreUpdate()
    {
        if (timeUntilStart <= 0)
            ApplyBehaviours();
        else
        {
            timeUntilStart--;
            color = Color.White * (1 - timeUntilStart / 60f);
        }
    }

    public void HandleCollision(Enemy other)
    {
        var d = Position - other.Position;
        Velocity += 10 * d / (d.LengthSquared() + 1);
    }

    public void WasShot()
    {
        IsExpired = true;

        float hue1 = rand.NextFloat(0, 6);
        float hue2 = (hue1 + rand.NextFloat(0, 2)) % 6f;
        Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
        Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);
        for (int i = 0; i < 120; i++)
        {
            float speed = 18f * (1f - 1 / rand.NextFloat(1f, 10f));
            var state = new ParticleState
            {
                Velocity = rand.NextVector2(speed, speed),
                Type = ParticleType.Enemy,
                LengthMultiplier = 1f
            };
            Color color = Color.Lerp(color1, color2, rand.NextFloat(0, 1));
            GameServices.Particles.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f, state);
        }

        PlayerStatus.AddPoints(PointValue);
        PlayerStatus.IncreaseMultiplier();
        Sound.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);
    }

    IEnumerable<int> FollowPlayer(float acceleration = 1f)
    {
        while (true)
        {
            Velocity += (PlayerShip.Instance.Position - Position).ScaleTo(acceleration);
            if (Velocity != Vector2.Zero)
                Orientation = Velocity.ToAngle();
            yield return 0;
        }
    }

    IEnumerable<int> MoveRandomly()
    {
        float direction = rand.NextFloat(0, MathHelper.TwoPi);
        while (true)
        {
            direction += rand.NextFloat(-0.1f, 0.1f);
            direction = MathHelper.WrapAngle(direction);
            for (int i = 0; i < 6; i++)
            {
                Velocity += MathUtil.FromPolar(direction, 0.4f);
                Orientation -= 0.05f;
                var bounds = GameServices.Viewport.Bounds;
                bounds.Inflate(-image.Width, -image.Height);
                if (!bounds.Contains(Position.ToPoint()))
                    direction = (GameServices.ScreenSize / 2 - Position).ToAngle() +
                                rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                yield return 0;
            }
        }
    }

    private void AddBehaviour(IEnumerable<int> behaviour) => behaviours.Add(behaviour.GetEnumerator());

    private void ApplyBehaviours()
    {
        for (int i = 0; i < behaviours.Count; i++)
            if (!behaviours[i].MoveNext())
                behaviours.RemoveAt(i--);
    }
}
