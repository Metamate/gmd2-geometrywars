using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public static class EnemySpawner
{
    private static float _inverseSpawnChance = GameSettings.Enemy.Spawning.ChanceStart;

    public static void Update(bool playerActive, Func<Vector2> getPlayerPosition)
    {
        if (_inverseSpawnChance > GameSettings.Enemy.Spawning.ChanceMin)
            _inverseSpawnChance -= GameSettings.Enemy.Spawning.ChanceDecay;


        if (!playerActive) return;
        if (EntityManager.Count >= GameSettings.Performance.MaxEntities) return;

        SpawnEnemies(getPlayerPosition);
        SpawnBlackHoles(getPlayerPosition);
    }

    private static void SpawnEnemies(Func<Vector2> getPlayerPosition)
    {
        if (Random.Shared.NextSingle() < 1f / _inverseSpawnChance)
        {
            var spawnPos = GetRandomSpawnPosition(getPlayerPosition());
            EntityManager.Add(Enemy.CreateWanderer(spawnPos));
            GameServices.Audio.Play(Sound.Spawn, 0.15f);
        }

        if (Random.Shared.NextSingle() < 1f / (_inverseSpawnChance * 2f))
        {
            var spawnPos = GetRandomSpawnPosition(getPlayerPosition());
            EntityManager.Add(Enemy.CreateSeeker(spawnPos, getPlayerPosition));
            GameServices.Audio.Play(Sound.Spawn, 0.2f);
        }
    }

    private static void SpawnBlackHoles(Func<Vector2> getPlayerPosition)
    {
        if (EntityManager.BlackHoleCount < GameSettings.Hazards.MaxBlackHoles && 
            Random.Shared.NextSingle() < 1f / GameSettings.Hazards.BlackHoleSpawnChance)
        {
            EntityManager.Add(new BlackHole(GetRandomSpawnPosition(getPlayerPosition())));
            GameServices.Audio.Play(Sound.Spawn, 0.3f, -0.2f);
        }
    }

    private static Vector2 GetRandomSpawnPosition(Vector2 playerPosition)
    {
        Vector2 pos;
        float minDistSq = GameSettings.Enemy.Spawning.MinDistance * GameSettings.Enemy.Spawning.MinDistance;
        
        do
        {
            pos = new Vector2(
                Random.Shared.Next((int)FrameContext.ScreenSize.X),
                Random.Shared.Next((int)FrameContext.ScreenSize.Y));
        }
        while (Vector2.DistanceSquared(pos, playerPosition) < minDistSq);
        
        return pos;
    }

    public static void Reset() => _inverseSpawnChance = GameSettings.Enemy.Spawning.ChanceStart;
}
