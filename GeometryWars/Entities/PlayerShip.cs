using GeometryWars.Components.Lifecycle;

namespace GeometryWars.Entities;

// Archetype for the player-controlled ship.
public sealed class PlayerShip : Entity
{
    public bool IsDead => GetComponent<RespawnStateComponent>()?.IsRespawning ?? false;
}
