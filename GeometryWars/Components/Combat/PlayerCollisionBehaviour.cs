using GeometryWars.Components.Core;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Entities;
using GeometryWars.Systems;

namespace GeometryWars.Components.Combat;

// Handles collision response for the player ship.
// Follows the same pattern as EnemyCollisionBehaviour and BulletCollisionBehaviour.
public sealed class PlayerCollisionBehaviour : Component, ICollisionComponent
{
    private readonly IEntitySweeper _sweeper;
    private readonly ISpawnController _spawner;
    private PlayerShip _player;
    private PlayerRespawnBehaviour _respawn;

    public PlayerCollisionBehaviour(IEntitySweeper sweeper, ISpawnController spawner)
    {
        _sweeper = sweeper;
        _spawner = spawner;
    }

    public override void OnStart(Entity owner)
    {
        _player  = owner as PlayerShip;
        _respawn = owner.GetComponent<PlayerRespawnBehaviour>();
    }

    public override void Update(Entity owner) { }

    public void OnCollision(Entity owner, Entity other)
    {
        if (_player == null || _player.IsDead) return;

        bool hitByEnemy     = other is Enemy e && e.IsActive;
        bool hitByBlackHole = other is BlackHole;

        if (hitByEnemy || hitByBlackHole)
        {
            _respawn.HandleDeath();
            _sweeper.KillAllEnemies();
            if (hitByBlackHole)
                _sweeper.KillAllBlackHoles();
            _spawner.Reset();
        }
    }
}
