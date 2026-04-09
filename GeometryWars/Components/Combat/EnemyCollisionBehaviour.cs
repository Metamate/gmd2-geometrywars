using GeometryWars.Components.Core;
using GeometryWars.Components.Identity;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;

namespace GeometryWars.Components.Combat;

// Handles collision response for enemies.
public sealed class EnemyCollisionBehaviour : Component
{
    private DestroyableComponent _destroyable;
    private RigidbodyComponent _rigidbody;
    private TransformComponent _transform;

    public override void OnStart(Entity owner)
    {
        _destroyable = owner.GetComponent<DestroyableComponent>();
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
        _transform = owner.Transform;
    }

    public override void OnCollision(Entity owner, Entity other)
    {
        if (other.HasComponent<BulletTagComponent>() || (other.HasComponent<BlackHoleTagComponent>() && other.IsActive))
        {
            _destroyable?.Destroy();
        }
        else if (other.HasComponent<EnemyTagComponent>())
        {
            var otherTransform = other.Transform;
            var otherRigidbody = other.GetComponent<RigidbodyComponent>();
            if (otherTransform == null || otherRigidbody == null) return;

            var d = _transform.Position - otherTransform.Position;
            _rigidbody.AddForce(10 * d / (d.LengthSquared() + 1));
        }
    }
}
