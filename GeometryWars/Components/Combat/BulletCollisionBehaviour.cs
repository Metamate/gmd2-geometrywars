using GeometryWars.Components.Core;
using GeometryWars.Entities;

namespace GeometryWars.Components.Combat;

// Handles collision logic for bullets.
public sealed class BulletCollisionBehaviour : Component
{
    public override void OnCollision(Entity owner, Entity other)
    {
        if (other is Enemy || other is BlackHole)
        {
            owner.IsExpired = true;
        }
    }
}
