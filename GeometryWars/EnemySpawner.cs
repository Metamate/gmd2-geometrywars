using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public static class EnemySpawner
{
    private static Random rand = new();
    private static float inverseSpawnChance = 60;
    private static readonly float inverseBlackHoleChance = 600;

    // playerPosition is null when the player is dead; no dependency on PlayerShip.
    public static void Update(Vector2? playerPosition)
    {
        if (playerPosition.HasValue && EntityManager.Count < 200)
        {
            Vector2 playerPos = playerPosition.Value;

            if (rand.Next((int)inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition(playerPos), () => playerPosition.Value));

            if (rand.Next((int)inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition(playerPos)));

            if (EntityManager.BlackHoleCount < 2 && rand.Next((int)inverseBlackHoleChance) == 0)
                EntityManager.Add(new BlackHole(GetSpawnPosition(playerPos)));
        }

        if (inverseSpawnChance > 20)
            inverseSpawnChance -= 0.005f;
    }

    private static Vector2 GetSpawnPosition(Vector2 playerPosition)
    {
        Vector2 pos;
        do
        {
            pos = new Vector2(rand.Next((int)GameServices.ScreenSize.X), rand.Next((int)GameServices.ScreenSize.Y));
        }
        while (Vector2.DistanceSquared(pos, playerPosition) < 250 * 250);
        return pos;
    }

    public static void Reset() => inverseSpawnChance = 60;
}
