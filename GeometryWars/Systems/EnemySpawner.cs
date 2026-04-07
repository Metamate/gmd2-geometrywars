using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public static class EnemySpawner
{
    private static float _inverseSpawnChance = GameSettings.EnemySpawnChanceStart;

    // isPlayerAlive guards spawning so enemies stop appearing during the death sequence.
    // getPlayerPosition is a delegate passed to seekers so they track the live player
    // position each frame without needing a direct reference to PlayerShip.
    public static void Update(bool isPlayerAlive, Func<Vector2> getPlayerPosition)
    {
        if (isPlayerAlive && GameServices.Entities.Count < GameSettings.MaxActiveEntities)
        {
            Vector2 playerPos = getPlayerPosition();

            if (Random.Shared.Next((int)_inverseSpawnChance) == 0)
                GameServices.Entities.Add(Enemy.CreateSeeker(GetSpawnPosition(playerPos), getPlayerPosition));

            if (Random.Shared.Next((int)_inverseSpawnChance) == 0)
                GameServices.Entities.Add(Enemy.CreateWanderer(GetSpawnPosition(playerPos)));

            if (GameServices.Entities.BlackHoleCount < GameSettings.MaxBlackHoles &&
                Random.Shared.Next((int)GameSettings.BlackHoleSpawnChance) == 0)
                GameServices.Entities.Add(new BlackHole(GetSpawnPosition(playerPos)));
        }

        if (_inverseSpawnChance > GameSettings.EnemySpawnChanceMin)
            _inverseSpawnChance -= GameSettings.EnemySpawnChanceDecay;
    }

    private static Vector2 GetSpawnPosition(Vector2 playerPosition)
    {
        float minDistSq = GameSettings.SpawnMinPlayerDistance * GameSettings.SpawnMinPlayerDistance;
        Vector2 pos;
        do
        {
            pos = new Vector2(Random.Shared.Next((int)GameServices.ScreenSize.X), Random.Shared.Next((int)GameServices.ScreenSize.Y));
        }
        while (Vector2.DistanceSquared(pos, playerPosition) < minDistSq);
        return pos;
    }

    public static void Reset() => _inverseSpawnChance = GameSettings.EnemySpawnChanceStart;
}
