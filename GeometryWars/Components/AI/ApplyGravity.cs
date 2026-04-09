using GeometryWars.Components.Core;
using GeometryWars.Components.Identity;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Systems;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.AI;

// Applies gravitational pull to nearby entities.
public sealed class ApplyGravity : Component
{
    private readonly float _range;
    private readonly float _force;
    private readonly INeighborQuery _neighborQuery;
    private Transform _transform;

    public ApplyGravity(float range, float force, INeighborQuery neighborQuery)
    {
        _range = range;
        _force = force;
        _neighborQuery = neighborQuery;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
    }

    public override void Update(Entity owner)
    {
        _neighborQuery.ForEachNearbyEntity(_transform.Position, _range, entity =>
        {
            if (entity == owner || entity.HasComponent<BlackHoleTag>()) return;

            var targetTransform = entity.Transform;
            var targetRigidbody = entity.GetComponent<Rigidbody>();
            
            if (targetTransform == null || targetRigidbody == null) return;

            if (!targetRigidbody.IsActive) return;

            if (entity.HasComponent<BulletTag>())
            {
                targetRigidbody.AddForce((targetTransform.Position - _transform.Position).ScaleTo(0.3f));
            }
            else
            {
                var dPos = _transform.Position - targetTransform.Position;
                targetRigidbody.AddForce(dPos.ScaleTo(MathHelper.Lerp(_force, 0, dPos.Length() / _range)));
            }
        });
    }
}
