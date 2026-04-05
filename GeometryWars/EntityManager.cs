using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GeometryWars;

static class EntityManager
{
    static List<Entity> entities = [];
    static List<Enemy> enemies = [];
    static List<Bullet> bullets = [];
    static List<BlackHole> blackHoles = [];
    static bool isUpdating;
    static readonly List<Entity> addedEntities = [];
    public static IEnumerable<BlackHole> BlackHoles { get { return blackHoles; } }

    public static int Count { get { return entities.Count; } }
    public static int BlackHoleCount { get { return blackHoles.Count; } }

    public static void Add(Entity entity)
    {
        if (!isUpdating)
            AddEntity(entity);
        else
            addedEntities.Add(entity);
    }

    public static void Update()
    {
        isUpdating = true;
        HandleCollisions();

        foreach (var entity in entities)
            entity.Update();

        isUpdating = false;

        foreach (var entity in addedEntities)
            AddEntity(entity);

        addedEntities.Clear();

        //remove any expired entities
        entities = [.. entities.Where(x => !x.IsExpired)];
        bullets = [.. bullets.Where(x => !x.IsExpired)];
        enemies = [.. enemies.Where(x => !x.IsExpired)];
        blackHoles = [.. blackHoles.Where(x => !x.IsExpired)];
    }

    public static void Draw(SpriteBatch spriteBatch)
    {
        foreach (var entity in entities)
            entity.Draw(spriteBatch);
    }

    private static void AddEntity(Entity entity)
    {
        entities.Add(entity);
        if (entity is Bullet)
            bullets.Add(entity as Bullet);
        else if (entity is BlackHole)
            blackHoles.Add(entity as BlackHole);
        else if (entity is Enemy)
            enemies.Add(entity as Enemy);
    }
    private static bool IsColliding(Entity a, Entity b)
    {
        float radius = a.Radius + b.Radius;
        return !a.IsExpired && !b.IsExpired && Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
    }

    static void HandleCollisions()
    {
        // handle collisions between enemies 
        for (int i = 0; i < enemies.Count; i++)
            for (int j = i + 1; j < enemies.Count; j++)
            {
                if (IsColliding(enemies[i], enemies[j]))
                {
                    enemies[i].HandleCollision(enemies[j]);
                    enemies[j].HandleCollision(enemies[i]);
                }
            }
        // handle collisions between bullets and enemies 
        for (int i = 0; i < enemies.Count; i++)
            for (int j = 0; j < bullets.Count; j++)
            {
                if (IsColliding(enemies[i], bullets[j]))
                {
                    enemies[i].WasShot();
                    bullets[j].IsExpired = true;
                }
            }
        // handle collisions between the player and enemies 
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsActive && IsColliding(PlayerShip.Instance, enemies[i]))
            {
                PlayerShip.Instance.Kill();
                enemies.ForEach(x => x.WasShot());
                EnemySpawner.Reset();
                break;
            }
        }

        // handle collisions with black holes 
        for (int i = 0; i < blackHoles.Count; i++)
        {
            for (int j = 0; j < enemies.Count; j++)
                if (enemies[j].IsActive && IsColliding(blackHoles[i], enemies[j]))
                    enemies[j].WasShot();
            for (int j = 0; j < bullets.Count; j++)
            {
                if (IsColliding(blackHoles[i], bullets[j]))
                {
                    bullets[j].IsExpired = true;
                    blackHoles[i].WasShot();
                }
            }
            if (IsColliding(PlayerShip.Instance, blackHoles[i]))
            {
                KillPlayer();
                break;
            }
        }
    }

    private static void KillPlayer()
    {
        PlayerShip.Instance.Kill();
        enemies.ForEach(x => x.WasShot());
        blackHoles.ForEach(x => x.Kill());
        EnemySpawner.Reset();
    }

    public static IEnumerable<Entity> GetNearbyEntities(Vector2 position, float radius)
    {
        return entities.Where(x => Vector2.DistanceSquared(position, x.Position) < radius * radius);
    }

}