using System;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public class Enemy : Entity
{
    private readonly EnemyDef _def;
    private int _timeUntilStart = GameSettings.Enemy.SpawnDelay;

    public Enemy(EnemyDef def, Vector2 position)
    {
        _def     = def;
        Image    = def.GetTexture();
        Position = position;
        Tint     = Color.Transparent;

        // Common movement: decelerate each frame and stay on screen.
        AddComponent(new VelocityMover(damping: GameSettings.Enemy.Damping, clampToScreen: true));

        Collider = new CircleCollider(Image.Width / 2f);
    }

    public bool IsActive => _timeUntilStart <= 0;
    public int PointValue => _def.PointValue;

    // Component-based assembly: we create a generic Enemy and "plug in" its AI.
    public static Enemy CreateSeeker(Vector2 position, Func<Vector2> getTargetPosition)
    {
        var def   = GameSettings.Enemy.Seeker;
        var enemy = new Enemy(def, position);
        enemy.AddComponent(new SeekBehaviour(getTargetPosition, def.Acceleration));
        return enemy;
    }

    public static Enemy CreateWanderer(Vector2 position)
    {
        var def   = GameSettings.Enemy.Wanderer;
        var enemy = new Enemy(def, position);
        enemy.AddComponent(new WanderBehaviour());
        return enemy;
    }

    protected override void OnUpdate()
    {
        if (_timeUntilStart > 0)
        {
            _timeUntilStart--;
            Tint = Color.White * (1 - _timeUntilStart / (float)GameSettings.Enemy.SpawnDelay);
        }
        
        // Note: AI logic (Seeker/Wanderer) has been moved to dedicated components.
        // Components only update if IsActive is true.
    }

    public override void OnCollision(Entity other)
    {
        if (other is Bullet || (other is BlackHole && IsActive))
            WasShot();
        else if (other is Enemy e)
            HandleCollision(e);
    }

    public void HandleCollision(Enemy other)
    {
        var d = Position - other.Position;
        Velocity += 10 * d / (d.LengthSquared() + 1);
    }

    public void WasShot(bool awardPoints = true)
    {
        IsExpired = true;

        float hue1 = Random.Shared.NextFloat(0, 6);
        float hue2 = (hue1 + Random.Shared.NextFloat(0, 2)) % 6f;
        Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
        Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);
        for (int i = 0; i < GameSettings.Visuals.EnemyDeathParticles; i++)
        {
            float speed = GameSettings.Visuals.DeathParticleSpeed * (1f - 1 / Random.Shared.NextFloat(1f, 10f));
            var state = new ParticleState
            {
                Velocity = Random.Shared.NextVector2(speed, speed),
                Type = ParticleType.Enemy,
                LengthMultiplier = 1f
            };
            Color color = Color.Lerp(color1, color2, Random.Shared.NextFloat(0, 1));
            GameServices.Particles.CreateParticle(Art.LineParticle, Position, color, GameSettings.Visuals.DeathParticleLife, GameSettings.Visuals.DeathParticleSize, state);
        }

        if (awardPoints)
        {
            PlayerStatus.AddPoints(PointValue);
            PlayerStatus.IncreaseMultiplier();
        }
        GameServices.Audio.Play(Sound.Explosion, 0.5f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}
