using GMDCore.ECS.Components;
using GeometryWars3.Components.Identity;
using GeometryWars3.Components.Lifecycle;
using GMDCore.ECS;

namespace GeometryWars3.Components.Combat;

// Starts the player's respawn flow when it collides with a lethal hazard.
public sealed class BeginRespawnOnLethalCollision : Component
{
    private RespawnState _respawnState;

    public override void OnStart(Entity owner)
    {
        _respawnState = owner.RequireComponent<RespawnState>();
    }

    public override void OnCollision(Entity owner, Entity other)
    {
        if (_respawnState?.IsRespawning == true)
            return;

        bool hitByEnemy = other.IsActive && other.HasComponent<EnemyTag>();
        bool hitByBlackHole = other.HasComponent<BlackHoleTag>();

        if (hitByEnemy || hitByBlackHole)
            _respawnState.BeginRespawn();
    }
}
