using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Systems;

namespace GeometryWars.Components.Combat;

// Handles collision response for enemies, including death effects and score.
public sealed class EnemyCollisionBehaviour : Component, ICollisionComponent
{
    private RigidbodyComponent _rigidbody;
    private TransformComponent _transform;

    public override void OnStart(Entity owner)
    {
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
        _transform = owner.Transform;
    }

    public override void Update(Entity owner) { }

    public void OnCollision(Entity owner, Entity other)
    {
        if (owner is not Enemy enemy) return;

        if (other is Bullet || (other is BlackHole bh && bh.IsActive))
        {
            WasShot(enemy);
        }
        else if (other is Enemy e)
        {
            var otherTransform = e.Transform;
            var otherRigidbody = e.GetComponent<RigidbodyComponent>();
            if (otherTransform == null || otherRigidbody == null) return;

            var d = _transform.Position - otherTransform.Position;
            _rigidbody.AddForce(10 * d / (d.LengthSquared() + 1));
        }
    }

    private static void WasShot(Enemy enemy)
    {
        // Award score before killing (Kill() marks entity as expired).
        PlayerStatus.AddPoints(enemy.PointValue);
        PlayerStatus.IncreaseMultiplier();

        // Visual / audio effects live in Enemy.Kill() — no duplication needed.
        enemy.Kill();
    }
}
