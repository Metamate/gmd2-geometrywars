using System.Collections.Generic;
using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

/// <summary>
/// Manages the lifecycle, updates, and rendering of all game entities.
/// Handles collision detection and entity pooling.
/// </summary>
static class EntityManager
{
    private static readonly List<Entity> _entities = [];
    private static readonly List<Enemy> _enemies = [];
    private static readonly List<Bullet> _bullets = [];
    private static readonly List<BlackHole> _blackHoles = [];
    private static readonly List<Entity> _pendingAdd = [];
    
    private static readonly List<(Entity Entity, ColliderComponent Collider)> _collidables = [];

    private static readonly ObjectPool<Bullet> _bulletPool = new(() => new Bullet(), 128);
    private static bool _isUpdating;

    private static readonly List<Entity> _nearbyEntitiesBuffer = [];

    public static List<BlackHole> BlackHoles => _blackHoles;
    public static int Count => _entities.Count;
    public static int BlackHoleCount => _blackHoles.Count;

    public static Bullet GetBullet(Vector2 position, Vector2 velocity)
    {
        var bullet = _bulletPool.Get();
        bullet.Reset(position, velocity);
        return bullet;
    }

    public static void Add(Entity entity)
    {
        if (!_isUpdating) RegisterEntity(entity);
        else _pendingAdd.Add(entity);
    }

    public static void Clear()
    {
        _entities.Clear();
        _enemies.Clear();
        _bullets.Clear();
        _blackHoles.Clear();
        _pendingAdd.Clear();
        _collidables.Clear();
    }

    public static void KillAllEnemies()
    {
        for (int i = 0; i < _enemies.Count; i++)
            _enemies[i].Kill();
    }

    public static void KillAllBlackHoles()
    {
        for (int i = 0; i < _blackHoles.Count; i++)
            _blackHoles[i].Kill();
    }

    public static void Update()
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

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in _entities)
            entity.Draw(spriteBatch);
    }

    private static void RegisterEntity(Entity entity)
    {
        _entities.Add(entity);
        if (entity is Bullet b) _bullets.Add(b);
        else if (entity is BlackHole bh) _blackHoles.Add(bh);
        else if (entity is Enemy e) _enemies.Add(e);

        var collider = entity.GetComponent<ColliderComponent>();
        if (collider != null)
        {
            _collidables.Add((entity, collider));
        }
    }

    private static void HandleCollisions()
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

    public static List<Entity> GetNearbyEntities(Vector2 position, float radius)
    {
        _nearbyEntitiesBuffer.Clear();
        float rSq = radius * radius;
        for (int i = 0; i < _entities.Count; i++)
        {
            if (Vector2.DistanceSquared(position, _entities[i].Position) < rSq)
                _nearbyEntitiesBuffer.Add(_entities[i]);
        }
        return _nearbyEntitiesBuffer;
    }
}
