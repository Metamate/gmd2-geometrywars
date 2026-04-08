using System;
using GeometryWars.Components.Physics;
using GeometryWars.Components.Visuals;
using GeometryWars.Components.Combat;
using GeometryWars.Components.AI;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Entities;

// Archetype for enemy units.
public class Enemy : Entity
{
    public int PointValue { get; }

    public Enemy(Texture2D texture, int pointValue, Vector2 position)
    {
        PointValue = pointValue;
        Transform.Position = position;

        Vector2 size = new(texture.Width, texture.Height);

        AddComponent(new RigidbodyComponent(damping: GameSettings.Enemy.Damping));
        AddComponent(new ScreenClampBehaviour(size));

        var sprite = AddComponent(new SpriteComponent(texture));
        sprite.Tint = Color.Transparent;

        AddComponent(new EnemyCollisionBehaviour());
        AddComponent(new CircleColliderComponent(size.X / 2f));
        AddComponent(new SpawnFadeBehaviour(GameSettings.Enemy.SpawnDelay));
    }

    public static Enemy CreateSeeker(Vector2 position, Func<Vector2> getTargetPosition)
    {
        var enemy = new Enemy(Art.Seeker, GameSettings.Enemy.SeekerPointValue, position);
        enemy.AddComponent(new SeekBehaviour(getTargetPosition, GameSettings.Enemy.SeekerAcceleration));
        return enemy;
    }

    public static Enemy CreateWanderer(Vector2 position)
    {
        var enemy = new Enemy(Art.Wanderer, GameSettings.Enemy.WandererPointValue, position);
        enemy.AddComponent(new WanderBehaviour());
        return enemy;
    }

    public void Kill()
    {
        IsExpired = true;
        var pos = Transform.Position;

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
            GameServices.Particles.CreateParticle(Art.LineParticle, pos, color, GameSettings.Visuals.DeathParticleLife, GameSettings.Visuals.DeathParticleSize, state);
        }

        GameServices.Audio.Play(Sound.Explosion, 0.5f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}
