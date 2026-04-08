using System;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public class Enemy : Entity
{
    private readonly EnemyDef _def;
    public bool IsActive { get; set; }
    public int PointValue => _def.PointValue;

    public Enemy(EnemyDef def, Vector2 position)
    {
        _def     = def;

        var tex = def.GetTexture();
        Vector2 size = new(tex.Width, tex.Height);

        // Assembler: Composition of specific capabilities
        AddComponent(new TransformComponent(position));
        AddComponent(new RigidbodyComponent(damping: GameSettings.Enemy.Damping));
        AddComponent(new ScreenClampBehaviour(size));
        
        var sprite = AddComponent(new SpriteComponent(tex));
        sprite.Tint = Color.Transparent; // Set tint on the SpriteComponent, not the Entity

        AddComponent(new SpawnFadeBehaviour(GameSettings.Enemy.SpawnDelay));
        AddComponent(new EnemyCollisionBehaviour());
        AddComponent(new CircleColliderComponent(size.X / 2f));
    }

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

    public void Kill()
    {
        IsExpired = true;
        var pos = Transform?.Position ?? Vector2.Zero;

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
