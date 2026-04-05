using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public static class EnemySpawner
{
    private static readonly Random rand = new();
    private static float _inverseSpawnChance = GameSettings.EnemySpawnChanceStart;

    // getPlayerPosition returns null when the player is dead; no dependency on PlayerShip.
    // A delegate is used (rather than a plain Vector2?) so seekers capture a live reference
    // and get the current position on every frame, not just the position at spawn time.
    public static void Update(Func<Vector2?> getPlayerPosition)
    {
        Vector2? playerPosition = getPlayerPosition();
        if (playerPosition.HasValue && EntityManager.Count < GameSettings.MaxActiveEntities)
        {
            Vector2 playerPos = playerPosition.Value;

            if (rand.Next((int)_inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateSeeker(GetSpawnPosition(playerPos), () => getPlayerPosition().GetValueOrDefault()));

            if (rand.Next((int)_inverseSpawnChance) == 0)
                EntityManager.Add(Enemy.CreateWanderer(GetSpawnPosition(playerPos)));

            if (EntityManager.BlackHoleCount < GameSettings.MaxBlackHoles &&
                rand.Next((int)GameSettings.BlackHoleSpawnChance) == 0)
                EntityManager.Add(new BlackHole(GetSpawnPosition(playerPos)));
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
            pos = new Vector2(rand.Next((int)GameServices.ScreenSize.X), rand.Next((int)GameServices.ScreenSize.Y));
        }
        while (Vector2.DistanceSquared(pos, playerPosition) < minDistSq);
        return pos;
    }

    public static void Reset() => _inverseSpawnChance = GameSettings.EnemySpawnChanceStart;
}
