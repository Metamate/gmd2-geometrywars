using System;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public static class EnemySpawner
{
    private static Random rand = new();
    private static float inverseSpawnChance = 60;
    private static float inverseBlackHoleChance = 600;

    public static void Update(GameContext ctx)
    {
        if (!PlayerShip.Instance.IsDead && EntityManager.Count < 200)
        {
            if (rand.Next((int)inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition(ctx)));

            if (rand.Next((int)inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition(ctx)));

            if (EntityManager.BlackHoleCount < 2 && rand.Next((int)inverseBlackHoleChance) == 0)
                EntityManager.Add(new BlackHole(GetSpawnPosition(ctx)));
        }

        if (inverseSpawnChance > 20)
            inverseSpawnChance -= 0.005f;
    }

    private static Vector2 GetSpawnPosition(GameContext ctx)
    {
        Vector2 pos;
        do
        {
            pos = new Vector2(rand.Next((int)ctx.ScreenSize.X), rand.Next((int)ctx.ScreenSize.Y));
        }
        while (Vector2.DistanceSquared(pos, PlayerShip.Instance.Position) < 250 * 250);
        return pos;
    }

    public static void Reset()
    {
        inverseSpawnChance = 60;
    }
}
