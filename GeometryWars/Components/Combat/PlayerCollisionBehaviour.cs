using GeometryWars.Components.Core;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Entities;
using GeometryWars.Systems;

namespace GeometryWars.Components.Combat;

// Handles collision response for the player ship.
// Follows the same pattern as EnemyCollisionBehaviour and BulletCollisionBehaviour.
public sealed class PlayerCollisionBehaviour : Component
{
    private readonly IEntitySweeper _sweeper;
    private readonly ISpawnController _spawner;
    private RespawnStateComponent _respawnState;

    public PlayerCollisionBehaviour(IEntitySweeper sweeper, ISpawnController spawner)
    {
        _sweeper = sweeper;
        _spawner = spawner;
    }

    public override void OnStart(Entity owner)
    {
        _respawnState = owner.GetComponent<RespawnStateComponent>();
    }

    public override void OnCollision(Entity owner, Entity other)
    {
        if (_respawnState?.IsRespawning == true)
            return;

        bool hitByEnemy     = other is Enemy e && e.IsActive;
        bool hitByBlackHole = other is BlackHole;

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
