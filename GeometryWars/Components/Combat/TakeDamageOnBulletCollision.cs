using GMDCore.ECS.Components;
using GeometryWars.Components.Identity;
using GMDCore.ECS;

namespace GeometryWars.Components.Combat;

// Applies damage to the owner's health whenever it is hit by a bullet.
public sealed class TakeDamageOnBulletCollision : Component
{
    private readonly int _damagePerHit;
    private Health _health;

    public TakeDamageOnBulletCollision(int damagePerHit = 1)
    {
        _damagePerHit = damagePerHit;
    }

    public override void OnStart(Entity owner)
    {
        _health = owner.GetComponent<Health>();
    }

    public override void OnCollision(Entity owner, Entity other)
    {
        if (!other.HasComponent<BulletTag>())
            return;

        _health?.ApplyDamage(_damagePerHit);
    }
}

