using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Manages entity lifecycle: add, update, draw, remove.
//
// Optimized for zero-allocation in hot paths (GetNearbyEntities, Collisions).
static class EntityManager
{
    private static readonly List<Entity> entities = [];
    private static readonly List<Enemy> enemies = [];
    private static readonly List<Bullet> bullets = [];
    private static readonly List<BlackHole> blackHoles = [];
    private static readonly List<Entity> pendingAdd = [];
    private static readonly ObjectPool<Bullet> bulletPool = new(() => new Bullet(), 128);
    private static bool isUpdating;

    // Pre-allocated list for proximity queries to avoid per-query heap allocations.
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
    }

    public static void KillAllEnemies()
    {
        for (int i = 0; i < enemies.Count; i++)
            enemies[i].WasShot(awardPoints: false);
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
    }

    private static void HandleCollisions()
    {
        for (int i = 0; i < entities.Count; i++)
        {
            var a = entities[i];
            if (a.IsExpired || !a.Collider.IsActive) continue;

            for (int j = i + 1; j < entities.Count; j++)
            {
                var b = entities[j];
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

    /// <summary>
    /// Returns entities within a radius using a shared reusable buffer. 
    /// 
    /// ARCHITECTURAL NOTE (Temporal Coupling): 
    /// This method is optimized for zero-allocation. The returned list is 
    /// shared across all callers. You MUST process the results immediately; 
    /// do not store the list, as it will be cleared by the next caller!
    /// </summary>
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
