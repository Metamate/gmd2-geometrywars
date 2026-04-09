using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Systems;

namespace GeometryWars.Components.Physics;

// Pushes the background grid based on the owner's current speed.
public sealed class ApplyGridForceFromVelocity : Component
{
    private readonly IGridField _grid;
    private readonly float _forceScale;
    private readonly float _radius;
    private Transform _transform;
    private Rigidbody _rigidbody;

    public ApplyGridForceFromVelocity(IGridField grid, float forceScale, float radius)
    {
        _grid = grid;
        _forceScale = forceScale;
        _radius = radius;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _rigidbody = owner.GetComponent<Rigidbody>();
    }

    public override void PostUpdate(Entity owner)
    {
        _grid.ApplyExplosiveForce(_forceScale * _rigidbody.Velocity.Length(), _transform.Position, _radius);
    }
}
