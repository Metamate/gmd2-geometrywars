using GMDCore.ECS.Components;
using GeometryWars.Components.Lifecycle;
using GMDCore.Physics;
using GMDCore.ECS;
using GeometryWars.Input;
using GeometryWars.Utils;

namespace GeometryWars.Components.Input;

// Translates movement input into thrust and idle-facing direction.
public sealed class ApplyMovementInput : Component
{
    private readonly GameController _controller;
    private readonly float _moveSpeed;
    private RespawnState _respawnState;
    private Rigidbody _rigidbody;
    private Transform _transform;

    public ApplyMovementInput(GameController controller, float moveSpeed)
    {
        _controller = controller;
        _moveSpeed = moveSpeed;
    }

    public override void OnStart(Entity owner)
    {
        _respawnState = owner.RequireComponent<RespawnState>();
        _rigidbody = owner.RequireComponent<Rigidbody>();
        _transform = owner.Transform;
    }

    public override void PreUpdate(Entity owner)
    {
        if (_respawnState?.IsRespawning == true)
            return;

        _rigidbody.AddForce(_moveSpeed * _controller.Movement);

        if (!_controller.IsShooting && _rigidbody.Velocity.LengthSquared() > 0.01f)
            _transform.Orientation = _rigidbody.Velocity.ToAngle();
    }
}

