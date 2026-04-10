using GMDCore.ECS.Components;
using GeometryWars.Components.Identity;
using GeometryWars.Components.Lifecycle;
using GMDCore.Physics;
using GMDCore.ECS;

namespace GeometryWars.Components.Combat;

// Destroys the owner when it collides with a bullet or active black hole.
public sealed class DestroyOnBulletOrBlackHoleCollision : Component
{
    private Destroyable _destroyable;

    public override void OnStart(Entity owner)
    {
        _destroyable = owner.RequireComponent<Destroyable>();
    }

    public override void OnCollision(Entity owner, Entity other)
    {
        if (other.HasComponent<BulletTag>() || (other.HasComponent<BlackHoleTag>() && other.IsActive))
            _destroyable?.Destroy();
    }
}

