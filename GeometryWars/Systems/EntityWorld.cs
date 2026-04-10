using System.Collections.Generic;
using GeometryWars.Components.Identity;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Systems;

// Manages the lifecycle, updates, and rendering of all game entities.
public sealed class EntityWorld : INeighborQuery, IBulletSpawner
{
    private readonly List<Entity> _pendingAdd = [];
    private readonly EntityCatalog _catalog = new();
    private readonly CollisionSystem _collisions = new();

    private ObjectPool<Entity> _bulletPool;
    private bool _isUpdating;

    public IReadOnlyList<Entity> BlackHoles => _catalog.BlackHoles;
    public int Count => _catalog.Count;
    public int BlackHoleCount => _catalog.BlackHoleCount;

    public void ConfigureBulletFactory(System.Func<Entity> createBullet)
    {
        if (_bulletPool != null)
            throw new System.InvalidOperationException("Bullet factory has already been configured.");

        _bulletPool = new ObjectPool<Entity>(createBullet);
    }

    public Entity GetBullet(Vector2 position, Vector2 velocity)
    {
        if (_bulletPool == null)
            throw new System.InvalidOperationException("Bullet factory has not been configured.");

        var bullet = _bulletPool.Get();
        if (!bullet.HasComponent<BulletTag>())
            throw new System.InvalidOperationException("Bullet factory must create entities tagged as bullets.");

        var rigidbody = bullet.GetComponent<Rigidbody>() ?? throw new System.InvalidOperationException("Bullet entities must include a Rigidbody.");

        bullet.IsExpired = false;
        bullet.Transform.Position = position;
        bullet.Transform.Orientation = velocity.ToAngle();
        rigidbody.Velocity = velocity;
        return bullet;
    }

    public void SpawnBullet(Vector2 position, Vector2 velocity)
    {
        Add(GetBullet(position, velocity));
    }

    public void Add(Entity entity)
    {
        if (!_isUpdating) RegisterEntity(entity);
        else _pendingAdd.Add(entity);
    }

    public void Clear()
    {
        _catalog.Clear(HandleRemovedEntity);
        _pendingAdd.Clear();
        _collisions.Clear();
    }

    public void KillAllEnemies()
    {
        for (int i = 0; i < _catalog.Enemies.Count; i++)
        {
            var destroyable = _catalog.Enemies[i].GetComponent<Destroyable>();
            if (destroyable != null)
                destroyable.Destroy();
            else
                _catalog.Enemies[i].IsExpired = true;
        }
    }

    public void KillAllBlackHoles()
    {
        for (int i = 0; i < _catalog.BlackHoles.Count; i++)
        {
            var destroyable = _catalog.BlackHoles[i].GetComponent<Destroyable>();
            if (destroyable != null)
                destroyable.Destroy();
            else
                _catalog.BlackHoles[i].IsExpired = true;
        }
    }

    public void Update()
    {
        _isUpdating = true;
        _collisions.HandleCollisions();
        for (int i = 0; i < _catalog.Entities.Count; i++)
            _catalog.Entities[i].Update();
        _isUpdating = false;

        for (int i = 0; i < _pendingAdd.Count; i++)
            RegisterEntity(_pendingAdd[i]);
        _pendingAdd.Clear();

        _catalog.RemoveExpired(HandleRemovedEntity);
        _collisions.RemoveExpired();
    }

    public void Draw(SpriteBatch spriteBatch) => _catalog.Draw(spriteBatch);

    private void RegisterEntity(Entity entity)
    {
        entity.Start();
        _catalog.Add(entity);
        _collisions.Register(entity);
    }

    private void HandleRemovedEntity(Entity entity)
    {
        entity.Remove();

        if (entity.HasComponent<BulletTag>())
            _bulletPool?.Return(entity);
    }

    public void ForEachNearbyEntity(Vector2 position, float radius, System.Action<Entity> visitor)
        => _catalog.ForEachNearbyEntity(position, radius, visitor);
}
