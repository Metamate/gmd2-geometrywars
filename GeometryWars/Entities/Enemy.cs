using System;
using System.Collections.Generic;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public class Enemy : Entity
{
    private static readonly Random rand = new();
    private int timeUntilStart = GameSettings.EnemySpawnDelay;
    private List<IEnumerator<int>> behaviours = [];

    public Enemy(Texture2D image, Vector2 position)
    {
        this.image = image;
        Position = position;
        color = Color.Transparent;

        // Movement: decelerate each frame and stay on screen.
        AddComponent(new VelocityMover(damping: GameSettings.EnemyDamping, clampToScreen: true));

        // Collision response: enemies push each other apart; bullets kill them.
        // Player collision is handled entirely on the player's side.
        AddComponent(new CircleCollider(image.Width / 2f, other =>
        {
            if (other is Bullet || (other is BlackHole && IsActive))
                WasShot();
            else if (other is Enemy e)
                HandleCollision(e);
        }));
    }

    public bool IsActive => timeUntilStart <= 0;
    public int PointValue { get; set; }

    public static Enemy CreateSeeker(Vector2 position, Func<Vector2> getTargetPosition)
    {
        var def = GameSettings.Seeker;
        var enemy = new Enemy(def.GetTexture(), position);
        enemy.PointValue = def.PointValue;
        enemy.AddBehaviour(enemy.FollowPlayer(getTargetPosition, def.Acceleration));
        return enemy;
    }

    public static Enemy CreateWanderer(Vector2 position)
    {
        var def = GameSettings.Wanderer;
        var enemy = new Enemy(def.GetTexture(), position);
        enemy.PointValue = def.PointValue;
        enemy.AddBehaviour(enemy.MoveRandomly());
        return enemy;
    }

    protected override void OnPreUpdate()
    {
        if (timeUntilStart <= 0)
            ApplyBehaviours();
        else
        {
            timeUntilStart--;
            color = Color.White * (1 - timeUntilStart / (float)GameSettings.EnemySpawnDelay);
        }
    }

    public void HandleCollision(Enemy other)
    {
        var d = Position - other.Position;
        Velocity += 10 * d / (d.LengthSquared() + 1);
    }

    // awardPoints = false when enemies are killed as a side-effect of the player dying,
    // so the player doesn't score points from their own death explosion.
    public void WasShot(bool awardPoints = true)
    {
        IsExpired = true;

        float hue1 = rand.NextFloat(0, 6);
        float hue2 = (hue1 + rand.NextFloat(0, 2)) % 6f;
        Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
        Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);
        for (int i = 0; i < GameSettings.EnemyDeathParticles; i++)
        {
            float speed = GameSettings.DeathParticleSpeed * (1f - 1 / rand.NextFloat(1f, 10f));
            var state = new ParticleState
            {
                Velocity = rand.NextVector2(speed, speed),
                Type = ParticleType.Enemy,
                LengthMultiplier = 1f
            };
            Color color = Color.Lerp(color1, color2, rand.NextFloat(0, 1));
            GameServices.Particles.CreateParticle(Art.LineParticle, Position, color, GameSettings.DeathParticleLife, GameSettings.DeathParticleSize, state);
        }

        if (awardPoints)
        {
            PlayerStatus.AddPoints(PointValue);
            PlayerStatus.IncreaseMultiplier();
        }
        Sound.Explosion.Play(0.5f, rand.NextFloat(-0.2f, 0.2f), 0);
    }

    IEnumerable<int> FollowPlayer(Func<Vector2> getTargetPosition, float acceleration)
    {
        while (true)
        {
            Velocity += (getTargetPosition() - Position).ScaleTo(acceleration);
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
            direction += rand.NextFloat(-GameSettings.WandererTurnRate, GameSettings.WandererTurnRate);
            direction = MathHelper.WrapAngle(direction);
            for (int i = 0; i < GameSettings.WandererStepsPerTick; i++)
            {
                Velocity += MathUtil.FromPolar(direction, GameSettings.WandererVelocity);
                Orientation -= GameSettings.WandererOrientationDecay;
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
