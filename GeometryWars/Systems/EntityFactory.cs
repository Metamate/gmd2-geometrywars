using System;
using System.Collections.Generic;
using GeometryWars.Components.AI;
using GeometryWars.Components.Combat;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Components.Physics;
using GeometryWars.Components.Visuals;
using GeometryWars.Entities;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

// Centralizes entity archetype wiring so entities stay self-contained and
// components receive only the dependencies they actually need.
public sealed class EntityFactory
{
    private readonly ScoreTracker _score;
    private readonly ParticleManager<ParticleState> _particles;
    private readonly Grid _grid;
    private readonly EntityWorld _world;
    private readonly ISpawnController _spawner;
    private readonly GameRuntime _runtime;

    public EntityFactory(
        ScoreTracker score,
        ParticleManager<ParticleState> particles,
        Grid grid,
        EntityWorld world,
        ISpawnController spawner,
        GameRuntime runtime)
    {
        _score = score;
        _particles = particles;
        _grid = grid;
        _world = world;
        _spawner = spawner;
        _runtime = runtime;
    }

    public PlayerShip CreatePlayer()
    {
        var player = new PlayerShip
        {
            Transform = { Position = _runtime.Frame.ScreenSize / 2 }
        };

        Vector2 size = new(_runtime.Assets.Player.Width, _runtime.Assets.Player.Height);
        var respawn = new PlayerRespawnBehaviour(_score, _particles, _grid, _runtime);

        player.AddComponent(new RigidbodyComponent(damping: 0f));
        player.AddComponent(new ScreenClampBehaviour(size, _runtime));
        player.AddComponent(new SpriteComponent(_runtime.Assets.Player));
        player.AddComponent(new PlayerInputComponent(_world, _runtime));
        player.AddComponent(new ExhaustFireComponent(_particles, _runtime));
        player.AddComponent(new GlowOverlay(_runtime.Assets.Glow, Color.White * 0.15f));
        player.AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
        player.AddComponent(new PlayerCollisionBehaviour(_world, _spawner));
        player.AddComponent(respawn);

        return player;
    }

    public Bullet CreateBullet()
    {
        var bullet = new Bullet();
        bullet.AddComponent(new RigidbodyComponent(damping: 1f));
        bullet.AddComponent(new SpriteComponent(_runtime.Assets.Bullet));
        bullet.AddComponent(new BulletMovementBehaviour(_particles, _grid, _runtime));
        bullet.AddComponent(new BulletCollisionBehaviour());
        bullet.AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
        return bullet;
    }

    public Enemy CreateSeeker(Vector2 position, Func<Vector2> getTargetPosition)
    {
        var enemy = CreateEnemyBase(_runtime.Assets.Seeker, GameSettings.Enemy.SeekerPointValue, position);
        enemy.AddComponent(new SeekBehaviour(getTargetPosition, GameSettings.Enemy.SeekerAcceleration));
        return enemy;
    }

    public Enemy CreateWanderer(Vector2 position)
    {
        var enemy = CreateEnemyBase(_runtime.Assets.Wanderer, GameSettings.Enemy.WandererPointValue, position);
        enemy.AddComponent(new WanderBehaviour(_runtime));
        return enemy;
    }

    public BlackHole CreateBlackHole(Vector2 position)
    {
        var blackHole = new BlackHole
        {
            Transform = { Position = position }
        };

        blackHole.AddComponent(new RigidbodyComponent());
        blackHole.AddComponent(new SpriteComponent(_runtime.Assets.BlackHole));
        blackHole.AddComponent(new GlowOverlay(_runtime.Assets.Glow, Color.DarkViolet * 0.4f));
        blackHole.AddComponent(new GravityBehaviour(GameSettings.Hazards.BlackHoleGravityRange, GameSettings.Hazards.BlackHoleGravityForce, _world));
        blackHole.AddComponent(new SprayBehaviour(GameSettings.Hazards.BlackHoleGridRange, _particles, _grid, _runtime));
        blackHole.AddComponent(new BlackHoleCollisionBehaviour(GameSettings.Hazards.BlackHoleHitpoints, _particles, _runtime));
        blackHole.AddComponent(new CircleColliderComponent(_runtime.Assets.BlackHole.Width / 2f));

        return blackHole;
    }

    private Enemy CreateEnemyBase(Microsoft.Xna.Framework.Graphics.Texture2D texture, int pointValue, Vector2 position)
    {
        var enemy = new Enemy(pointValue)
        {
            Transform = { Position = position }
        };

        Vector2 size = new(texture.Width, texture.Height);
        enemy.AddComponent(new RigidbodyComponent(damping: GameSettings.Enemy.Damping));
        enemy.AddComponent(new ScreenClampBehaviour(size, _runtime));
        var sprite = enemy.AddComponent(new SpriteComponent(texture));
        sprite.Tint = Color.Transparent;

        enemy.AddComponent(new EnemyCollisionBehaviour(_score));
        enemy.AddComponent(new EnemyDeathEffectsComponent(_particles, PlayExplosionSound, _runtime));
        enemy.AddComponent(new CircleColliderComponent(size.X / 2f));
        enemy.AddComponent(new SpawnFadeBehaviour(GameSettings.Enemy.SpawnDelay));
        return enemy;
    }

    private void PlayExplosionSound()
    {
        _runtime.Audio.Play(_runtime.Assets.Explosion, 0.5f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}
