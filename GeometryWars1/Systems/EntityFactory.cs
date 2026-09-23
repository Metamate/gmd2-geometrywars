using System;
using GeometryWars1.Definitions;
using GeometryWars1.Components.Combat;
using GeometryWars1.Components.Identity;
using GeometryWars1.Components.Input;
using GeometryWars1.Components.Lifecycle;
using GeometryWars1.Components.Physics;
using GMDCore.Physics;
using GeometryWars1.Components.Visuals;
using GMDCore.ECS;
using GMDCore.Particles;
using GeometryWars1.Services;
using GeometryWars1.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars1.Systems;

// Centralizes entity recipe wiring so entities stay self-contained and
// components receive only the dependencies they actually need.
public sealed class EntityFactory
{
    private static readonly PlayerDefinition PlayerDefinition = GameplayDefinitions.Player;
    private static readonly BulletDefinition BulletDefinition = GameplayDefinitions.Bullet;

    private readonly IBulletSpawner _bulletSpawner;
    private readonly PlayContext _context;

    public EntityFactory(
        IBulletSpawner bulletSpawner,
        PlayContext context)
    {
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

        player.AddComponent(new Rigidbody(damping: 0f));
        player.AddComponent(new ClampToScreen(size, _context.Frame));
        player.AddComponent(new Sprite(playerTexture));
        player.AddComponent(new ApplyMovementInput(_context.Controller, PlayerDefinition.MoveSpeed));
        player.AddComponent(new Weapon(PlayerDefinition.PrimaryWeapon.CooldownFrames));
        player.AddComponent(new FireWeaponOnInput(_context.Controller));
        player.AddComponent(new SpawnTwinBulletsOnFired(_bulletSpawner, PlayerDefinition.PrimaryWeapon));
        player.AddComponent(new GlowOverlay(_context.Assets.Glow, Color.White * PlayerDefinition.GlowOpacity));

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
        bullet.AddComponent(new CircleCollider(BulletDefinition.ColliderRadius));
        return bullet;
    }
}
