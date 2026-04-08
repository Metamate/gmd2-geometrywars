using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

/// <summary>
/// Component that pulls nearby entities towards its owner.
/// Demonstrates how "spatial logic" can be moved out of a concrete entity.
/// </summary>
public sealed class GravityBehaviour : IComponent
{
    private readonly float _range;
    private readonly float _force;

    public GravityBehaviour(float range, float force)
    {
        _range = range;
        _force = force;
    }

    public void Update(Entity owner)
    {
        // Pull nearby entities (excluding the owner)
        foreach (var entity in EntityManager.GetNearbyEntities(owner.Position, _range))
        {
            if (entity == owner) continue;

            if (entity is Enemy enemy && !enemy.IsActive)
                continue;

            if (entity is Bullet)
            {
                // Bullets are repelled from gravity wells
                entity.Velocity += (entity.Position - owner.Position).ScaleTo(0.3f);
            }
            else
            {
                // Everything else is attracted
                var dPos = owner.Position - entity.Position;
                entity.Velocity += dPos.ScaleTo(MathHelper.Lerp(_force, 0, dPos.Length() / _range));
            }
        }
    }
}
