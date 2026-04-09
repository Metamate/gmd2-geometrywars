using System.Collections.Generic;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Systems;

// Manages the lifecycle, updates, and rendering of all game entities.
public sealed class EntityWorld : INeighborQuery, IEntitySweeper, IBulletSpawner
{
    private readonly List<Entity> _pendingAdd = [];
    private readonly EntityCatalog _catalog = new();
    private readonly CollisionSystem _collisions = new();

    private ObjectPool<Bullet> _bulletPool;
    private bool _isUpdating;

    public IReadOnlyList<BlackHole> BlackHoles => _catalog.BlackHoles;
    public int Count => _catalog.Count;
    public int BlackHoleCount => _catalog.BlackHoleCount;

    public void ConfigureBulletFactory(System.Func<Bullet> createBullet)
    {
        if (_bulletPool != null)
            throw new System.InvalidOperationException("Bullet factory has already been configured.");

        _bulletPool = new ObjectPool<Bullet>(createBullet);
    }

    public Bullet GetBullet(Vector2 position, Vector2 velocity)
    {
        if (_bulletPool == null)
            throw new System.InvalidOperationException("Bullet factory has not been configured.");

        var bullet = _bulletPool.Get();
        bullet.Reset(position, velocity);
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
        _catalog.Clear();
        _pendingAdd.Clear();
        _collisions.Clear();
    }

    public void KillAllEnemies()
    {
        for (int i = 0; i < _catalog.Enemies.Count; i++)
            _catalog.Enemies[i].Kill();
    }

    public void KillAllBlackHoles()
    {
        for (int i = 0; i < _catalog.BlackHoles.Count; i++)
            _catalog.BlackHoles[i].Kill();
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

        _catalog.RemoveExpired(bullet => _bulletPool.Return(bullet));
        _collisions.RemoveExpired();
    }

    public void Draw(SpriteBatch spriteBatch) => _catalog.Draw(spriteBatch);

    private void RegisterEntity(Entity entity)
    {
        entity.Start();
        _catalog.Add(entity);
        _collisions.Register(entity);
    }

    public void ForEachNearbyEntity(Vector2 position, float radius, System.Action<Entity> visitor)
        => _catalog.ForEachNearbyEntity(position, radius, visitor);
}
