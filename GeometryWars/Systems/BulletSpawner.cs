using System;
using GeometryWars.Components.Identity;
using GeometryWars.Utils;
using GMDCore.Collections;
using GMDCore.ECS;
using GMDCore.Physics;
using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

// Owns pooled bullet creation/reuse without baking projectile rules into EntityWorld.
public sealed class BulletSpawner : IBulletSpawner
{
    private readonly EntityWorld _world;
    private ObjectPool<Entity> _pool;
    private bool _isShutdown;

    public BulletSpawner(EntityWorld world)
    {
        _world = world;
        _world.EntityRemoved += HandleEntityRemoved;
    }

    public void ConfigureFactory(Func<Entity> createBullet)
    {
        if (_pool != null)
            throw new InvalidOperationException("Bullet factory has already been configured.");

        _pool = new ObjectPool<Entity>(createBullet);
    }

    public void SpawnBullet(Vector2 position, Vector2 velocity)
    {
        if (_isShutdown)
            throw new InvalidOperationException("Bullet spawner cannot spawn after shutdown.");

        if (_pool == null)
            throw new InvalidOperationException("Bullet factory has not been configured.");

        var bullet = _pool.Get();
        if (!bullet.HasComponent<BulletTag>())
            throw new InvalidOperationException("Bullet factory must create entities tagged as bullets.");

        var rigidbody = bullet.GetComponent<Rigidbody>()
            ?? throw new InvalidOperationException("Bullet entities must include a Rigidbody.");

        bullet.IsExpired = false;
        bullet.Transform.Position = position;
        bullet.Transform.Orientation = velocity.ToAngle();
        rigidbody.Velocity = velocity;
        _world.Add(bullet);
    }

    private void HandleEntityRemoved(Entity entity)
    {
        if (entity.HasComponent<BulletTag>())
            _pool?.Return(entity);
    }

    public void Shutdown()
    {
        if (_isShutdown)
            return;

        _world.EntityRemoved -= HandleEntityRemoved;
        _isShutdown = true;
    }
}
