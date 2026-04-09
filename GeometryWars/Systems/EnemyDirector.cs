using System;
using GeometryWars.Entities;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Systems;

// Handles the timing and positioning of enemy spawns.
public sealed class EnemyDirector : ISpawnController
{
    private readonly EntityWorld _world;
    private readonly EntityFactory _factory;
    private readonly GameRuntime _runtime;
    private float _inverseSpawnChance = GameSettings.Enemy.Spawning.ChanceStart;

    public EnemyDirector(EntityWorld world, EntityFactory factory, GameRuntime runtime)
    {
        _world = world;
        _factory = factory;
        _runtime = runtime;
    }

    public void Update(bool playerActive, Func<Vector2> getPlayerPosition)
    {
        // 1. Difficulty Scaling
        if (_inverseSpawnChance > GameSettings.Enemy.Spawning.ChanceMin)
            _inverseSpawnChance -= GameSettings.Enemy.Spawning.ChanceDecay;

        // 2. Early Gates
        if (!playerActive) return;
        if (_world.Count >= GameSettings.Performance.MaxEntities) return;

        // 3. Spawning Logic
        UpdateEnemySpawns(getPlayerPosition);
        UpdateBlackHoleSpawns(getPlayerPosition);
    }

    private void UpdateEnemySpawns(Func<Vector2> getPlayerPosition)
    {
        if (Random.Shared.NextSingle() < 1f / _inverseSpawnChance)
        {
            var spawnPos = GetRandomSpawnPosition(getPlayerPosition());
            _world.Add(_factory.CreateWanderer(spawnPos));
            _runtime.Audio.Play(_runtime.Assets.Spawn, 0.15f);
        }

        if (Random.Shared.NextSingle() < 1f / (_inverseSpawnChance * 2f))
        {
            var spawnPos = GetRandomSpawnPosition(getPlayerPosition());
            _world.Add(_factory.CreateSeeker(spawnPos, getPlayerPosition));
            _runtime.Audio.Play(_runtime.Assets.Spawn, 0.2f);
        }
    }

    private void UpdateBlackHoleSpawns(Func<Vector2> getPlayerPosition)
    {
        if (_world.BlackHoleCount < GameSettings.Hazards.MaxBlackHoles &&
            Random.Shared.NextSingle() < 1f / GameSettings.Hazards.BlackHoleSpawnChance)
        {
            _world.Add(_factory.CreateBlackHole(GetRandomSpawnPosition(getPlayerPosition())));
            _runtime.Audio.Play(_runtime.Assets.Spawn, 0.3f, -0.2f);
        }
    }

    private Vector2 GetRandomSpawnPosition(Vector2 playerPosition)
    {
        Vector2 pos;
        float minDistSq = GameSettings.Enemy.Spawning.MinDistance * GameSettings.Enemy.Spawning.MinDistance;
        
        do
        {
            pos = new Vector2(
                Random.Shared.Next((int)_runtime.Frame.ScreenSize.X),
                Random.Shared.Next((int)_runtime.Frame.ScreenSize.Y));
        }
        while (Vector2.DistanceSquared(pos, playerPosition) < minDistSq);
        
        return pos;
    }

    public void Reset() => _inverseSpawnChance = GameSettings.Enemy.Spawning.ChanceStart;
}
