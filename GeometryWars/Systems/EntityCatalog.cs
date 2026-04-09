using System;
using System.Collections.Generic;
using GeometryWars.Components.Identity;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Systems;

// Owns entity storage plus the lightweight indexes used by gameplay systems.
internal sealed class EntityCatalog
{
    private readonly List<Entity> _entities = [];
    private readonly List<Entity> _enemies = [];
    private readonly List<Entity> _bullets = [];
    private readonly List<Entity> _blackHoles = [];

    public IReadOnlyList<Entity> Entities => _entities;
    public IReadOnlyList<Entity> Enemies => _enemies;
    public IReadOnlyList<Entity> Bullets => _bullets;
    public IReadOnlyList<Entity> BlackHoles => _blackHoles;

    public int Count => _entities.Count;
    public int BlackHoleCount => _blackHoles.Count;

    public void Add(Entity entity)
    {
        _entities.Add(entity);

        if (entity.HasComponent<BulletTagComponent>()) _bullets.Add(entity);
        else if (entity.HasComponent<BlackHoleTagComponent>()) _blackHoles.Add(entity);
        else if (entity.HasComponent<EnemyTagComponent>()) _enemies.Add(entity);
    }

    public void Clear()
    {
        _entities.Clear();
        _enemies.Clear();
        _bullets.Clear();
        _blackHoles.Clear();
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        for (int i = 0; i < _entities.Count; i++)
            _entities[i].Draw(spriteBatch);
    }

    public void ForEachNearbyEntity(Vector2 position, float radius, Action<Entity> visitor)
    {
        float rSq = radius * radius;
        for (int i = 0; i < _entities.Count; i++)
        {
            if (Vector2.DistanceSquared(position, _entities[i].Position) < rSq)
                visitor(_entities[i]);
        }
    }

    public void RemoveExpired(Action<Entity> onBulletExpired)
    {
        for (int i = 0; i < _bullets.Count; i++)
        {
            if (_bullets[i].IsExpired)
                onBulletExpired(_bullets[i]);
        }

        _entities.RemoveAll(entity => entity.IsExpired);
        _bullets.RemoveAll(bullet => bullet.IsExpired);
        _enemies.RemoveAll(enemy => enemy.IsExpired);
        _blackHoles.RemoveAll(blackHole => blackHole.IsExpired);
    }
}
