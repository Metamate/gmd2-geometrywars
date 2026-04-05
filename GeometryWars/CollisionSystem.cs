using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace GeometryWars;

// Owns all collision detection and response logic, extracted from EntityManager.
// EntityManager delegates to this each frame after updates.
public static class CollisionSystem
{
    public static void HandleCollisions(
        List<Enemy> enemies,
        List<Bullet> bullets,
        List<BlackHole> blackHoles,
        PlayerShip player)
    {
        HandleEnemyEnemyCollisions(enemies);
        HandleBulletEnemyCollisions(bullets, enemies);
        HandlePlayerEnemyCollisions(player, enemies);
        HandleBlackHoleCollisions(blackHoles, enemies, bullets, player);
    }

    private static bool IsColliding(Entity a, Entity b)
    {
        float radius = a.Radius + b.Radius;
        return !a.IsExpired && !b.IsExpired &&
               Vector2.DistanceSquared(a.Position, b.Position) < radius * radius;
    }

    private static void HandleEnemyEnemyCollisions(List<Enemy> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
            for (int j = i + 1; j < enemies.Count; j++)
            {
                if (IsColliding(enemies[i], enemies[j]))
                {
                    enemies[i].HandleCollision(enemies[j]);
                    enemies[j].HandleCollision(enemies[i]);
                }
            }
    }

    private static void HandleBulletEnemyCollisions(List<Bullet> bullets, List<Enemy> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
            for (int j = 0; j < bullets.Count; j++)
            {
                if (IsColliding(enemies[i], bullets[j]))
                {
                    enemies[i].WasShot();
                    bullets[j].IsExpired = true;
                }
            }
    }

    private static void HandlePlayerEnemyCollisions(PlayerShip player, List<Enemy> enemies)
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].IsActive && IsColliding(player, enemies[i]))
            {
                player.Kill();
                enemies.ForEach(e => e.WasShot());
                EnemySpawner.Reset();
                break;
            }
        }
    }

    private static void HandleBlackHoleCollisions(
        List<BlackHole> blackHoles,
        List<Enemy> enemies,
        List<Bullet> bullets,
        PlayerShip player)
    {
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

            if (IsColliding(player, blackHoles[i]))
            {
                player.Kill();
                enemies.ForEach(e => e.WasShot());
                blackHoles.ForEach(bh => bh.Kill());
                EnemySpawner.Reset();
                break;
            }
        }
    }
}
