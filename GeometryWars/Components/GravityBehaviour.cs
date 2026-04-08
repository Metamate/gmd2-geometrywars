using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class GravityBehaviour : IComponent
{
    private readonly float _range;
    private readonly float _force;
    private TransformComponent _transform;

    public GravityBehaviour(float range, float force)
    {
        _range = range;
        _force = force;
    }

    public void OnAdded(Entity owner)
    {
        _transform = owner.Transform;
    }

    public void Update(Entity owner)
    {
        foreach (var entity in EntityManager.GetNearbyEntities(_transform.Position, _range))
        {
            if (entity == owner || entity is BlackHole) continue;

            if (entity is Enemy enemy && !enemy.IsActive)
                continue;

            var targetTransform = entity.Transform;
            var targetRigidbody = entity.GetComponent<RigidbodyComponent>();
            
            if (targetTransform == null || targetRigidbody == null) continue;

            if (entity is Bullet)
            {
                targetRigidbody.Velocity += (targetTransform.Position - _transform.Position).ScaleTo(0.3f);
            }
            else
            {
                var dPos = _transform.Position - targetTransform.Position;
                targetRigidbody.Velocity += dPos.ScaleTo(MathHelper.Lerp(_force, 0, dPos.Length() / _range));
            }
        }
    }
}
