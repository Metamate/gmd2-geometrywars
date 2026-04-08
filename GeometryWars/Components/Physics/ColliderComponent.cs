using GeometryWars.Components.Core;
using GeometryWars.Entities;

namespace GeometryWars.Components.Physics;

/// <summary>
/// Base class for all collision volumes.
/// </summary>
public abstract class ColliderComponent : Component
{
    public override void Update(Entity owner) { }
}
