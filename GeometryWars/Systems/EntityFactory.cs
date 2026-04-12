using System;
using GeometryWars.Definitions;
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
    private static readonly PlayerDefinition PlayerDefinition = GameplayDefinitions.Player;
    private static readonly BulletDefinition BulletDefinition = GameplayDefinitions.Bullet;
    private static readonly SeekerEnemyDefinition SeekerDefinition = GameplayDefinitions.Seeker;
    private static readonly WanderEnemyDefinition WandererDefinition = GameplayDefinitions.Wanderer;
    private static readonly BlackHoleDefinition BlackHoleDefinition = GameplayDefinitions.BlackHole;

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
        var playerTexture = _context.Assets.GetTexture(PlayerDefinition.SpriteId);
        var player = new Entity
        {
            Transform = { Position = _context.Frame.ScreenSize / 2 }
        };

        Vector2 size = new(playerTexture.Width, playerTexture.Height);
        var respawnEffects = new PlayRespawnEffects(_particles, _grid, _context.Assets.LineParticle, PlayerDefinition);
        var respawnState = new RespawnState(_score);

        player.AddComponent(new Rigidbody(damping: 0f));
        player.AddComponent(new ClampToScreen(size, _context.Frame));
        player.AddComponent(new Sprite(playerTexture));
        player.AddComponent(new ApplyMovementInput(_context.Controller, PlayerDefinition.MoveSpeed));
        player.AddComponent(new Weapon(PlayerDefinition.PrimaryWeapon.CooldownFrames));
        player.AddComponent(new FireWeaponOnInput(_context.Controller));
        player.AddComponent(new SpawnTwinBulletsOnFired(_bulletSpawner, PlayerDefinition.PrimaryWeapon));
        player.AddComponent(new PlaySoundOnWeaponFired(_context.Audio, _context.Assets.GetRandomShot));
        player.AddComponent(new ExhaustFire(_particles, _context.Frame, _context.Assets.LineParticle, _context.Assets.Glow));
        player.AddComponent(new GlowOverlay(_context.Assets.Glow, Color.White * PlayerDefinition.GlowOpacity));
        player.AddComponent(new CircleCollider(PlayerDefinition.ColliderRadius));
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
        bullet.AddComponent(new Rigidbody(damping: BulletDefinition.RigidbodyDamping));
        bullet.AddComponent(new Sprite(_context.Assets.GetTexture(BulletDefinition.SpriteId)));
        bullet.AddComponent(new FaceVelocity());
        bullet.AddComponent(new ExpireOutsideViewportWithParticles(_particles, _context.Frame, _context.Assets.LineParticle, BulletDefinition));
        bullet.AddComponent(new ApplyGridForceFromVelocity(_grid, BulletDefinition.GridForce, BulletDefinition.GridRadius));
        bullet.AddComponent(new ExpireOnEnemyOrBlackHoleCollision());
        bullet.AddComponent(new CircleCollider(BulletDefinition.ColliderRadius));
        return bullet;
    }

    // Pursuit enemy that reuses the shared enemy shell and adds target seeking.
    public Entity CreateSeeker(Vector2 position, Func<Vector2> getTargetPosition)
    {
        var enemy = CreateEnemyBase(SeekerDefinition.Shell, position);
        enemy.AddComponent(new SeekTarget(getTargetPosition, SeekerDefinition.Acceleration));
        return enemy;
    }

    // Erratic enemy that reuses the shared enemy shell and adds wandering motion.
    public Entity CreateWanderer(Vector2 position)
    {
        var enemy = CreateEnemyBase(WandererDefinition.Shell, position);
        var texture = _context.Assets.GetTexture(WandererDefinition.Shell.SpriteId);
        enemy.AddComponent(new Wander(_context.Frame, new Vector2(texture.Width, texture.Height), WandererDefinition));
        return enemy;
    }

    // Environmental hazard that pulls nearby objects, disturbs the grid,
    // emits ambient particles, and can be worn down by bullets.
    public Entity CreateBlackHole(Vector2 position)
    {
        var texture = _context.Assets.GetTexture(BlackHoleDefinition.SpriteId);
        var blackHole = new Entity
        {
            Transform = { Position = position }
        };

        blackHole.AddComponent(new BlackHoleTag());
        blackHole.AddComponent(new Rigidbody());
        blackHole.AddComponent(new Sprite(texture));
        blackHole.AddComponent(new GlowOverlay(_context.Assets.Glow, Color.DarkViolet * BlackHoleDefinition.GlowOpacity));
        blackHole.AddComponent(new ApplyGravity(BlackHoleDefinition.GravityRange, BlackHoleDefinition.GravityForce, _neighborQuery));
        blackHole.AddComponent(new EmitOrbitingParticles(_particles, _context.Frame, _context.Assets.LineParticle));
        blackHole.AddComponent(new ApplyOscillatingImplosiveGridForce(BlackHoleDefinition.GridRange, _grid));
        blackHole.AddComponent(new Destroyable());
        blackHole.AddComponent(new Health(BlackHoleDefinition.Hitpoints));
        blackHole.AddComponent(new TakeDamageOnBulletCollision());
        blackHole.AddComponent(new DestroyWhenHealthDepleted());
        blackHole.AddComponent(new PlayHitParticlesOnDamage(_particles, _context.Frame, _context.Assets.LineParticle, BlackHoleDefinition));
        blackHole.AddComponent(new CircleCollider(texture.Width / 2f));

        return blackHole;
    }

    // Shared enemy shell: movement, spawn-in, collision rules, scoring,
    // and destruction feedback. Specific enemy types add only their unique AI.
    private Entity CreateEnemyBase(EnemyShellDefinition definition, Vector2 position)
    {
        var texture = _context.Assets.GetTexture(definition.SpriteId);
        var enemy = new Entity
        {
            Transform = { Position = position }
        };

        Vector2 size = new(texture.Width, texture.Height);
        enemy.AddComponent(new EnemyTag());
        enemy.AddComponent(new Rigidbody(damping: definition.RigidbodyDamping));
        enemy.AddComponent(new ClampToScreen(size, _context.Frame));
        var sprite = enemy.AddComponent(new Sprite(texture));
        sprite.Tint = Color.Transparent;

        enemy.AddComponent(new Destroyable());
        enemy.AddComponent(new DestroyOnBulletOrBlackHoleCollision());
        enemy.AddComponent(new RepelFromEnemies());
        enemy.AddComponent(new AwardScoreOnDestroyed(_score, definition.PointValue));
        enemy.AddComponent(new PlayBurstParticlesOnDestroyed(_particles, _context.Assets.LineParticle, definition.DeathParticleCount, velocity => ParticleState.StableTrail(velocity)));
        enemy.AddComponent(new PlaySoundOnDestroyed(PlayExplosionSound));
        enemy.AddComponent(new CircleCollider(size.X / 2f));
        enemy.AddComponent(new FadeInOnSpawn(definition.SpawnDelayFrames));
        return enemy;
    }

    private void PlayExplosionSound()
    {
        _context.Audio.Play(_context.Assets.GetRandomExplosion(), 0.5f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}

