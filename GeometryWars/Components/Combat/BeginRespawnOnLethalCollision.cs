using GeometryWars.Components.Core;
using GeometryWars.Components.Identity;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Entities;
using GeometryWars.Systems;

namespace GeometryWars.Components.Combat;

// Starts the player's respawn flow when it collides with a lethal hazard.
public sealed class BeginRespawnOnLethalCollision : Component
{
    private readonly IEntitySweeper _sweeper;
    private readonly ISpawnController _spawner;
    private RespawnState _respawnState;

    public BeginRespawnOnLethalCollision(IEntitySweeper sweeper, ISpawnController spawner)
    {
        _sweeper = sweeper;
        _spawner = spawner;
    }

    public override void OnStart(Entity owner)
    {
        _respawnState = owner.GetComponent<RespawnState>();
    }

    public override void OnCollision(Entity owner, Entity other)
    {
        if (_respawnState?.IsRespawning == true)
            return;

        bool hitByEnemy = other.IsActive && other.HasComponent<EnemyTag>();
        bool hitByBlackHole = other.HasComponent<BlackHoleTag>();

        if (hitByEnemy || hitByBlackHole)
        {
            _respawnState.BeginRespawn();
            _sweeper.KillAllEnemies();
            if (hitByBlackHole)
                _sweeper.KillAllBlackHoles();
            _spawner.Reset();
        }
    }
}
