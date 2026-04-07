using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public static class EnemySpawner
{
    private static float _inverseSpawnChance = GameSettings.Enemy.Spawning.ChanceStart;

    // isPlayerAlive guards spawning so enemies stop appearing during the death sequence.
    // getPlayerPosition is a delegate passed to seekers so they track the live player
    // position each frame without needing a direct reference to PlayerShip.
    public static void Update(bool isPlayerAlive, Func<Vector2> getPlayerPosition)
    {
        if (isPlayerAlive && EntityManager.Count < GameSettings.Performance.MaxEntities)
        {
            Vector2 playerPos = getPlayerPosition();

            if (Random.Shared.Next((int)_inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition(playerPos), getPlayerPosition));

            if (Random.Shared.Next((int)_inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition(playerPos)));

            if (EntityManager.BlackHoleCount < GameSettings.Hazards.MaxBlackHoles &&
                Random.Shared.Next((int)GameSettings.Hazards.BlackHoleSpawnChance) == 0)
                EntityManager.Add(new BlackHole(GetSpawnPosition(playerPos)));
        }

        if (_inverseSpawnChance > GameSettings.Enemy.Spawning.ChanceMin)
            _inverseSpawnChance -= GameSettings.Enemy.Spawning.ChanceDecay;
    }

    private static Vector2 GetSpawnPosition(Vector2 playerPosition)
    {
        float minDistSq = GameSettings.Enemy.Spawning.MinDistance * GameSettings.Enemy.Spawning.MinDistance;
        Vector2 pos;
        do
        {
            pos = new Vector2(Random.Shared.Next((int)FrameContext.ScreenSize.X), Random.Shared.Next((int)FrameContext.ScreenSize.Y));
        }
        while (Vector2.DistanceSquared(pos, playerPosition) < minDistSq);
        return pos;
    }

    public static void Reset() => _inverseSpawnChance = GameSettings.Enemy.Spawning.ChanceStart;
}
