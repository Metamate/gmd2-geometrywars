using System;
using GeometryWars.Components.Audio;
using GeometryWars.Components.AI;
using GeometryWars.Components.Combat;
using GeometryWars.Components.Identity;
using GeometryWars.Components.Input;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Components.Physics;
using GMDCore.Physics;
using GeometryWars.Components.Visuals;
using GMDCore.ECS;
using GMDCore.Particles;
using GeometryWars.Services;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

// Centralizes entity recipe wiring so entities stay self-contained and
// components receive only the dependencies they actually need.
public sealed class EntityFactory
{
    private readonly ScoreTracker _score;
    private readonly ParticleManager<ParticleState> _particles;
    private readonly Grid _grid;
    private readonly INeighborQuery _neighborQuery;
    private readonly IBulletSpawner _bulletSpawner;
    private readonly PlayContext _context;

    public EntityFactory(
        ScoreTracker score,
        ParticleManager<ParticleState> particles,
        Grid grid,
        INeighborQuery neighborQuery,
        IBulletSpawner bulletSpawner,
        PlayContext context)
    {
        _score = score;
        _particles = particles;
        _grid = grid;
        _neighborQuery = neighborQuery;
        _bulletSpawner = bulletSpawner;
        _context = context;
    }

    // Direct-control player ship with movement, twin-shot firing,
    // death/respawn flow, and ship-specific presentation effects.
    public Entity CreatePlayer()
    {
        var player = new Entity
        {
            Transform = { Position = _context.Frame.ScreenSize / 2 }
        };

        Vector2 size = new(_context.Assets.Player.Width, _context.Assets.Player.Height);
        var respawnEffects = new PlayRespawnEffects(_particles, _grid, _context.Assets.LineParticle);
        var respawnState = new RespawnState(_score);

        player.AddComponent(new Rigidbody(damping: 0f));
        player.AddComponent(new ClampToScreen(size, _context.Frame));
        player.AddComponent(new Sprite(_context.Assets.Player));
        player.AddComponent(new ApplyMovementInput(_context.Controller));
        player.AddComponent(new Weapon(GameSettings.Bullets.ShotCooldown));
        player.AddComponent(new FireWeaponOnInput(_context.Controller));
        player.AddComponent(new SpawnTwinBulletsOnFired(_bulletSpawner));
        player.AddComponent(new PlaySoundOnWeaponFired(_context.Audio, _context.Assets.GetRandomShot));
        player.AddComponent(new ExhaustFire(_particles, _context.Frame, _context.Assets.LineParticle, _context.Assets.Glow));
        player.AddComponent(new GlowOverlay(_context.Assets.Glow, Color.White * 0.15f));
        player.AddComponent(new CircleCollider(GameSettings.Bullets.ColliderRadius));
        player.AddComponent(new BeginRespawnOnLethalCollision());
        player.AddComponent(respawnEffects);
        player.AddComponent(respawnState);

        return player;
    }

    // Lightweight pooled projectile with simple motion, collision expiry,
    // and trail/grid feedback.
    public Entity CreateBullet()
    {
        var bullet = new Entity();
        bullet.AddComponent(new BulletTag());
        bullet.AddComponent(new Rigidbody(damping: 1f));
        bullet.AddComponent(new Sprite(_context.Assets.Bullet));
        bullet.AddComponent(new FaceVelocity());
        bullet.AddComponent(new ExpireOutsideViewportWithParticles(_particles, _context.Frame, _context.Assets.LineParticle));
        bullet.AddComponent(new ApplyGridForceFromVelocity(_grid, GameSettings.Physics.BulletGridForce, GameSettings.Physics.BulletGridRadius));
        bullet.AddComponent(new ExpireOnEnemyOrBlackHoleCollision());
        bullet.AddComponent(new CircleCollider(GameSettings.Bullets.ColliderRadius));
        return bullet;
    }

    // Pursuit enemy that reuses the shared enemy shell and adds target seeking.
    public Entity CreateSeeker(Vector2 position, Func<Vector2> getTargetPosition)
    {
        var enemy = CreateEnemyBase(_context.Assets.Seeker, GameSettings.Enemy.SeekerPointValue, position);
        enemy.AddComponent(new SeekTarget(getTargetPosition, GameSettings.Enemy.SeekerAcceleration));
        return enemy;
    }

    // Erratic enemy that reuses the shared enemy shell and adds wandering motion.
    public Entity CreateWanderer(Vector2 position)
    {
        var enemy = CreateEnemyBase(_context.Assets.Wanderer, GameSettings.Enemy.WandererPointValue, position);
        enemy.AddComponent(new Wander(_context.Frame, new Vector2(_context.Assets.Wanderer.Width, _context.Assets.Wanderer.Height)));
        return enemy;
    }

    // Environmental hazard that pulls nearby objects, disturbs the grid,
    // emits ambient particles, and can be worn down by bullets.
    public Entity CreateBlackHole(Vector2 position)
    {
        var blackHole = new Entity
        {
            Transform = { Position = position }
        };

        blackHole.AddComponent(new BlackHoleTag());
        blackHole.AddComponent(new Rigidbody());
        blackHole.AddComponent(new Sprite(_context.Assets.BlackHole));
        blackHole.AddComponent(new GlowOverlay(_context.Assets.Glow, Color.DarkViolet * 0.4f));
        blackHole.AddComponent(new ApplyGravity(GameSettings.Hazards.BlackHoleGravityRange, GameSettings.Hazards.BlackHoleGravityForce, _neighborQuery));
        blackHole.AddComponent(new EmitOrbitingParticles(_particles, _context.Frame, _context.Assets.LineParticle));
        blackHole.AddComponent(new ApplyOscillatingImplosiveGridForce(GameSettings.Hazards.BlackHoleGridRange, _grid));
        blackHole.AddComponent(new Destroyable());
        blackHole.AddComponent(new Health(GameSettings.Hazards.BlackHoleHitpoints));
        blackHole.AddComponent(new TakeDamageOnBulletCollision());
        blackHole.AddComponent(new DestroyWhenHealthDepleted());
        blackHole.AddComponent(new PlayHitParticlesOnDamage(_particles, _context.Frame, _context.Assets.LineParticle));
        blackHole.AddComponent(new CircleCollider(_context.Assets.BlackHole.Width / 2f));

        return blackHole;
    }

    // Shared enemy shell: movement, spawn-in, collision rules, scoring,
    // and destruction feedback. Specific enemy types add only their unique AI.
    private Entity CreateEnemyBase(Microsoft.Xna.Framework.Graphics.Texture2D texture, int pointValue, Vector2 position)
    {
        var enemy = new Entity
        {
            Transform = { Position = position }
        };

        Vector2 size = new(texture.Width, texture.Height);
        enemy.AddComponent(new EnemyTag());
        enemy.AddComponent(new Rigidbody(damping: GameSettings.Enemy.Damping));
        enemy.AddComponent(new ClampToScreen(size, _context.Frame));
        var sprite = enemy.AddComponent(new Sprite(texture));
        sprite.Tint = Color.Transparent;

        enemy.AddComponent(new Destroyable());
        enemy.AddComponent(new DestroyOnBulletOrBlackHoleCollision());
        enemy.AddComponent(new RepelFromEnemies());
        enemy.AddComponent(new AwardScoreOnDestroyed(_score, pointValue));
        enemy.AddComponent(new PlayBurstParticlesOnDestroyed(_particles, _context.Assets.LineParticle, GameSettings.Visuals.EnemyDeathParticles, velocity => ParticleState.StableTrail(velocity)));
        enemy.AddComponent(new PlaySoundOnDestroyed(PlayExplosionSound));
        enemy.AddComponent(new CircleCollider(size.X / 2f));
        enemy.AddComponent(new FadeInOnSpawn(GameSettings.Enemy.SpawnDelay));
        return enemy;
    }

    private void PlayExplosionSound()
    {
        _context.Audio.Play(_context.Assets.GetRandomExplosion(), 0.5f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}

