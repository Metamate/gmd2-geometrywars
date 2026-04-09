using GeometryWars.Components.Core;
using GeometryWars.Components.Identity;
using GeometryWars.Entities;

namespace GeometryWars.Components.Combat;

// Handles collision logic for bullets.
public sealed class BulletCollisionBehaviour : Component
{
    public override void OnCollision(Entity owner, Entity other)
    {
        if (other.HasComponent<EnemyTagComponent>() || other.HasComponent<BlackHoleTagComponent>())
        {
            owner.IsExpired = true;
        }
    }
}
