using System;
using GMDCore.ECS;
using GeometryWars2.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars2.Systems;

// Handles the timing and positioning of enemy spawns.
public sealed class EnemyDirector
{
    private readonly EntityWorld _world;
    private readonly EntityFactory _factory;
    private readonly PlayContext _context;
    private float _inverseSpawnChance = GameSettings.Enemy.Spawning.ChanceStart;

    public EnemyDirector(EntityWorld world, EntityFactory factory, PlayContext context)
    {
        _world = world;
        _factory = factory;
        _context = context;
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
    }

    private void UpdateEnemySpawns(Func<Vector2> getPlayerPosition)
    {
        if (Random.Shared.NextSingle() < 1f / _inverseSpawnChance)
        {
            var spawnPos = GetRandomSpawnPosition(getPlayerPosition());
            _world.Add(_factory.CreateWanderer(spawnPos));
        }

        if (Random.Shared.NextSingle() < 1f / (_inverseSpawnChance * 2f))
        {
            var spawnPos = GetRandomSpawnPosition(getPlayerPosition());
            _world.Add(_factory.CreateSeeker(spawnPos, getPlayerPosition));
        }
    }

    private Vector2 GetRandomSpawnPosition(Vector2 playerPosition)
    {
        Vector2 pos;
        float minDistSq = GameSettings.Enemy.Spawning.MinDistance * GameSettings.Enemy.Spawning.MinDistance;

        do
        {
            pos = new Vector2(
                Random.Shared.Next((int)_context.Frame.ScreenSize.X),
                Random.Shared.Next((int)_context.Frame.ScreenSize.Y));
        }
        while (Vector2.DistanceSquared(pos, playerPosition) < minDistSq);

        return pos;
    }

    public void Reset() => _inverseSpawnChance = GameSettings.Enemy.Spawning.ChanceStart;
}
