using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Manages entity lifecycle: add, update, draw, remove.
//
// Deferred removal: entities set IsExpired = true during Update() but are not
// removed until the end of the frame. This keeps the entity list stable while
// it is being iterated, so any entity can safely expire itself or others.
//
// Deferred add: entities added during an update are queued in _pendingAdd and
// inserted after the current update loop finishes. This prevents newly spawned
// entities from being updated on the same frame they are created.
//
// Collision detection: a generic circle-pair loop (O(n²)) calls OnCollision() on
// both entities when their circles overlap. Response logic lives in each entity's
// OnCollision() override — the manager only detects, never responds.
//
// Object pool: Bullet instances are recycled via an ObjectPool to avoid per-frame
// heap allocations. Use GetBullet() to acquire and Reset() to reinitialise.
public sealed class EntityManager
{
    private readonly List<Entity> _entities  = [];
    private readonly List<Enemy>  _enemies   = [];
    private readonly List<Bullet> _bullets   = [];
    private readonly List<BlackHole> _blackHoles = [];
    private readonly List<Entity> _pendingAdd = [];
    private readonly ObjectPool<Bullet> _bulletPool = new(() => new Bullet(), 128);
    private bool _isUpdating;

    public IEnumerable<BlackHole> BlackHoles => _blackHoles;
    public int Count          => _entities.Count;
    public int BlackHoleCount => _blackHoles.Count;

    public Bullet GetBullet(Vector2 position, Vector2 velocity)
    {
        var bullet = _bulletPool.Get();
        bullet.Reset(position, velocity);
        return bullet;
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
        // Note: _bulletPool is intentionally not cleared — pooled bullets survive
        // between play sessions so the pool does not need to be rebuilt on restart.
    }

    public void KillAllEnemies()   => _enemies.ForEach(e  => e.WasShot(awardPoints: false));
    public void KillAllBlackHoles() => _blackHoles.ForEach(bh => bh.Kill());

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

        _entities.RemoveAll(e  => e.IsExpired);
        _bullets.RemoveAll(b   => b.IsExpired);
        _enemies.RemoveAll(e   => e.IsExpired);
        _blackHoles.RemoveAll(bh => bh.IsExpired);
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in _entities)
            entity.Draw(spriteBatch);
    }

    private void RegisterEntity(Entity entity)
    {
        _entities.Add(entity);
        if      (entity is Bullet b)    _bullets.Add(b);
        else if (entity is BlackHole bh) _blackHoles.Add(bh);
        else if (entity is Enemy e)     _enemies.Add(e);
    }

    // Generic circle-pair collision detection.
    // Calls OnCollision() on both entities for each overlapping pair.
    // O(n²) over all entities — acceptable for this game's scale (<200 entities).
    private void HandleCollisions()
    {
        for (int i = 0; i < _entities.Count; i++)
        {
            var a = _entities[i];
            if (a.IsExpired || !a.Collider.IsActive) continue;

            for (int j = i + 1; j < _entities.Count; j++)
            {
                var b = _entities[j];
                if (b.IsExpired || !b.Collider.IsActive) continue;

                float r = a.Collider.Radius + b.Collider.Radius;
                if (Vector2.DistanceSquared(a.Position, b.Position) < r * r)
                {
                    a.OnCollision(b);
                    b.OnCollision(a);
                }
            }
        }
    }

    public IEnumerable<Entity> GetNearbyEntities(Vector2 position, float radius)
        => _entities.Where(e => Vector2.DistanceSquared(position, e.Position) < radius * radius);
}
