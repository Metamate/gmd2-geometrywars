using GMDCore.ECS.Components;
using GMDCore.Physics;
using GMDCore.ECS;
using GeometryWars0.Input;
using GeometryWars0.Utils;

namespace GeometryWars0.Components.Input;

// Translates movement input into thrust and idle-facing direction.
public sealed class ApplyMovementInput : Component
{
    private readonly GameController _controller;
    private readonly float _moveSpeed;
    private Rigidbody _rigidbody;
    private Transform _transform;

    public ApplyMovementInput(GameController controller, float moveSpeed)
    {
        _controller = controller;
        _moveSpeed = moveSpeed;
    }

    public override void OnStart(Entity owner)
    {
        _rigidbody = owner.RequireComponent<Rigidbody>();
        _transform = owner.Transform;
    }

    public override void PreUpdate(Entity owner)
    {
        _rigidbody.AddForce(_moveSpeed * _controller.Movement);

        if (!_controller.IsShooting && _rigidbody.Velocity.LengthSquared() > 0.01f)
            _transform.Orientation = _rigidbody.Velocity.ToAngle();
    }
}
