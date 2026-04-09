using GeometryWars.Components.Core;
using GeometryWars.Components.Identity;
using GeometryWars.Entities;

namespace GeometryWars.Components.Combat;

// Expires the owner when it hits an enemy or black hole.
public sealed class ExpireOnEnemyOrBlackHoleCollision : Component
{
    public override void OnCollision(Entity owner, Entity other)
    {
        if (other.HasComponent<EnemyTag>() || other.HasComponent<BlackHoleTag>())
        {
            owner.IsExpired = true;
        }
    }
}
