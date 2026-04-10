using System;
using System.Collections.Generic;
using GeometryWars.Components.Identity;
using GeometryWars.Components.Lifecycle;
using GMDCore.ECS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Systems;

// Manages the lifecycle, updates, collisions, and rendering of all live entities.
public sealed class EntityWorld : INeighborQuery
{
    private readonly List<Entity> _pendingAdd = [];
    private readonly EntityCatalog _catalog = new();
    private readonly CollisionSystem _collisions = new();

    private bool _isUpdating;

    public event Action<Entity> EntityRemoved;

    public IReadOnlyList<Entity> BlackHoles => _catalog.BlackHoles;
    public int Count => _catalog.Count;
    public int BlackHoleCount => _catalog.BlackHoleCount;

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
        for (int i = 0; i < _catalog.Entities.Count; i++)
            _catalog.Entities[i].Update();
        _isUpdating = false;

        // Collisions run after simulation so overlap tests use current positions.
        _collisions.HandleCollisions();

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
        EntityRemoved?.Invoke(entity);
    }

    public void ForEachNearbyEntity(Vector2 position, float radius, System.Action<Entity> visitor)
        => _catalog.ForEachNearbyEntity(position, radius, visitor);
}

