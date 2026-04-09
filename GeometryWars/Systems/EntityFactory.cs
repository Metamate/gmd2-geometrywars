using System;
using System.Collections.Generic;
using GeometryWars.Components.AI;
using GeometryWars.Components.Combat;
using GeometryWars.Components.Input;
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
    private readonly PlayContext _context;

    public EntityFactory(
        ScoreTracker score,
        ParticleManager<ParticleState> particles,
        Grid grid,
        EntityWorld world,
        ISpawnController spawner,
        PlayContext context)
    {
        _score = score;
        _particles = particles;
        _grid = grid;
        _world = world;
        _spawner = spawner;
        _context = context;
    }

    public PlayerShip CreatePlayer()
    {
        var player = new PlayerShip
        {
            Transform = { Position = _context.Frame.ScreenSize / 2 }
        };

        Vector2 size = new(_context.Assets.Player.Width, _context.Assets.Player.Height);
        var respawnEffects = new PlayRespawnEffectsComponent(_particles, _grid, _context.Assets.LineParticle);
        var respawnState = new RespawnStateComponent(_score);

        player.AddComponent(new RigidbodyComponent(damping: 0f));
        player.AddComponent(new ScreenClampBehaviour(size, _context.Frame));
        player.AddComponent(new SpriteComponent(_context.Assets.Player));
        player.AddComponent(new ApplyMovementInputComponent(_context.Controller));
        player.AddComponent(new FireBulletsOnInputComponent(_world, _context.Controller, _context.Audio, () => _context.Assets.Shot));
        player.AddComponent(new ExhaustFireComponent(_particles, _context.Frame, _context.Assets.LineParticle, _context.Assets.Glow));
        player.AddComponent(new GlowOverlay(_context.Assets.Glow, Color.White * 0.15f));
        player.AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
        player.AddComponent(new PlayerCollisionBehaviour(_world, _spawner));
        player.AddComponent(respawnEffects);
        player.AddComponent(respawnState);

        return player;
    }

    public Bullet CreateBullet()
    {
        var bullet = new Bullet();
        bullet.AddComponent(new RigidbodyComponent(damping: 1f));
        bullet.AddComponent(new SpriteComponent(_context.Assets.Bullet));
        bullet.AddComponent(new FaceVelocityComponent());
        bullet.AddComponent(new ExpireOutsideViewportWithParticlesComponent(_particles, _context.Frame, _context.Assets.LineParticle));
        bullet.AddComponent(new ApplyGridForceFromVelocityComponent(_grid, GameSettings.Physics.BulletGridForce, GameSettings.Physics.BulletGridRadius));
        bullet.AddComponent(new BulletCollisionBehaviour());
        bullet.AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
        return bullet;
    }

    public Enemy CreateSeeker(Vector2 position, Func<Vector2> getTargetPosition)
    {
        var enemy = CreateEnemyBase(_context.Assets.Seeker, GameSettings.Enemy.SeekerPointValue, position);
        enemy.AddComponent(new SeekBehaviour(getTargetPosition, GameSettings.Enemy.SeekerAcceleration));
        return enemy;
    }

    public Enemy CreateWanderer(Vector2 position)
    {
        var enemy = CreateEnemyBase(_context.Assets.Wanderer, GameSettings.Enemy.WandererPointValue, position);
        enemy.AddComponent(new WanderBehaviour(_context.Frame, new Vector2(_context.Assets.Wanderer.Width, _context.Assets.Wanderer.Height)));
        return enemy;
    }

    public BlackHole CreateBlackHole(Vector2 position)
    {
        var blackHole = new BlackHole
        {
            Transform = { Position = position }
        };

        blackHole.AddComponent(new RigidbodyComponent());
        blackHole.AddComponent(new SpriteComponent(_context.Assets.BlackHole));
        blackHole.AddComponent(new GlowOverlay(_context.Assets.Glow, Color.DarkViolet * 0.4f));
        blackHole.AddComponent(new GravityBehaviour(GameSettings.Hazards.BlackHoleGravityRange, GameSettings.Hazards.BlackHoleGravityForce, _world));
        blackHole.AddComponent(new EmitOrbitingParticlesComponent(_particles, _context.Frame, _context.Assets.LineParticle));
        blackHole.AddComponent(new ApplyOscillatingImplosiveGridForceComponent(GameSettings.Hazards.BlackHoleGridRange, _grid));
        blackHole.AddComponent(new HealthComponent(GameSettings.Hazards.BlackHoleHitpoints));
        blackHole.AddComponent(new TakeDamageOnBulletCollisionComponent());
        blackHole.AddComponent(new ExpireWhenHealthDepletedComponent());
        blackHole.AddComponent(new PlayHitParticlesOnBulletCollisionComponent(_particles, _context.Frame, _context.Assets.LineParticle));
        blackHole.AddComponent(new CircleColliderComponent(_context.Assets.BlackHole.Width / 2f));

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
        enemy.AddComponent(new ScreenClampBehaviour(size, _context.Frame));
        var sprite = enemy.AddComponent(new SpriteComponent(texture));
        sprite.Tint = Color.Transparent;

        enemy.AddComponent(new EnemyCollisionBehaviour(_score));
        enemy.AddComponent(new EnemyDeathEffectsComponent(_particles, PlayExplosionSound, _context.Assets.LineParticle));
        enemy.AddComponent(new CircleColliderComponent(size.X / 2f));
        enemy.AddComponent(new SpawnFadeBehaviour(GameSettings.Enemy.SpawnDelay));
        return enemy;
    }

    private void PlayExplosionSound()
    {
        _context.Audio.Play(_context.Assets.Explosion, 0.5f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}
