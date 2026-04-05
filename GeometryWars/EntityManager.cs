using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

// Manages entity lifecycle: add, update, draw, remove.
// Collision detection is delegated to CollisionSystem.
// Bullet instances are pooled via ObjectPool<Bullet> to avoid per-frame allocation.
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

    // Retrieve a pooled bullet, configured with given position and velocity.
    public static Bullet GetBullet(Microsoft.Xna.Framework.Vector2 position, Microsoft.Xna.Framework.Vector2 velocity)
    {
        var bullet = bulletPool.Get();
        bullet.Reset(position, velocity);
        return bullet;
    }

    public static void Add(Entity entity)
    {
        if (!isUpdating)
            RegisterEntity(entity);
        else
            pendingAdd.Add(entity);
    }

    public static void Clear()
    {
        entities.Clear();
        enemies.Clear();
        bullets.Clear();
        blackHoles.Clear();
        pendingAdd.Clear();
    }

    public static void Update(GameContext ctx)
    {
        isUpdating = true;

        CollisionSystem.HandleCollisions(enemies, bullets, blackHoles, PlayerShip.Instance);

        foreach (var entity in entities)
            entity.Update(ctx);

        isUpdating = false;

        foreach (var entity in pendingAdd)
            RegisterEntity(entity);
        pendingAdd.Clear();

        // Return expired bullets to pool before removing from lists
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

    // C# pattern matching replaces the old (entity as Bullet) casts.
    // Adding a new entity type only requires adding a new typed list and a case here.
    private static void RegisterEntity(Entity entity)
    {
        entities.Add(entity);
        if (entity is Bullet b) bullets.Add(b);
        else if (entity is BlackHole bh) blackHoles.Add(bh);
        else if (entity is Enemy e) enemies.Add(e);
    }

    public static IEnumerable<Entity> GetNearbyEntities(Microsoft.Xna.Framework.Vector2 position, float radius)
    {
        return entities.Where(e => Microsoft.Xna.Framework.Vector2.DistanceSquared(position, e.Position) < radius * radius);
    }
}
