using GMDCore.ECS.Components;
using GeometryWars.Components.Identity;
using GMDCore.Physics;
using GMDCore.ECS;

namespace GeometryWars.Components.Combat;

// Pushes the owner away from overlapping enemies to avoid clumping.
public sealed class RepelFromEnemies : Component
{
    private Rigidbody _rigidbody;
    private Transform _transform;

    public override void OnStart(Entity owner)
    {
        _rigidbody = owner.RequireComponent<Rigidbody>();
        _transform = owner.Transform;
    }

    public override void OnCollision(Entity owner, Entity other)
    {
        if (!other.HasComponent<EnemyTag>())
            return;

        var otherRigidbody = other.GetComponent<Rigidbody>();
        if (otherRigidbody == null)
            return;

        var d = _transform.Position - other.Transform.Position;
        _rigidbody.AddForce(10 * d / (d.LengthSquared() + 1));
    }
}

