using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Manages entity lifecycle: add, update, draw, remove.
// Collision detection: a generic circle-pair loop fires each entity's CircleCollider.OnCollide
// callback. Collision RULES live in the entity constructors, not here.
static class EntityManager
{
    private static readonly List<Entity> entities = [];
    private static readonly List<Enemy> enemies = [];
    private static readonly List<Bullet> bullets = [];
    private static readonly List<BlackHole> blackHoles = [];
    private static readonly List<Entity> pendingAdd = [];
    private static readonly ObjectPool<Bullet> bulletPool = new(() => new Bullet(), 128);
    private static bool isUpdating;

    public static IEnumerable<BlackHole> BlackHoles => blackHoles;
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

    public static void KillAllEnemies() => enemies.ForEach(e => e.WasShot());
    public static void KillAllBlackHoles() => blackHoles.ForEach(bh => bh.Kill());

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

    // Generic circle-pair collision detection. Fires both entities' OnCollide callbacks.
    // O(n²) over all entities — fine for this game's scale (<200 entities).
    private static void HandleCollisions()
    {
        for (int i = 0; i < entities.Count; i++)
        {
            var a = entities[i];
            if (a.IsExpired || a.Collider == null) continue;

            for (int j = i + 1; j < entities.Count; j++)
            {
                var b = entities[j];
                if (b.IsExpired || b.Collider == null) continue;

                float r = a.Collider.Radius + b.Collider.Radius;
                if (Vector2.DistanceSquared(a.Position, b.Position) < r * r)
                {
                    a.Collider.OnCollide?.Invoke(b);
                    b.Collider.OnCollide?.Invoke(a);
                }
            }
        }
    }

    public static IEnumerable<Entity> GetNearbyEntities(Vector2 position, float radius)
        => entities.Where(e => Vector2.DistanceSquared(position, e.Position) < radius * radius);
}
