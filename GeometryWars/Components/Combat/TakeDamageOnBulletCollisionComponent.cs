using GeometryWars.Components.Core;
using GeometryWars.Entities;

namespace GeometryWars.Components.Combat;

// Applies damage to the owner's health whenever it is hit by a bullet.
public sealed class TakeDamageOnBulletCollisionComponent : Component
{
    private readonly int _damagePerHit;
    private HealthComponent _health;

    public TakeDamageOnBulletCollisionComponent(int damagePerHit = 1)
    {
        _damagePerHit = damagePerHit;
    }

    public override void OnStart(Entity owner)
    {
        _health = owner.GetComponent<HealthComponent>();
    }

    public override void OnCollision(Entity owner, Entity other)
    {
        if (other is not Bullet)
            return;

        _health?.ApplyDamage(_damagePerHit);
    }
}
