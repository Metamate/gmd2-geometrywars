using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars;

public static class EnemySpawner
{
    private static float _inverseSpawnChance = GameSettings.Enemy.Spawning.ChanceStart;

    public static void Update(bool playerActive, Func<Vector2> getPlayerPosition)
    {
        if (!playerActive) return;

        // Random spawn logic based on current difficulty chance
        if (Random.Shared.NextSingle() < 1f / _inverseSpawnChance)
        {
            var spawnPos = GetRandomSpawnPosition(getPlayerPosition());
            
            // 20% chance for a Seeker, 80% for a Wanderer
            if (Random.Shared.Next(5) == 0)
                EntityManager.Add(Enemy.CreateSeeker(spawnPos, getPlayerPosition));
            else
                EntityManager.Add(Enemy.CreateWanderer(spawnPos));

            GameServices.Audio.Play(Sound.Spawn, 0.2f);
        }

        // Difficulty scaling
        if (_inverseSpawnChance > GameSettings.Enemy.Spawning.ChanceMin)
            _inverseSpawnChance -= GameSettings.Enemy.Spawning.ChanceDecay;

        // Occasional black hole spawn
        if (EntityManager.BlackHoleCount < GameSettings.Hazards.MaxBlackHoles && Random.Shared.NextSingle() < 1f / GameSettings.Hazards.BlackHoleSpawnChance)
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
