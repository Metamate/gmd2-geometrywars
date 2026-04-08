using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Manages entity lifecycle: add, update, draw, remove.
static class EntityManager
{
    private static readonly List<Entity> entities = [];
    private static readonly List<Enemy> enemies = [];
    private static readonly List<Bullet> bullets = [];
    private static readonly List<BlackHole> blackHoles = [];
    private static readonly List<Entity> pendingAdd = [];
    
    // Fast path for collision detection: 
    // We only keep track of entities that have a collider component.
    private static readonly List<(Entity Entity, CircleColliderComponent Collider)> collidables = [];

    private static readonly ObjectPool<Bullet> bulletPool = new(() => new Bullet(), 128);
    private static bool isUpdating;

    private static readonly List<Entity> nearbyEntitiesBuffer = [];

    public static List<BlackHole> BlackHoles => blackHoles;
    public static int Count => entities.Count;
    public static int BlackHoleCount => blackHoles.Count;

    public static Bullet GetBullet(Vector2 position, Vector2 velocity)
    {
        var bullet = bulletPool.Get();
        bullet.Reset(position, velocity);
        return bullet;
    }

    public static void Add(Entity entity)
    {
        if (!isUpdating) RegisterEntity(entity);
        else pendingAdd.Add(entity);
    }

    public static void Clear()
    {
        entities.Clear();
        enemies.Clear();
        bullets.Clear();
        blackHoles.Clear();
        pendingAdd.Clear();
        collidables.Clear();
    }

    public static void KillAllEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
            enemies[i].Kill();
    }

    public static void KillAllBlackHoles()
    {
        for (int i = 0; i < blackHoles.Count; i++)
            blackHoles[i].Kill();
    }

    public static void Update()
    {
        isUpdating = true;
        HandleCollisions();
        foreach (var entity in entities)
            entity.Update();
        isUpdating = false;

        foreach (var entity in pendingAdd)
            RegisterEntity(entity);
        pendingAdd.Clear();

        foreach (var b in bullets)
            if (b.IsExpired) bulletPool.Return(b);

        entities.RemoveAll(e => e.IsExpired);
        bullets.RemoveAll(b => b.IsExpired);
        enemies.RemoveAll(e => e.IsExpired);
        blackHoles.RemoveAll(bh => bh.IsExpired);
        collidables.RemoveAll(c => c.Entity.IsExpired);
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in entities)
            entity.Draw(spriteBatch);
    }

    private static void RegisterEntity(Entity entity)
    {
        entities.Add(entity);
        if (entity is Bullet b) bullets.Add(b);
        else if (entity is BlackHole bh) blackHoles.Add(bh);
        else if (entity is Enemy e) enemies.Add(e);

        // SYSTEM QUERY: Check if the new entity has a Collider component.
        // If it does, we register it for the collision system.
        var collider = entity.GetComponent<CircleColliderComponent>();
        if (collider != null)
        {
            collidables.Add((entity, collider));
        }
    }

    private static void HandleCollisions()
    {
        // Use our specialized 'collidables' list for O(n²) checks
        for (int i = 0; i < collidables.Count; i++)
        {
            var a = collidables[i];
            if (a.Entity.IsExpired || !a.Collider.IsActive) continue;

            for (int j = i + 1; j < collidables.Count; j++)
            {
                var b = collidables[j];
                if (b.Entity.IsExpired || !b.Collider.IsActive) continue;

                float r = a.Collider.Radius + b.Collider.Radius;
                if (Vector2.DistanceSquared(a.Entity.Position, b.Entity.Position) < r * r)
                {
                    a.Entity.OnCollision(b.Entity);
                    b.Entity.OnCollision(a.Entity);
                }
            }
        }
    }

    public static List<Entity> GetNearbyEntities(Vector2 position, float radius)
    {
        nearbyEntitiesBuffer.Clear();
        float rSq = radius * radius;
        for (int i = 0; i < entities.Count; i++)
        {
            if (Vector2.DistanceSquared(position, entities[i].Position) < rSq)
                nearbyEntitiesBuffer.Add(entities[i]);
        }
        return nearbyEntitiesBuffer;
    }
}
