using System;
using GeometryWars2.Definitions;
using GeometryWars2.Components.AI;
using GeometryWars2.Components.Combat;
using GeometryWars2.Components.Identity;
using GeometryWars2.Components.Input;
using GeometryWars2.Components.Lifecycle;
using GeometryWars2.Components.Physics;
using GMDCore.Physics;
using GeometryWars2.Components.Visuals;
using GMDCore.ECS;
using GMDCore.Particles;
using GeometryWars2.Services;
using GeometryWars2.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars2.Systems;

// Centralizes entity recipe wiring so entities stay self-contained and
// components receive only the dependencies they actually need.
public sealed class EntityFactory
{
    private static readonly PlayerDefinition PlayerDefinition = GameplayDefinitions.Player;
    private static readonly BulletDefinition BulletDefinition = GameplayDefinitions.Bullet;
    private static readonly SeekerEnemyDefinition SeekerDefinition = GameplayDefinitions.Seeker;
    private static readonly WanderEnemyDefinition WandererDefinition = GameplayDefinitions.Wanderer;

    private readonly ScoreTracker _score;
    private readonly IBulletSpawner _bulletSpawner;
    private readonly PlayContext _context;

    public EntityFactory(
        ScoreTracker score,
        IBulletSpawner bulletSpawner,
        PlayContext context)
    {
        _score = score;
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
        var respawnState = new RespawnState(_score);

        player.AddComponent(new Rigidbody(damping: 0f));
        player.AddComponent(new ClampToScreen(size, _context.Frame));
        player.AddComponent(new Sprite(playerTexture));
        player.AddComponent(new ApplyMovementInput(_context.Controller, PlayerDefinition.MoveSpeed));
        player.AddComponent(new Weapon(PlayerDefinition.PrimaryWeapon.CooldownFrames));
        player.AddComponent(new FireWeaponOnInput(_context.Controller));
        player.AddComponent(new SpawnTwinBulletsOnFired(_bulletSpawner, PlayerDefinition.PrimaryWeapon));
        player.AddComponent(new GlowOverlay(_context.Assets.Glow, Color.White * PlayerDefinition.GlowOpacity));
        player.AddComponent(new CircleCollider(PlayerDefinition.ColliderRadius));
        player.AddComponent(new BeginRespawnOnLethalCollision());
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
        bullet.AddComponent(new ExpireOutsideViewport(_context.Frame));
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
        enemy.AddComponent(new CircleCollider(size.X / 2f));
        enemy.AddComponent(new FadeInOnSpawn(definition.SpawnDelayFrames));
        return enemy;
    }
}
