using System.Collections.Generic;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Systems;

// Manages the lifecycle, updates, and rendering of all game entities.
public sealed class EntityWorld : INeighborQuery, IEntitySweeper, IBulletSpawner
{
    private readonly List<Entity> _entities = [];
    private readonly List<Enemy> _enemies = [];
    private readonly List<Bullet> _bullets = [];
    private readonly List<BlackHole> _blackHoles = [];
    private readonly List<Entity> _pendingAdd = [];
    
    private readonly List<(Entity Entity, ColliderComponent Collider)> _collidables = [];

    private ObjectPool<Bullet> _bulletPool;
    private bool _isUpdating;

    public IReadOnlyList<BlackHole> BlackHoles => _blackHoles;
    public int Count => _entities.Count;
    public int BlackHoleCount => _blackHoles.Count;

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
        _entities.Clear();
        _enemies.Clear();
        _bullets.Clear();
        _blackHoles.Clear();
        _pendingAdd.Clear();
        _collidables.Clear();
    }

    public void KillAllEnemies()
    {
        for (int i = 0; i < _enemies.Count; i++)
            _enemies[i].Kill();
    }

    public void KillAllBlackHoles()
    {
        for (int i = 0; i < _blackHoles.Count; i++)
            _blackHoles[i].Kill();
    }

    public void Update()
    {
        _isUpdating = true;
        HandleCollisions();
        foreach (var entity in _entities)
            entity.Update();
        _isUpdating = false;

        foreach (var entity in _pendingAdd)
            RegisterEntity(entity);
        _pendingAdd.Clear();

        foreach (var b in _bullets)
            if (b.IsExpired) _bulletPool.Return(b);

        _entities.RemoveAll(e => e.IsExpired);
        _bullets.RemoveAll(b => b.IsExpired);
        _enemies.RemoveAll(e => e.IsExpired);
        _blackHoles.RemoveAll(bh => bh.IsExpired);
        _collidables.RemoveAll(c => c.Entity.IsExpired);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in _entities)
            entity.Draw(spriteBatch);
    }

    private void RegisterEntity(Entity entity)
    {
        entity.Start();

        _entities.Add(entity);
        if (entity is Bullet b) _bullets.Add(b);
        else if (entity is BlackHole bh) _blackHoles.Add(bh);
        else if (entity is Enemy e) _enemies.Add(e);

        var collider = entity.GetComponent<ColliderComponent>();
        if (collider != null)
            _collidables.Add((entity, collider));
    }

    private void HandleCollisions()
    {
        for (int i = 0; i < _collidables.Count; i++)
        {
            var a = _collidables[i];
            if (a.Entity.IsExpired || !a.Collider.IsActive) continue;

            for (int j = i + 1; j < _collidables.Count; j++)
            {
                var b = _collidables[j];
                if (b.Entity.IsExpired || !b.Collider.IsActive) continue;

                if (CollisionRegistry.Intersects(a.Entity, a.Collider, b.Entity, b.Collider))
                {
                    a.Entity.OnCollision(b.Entity);
                    b.Entity.OnCollision(a.Entity);
                }
            }
        }
    }

    public void ForEachNearbyEntity(Vector2 position, float radius, System.Action<Entity> visitor)
    {
        float rSq = radius * radius;
        for (int i = 0; i < _entities.Count; i++)
        {
            if (Vector2.DistanceSquared(position, _entities[i].Position) < rSq)
                visitor(_entities[i]);
        }
    }
}
