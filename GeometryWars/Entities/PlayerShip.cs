using GeometryWars.Components.Physics;
using GeometryWars.Components.Visuals;
using GeometryWars.Components.Combat;
using GeometryWars.Components.Lifecycle;

namespace GeometryWars.Entities;

// Archetype for the player-controlled ship.
public class PlayerShip : Entity
{
    public bool IsDead => GetComponent<PlayerLifeComponent>()?.IsDead ?? false;
}
