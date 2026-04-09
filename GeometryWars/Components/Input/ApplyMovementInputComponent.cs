using GeometryWars.Components.Core;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Services;

namespace GeometryWars.Components.Input;

// Translates movement input into thrust and idle-facing direction.
public sealed class ApplyMovementInputComponent : Component
{
    private readonly GameController _controller;
    private RespawnStateComponent _respawnState;
    private RigidbodyComponent _rigidbody;
    private TransformComponent _transform;

    public ApplyMovementInputComponent(GameController controller)
    {
        _controller = controller;
    }

    public override void OnStart(Entity owner)
    {
        _respawnState = owner.GetComponent<RespawnStateComponent>();
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
        _transform = owner.Transform;
    }

    public override void PreUpdate(Entity owner)
    {
        if (_respawnState?.IsRespawning == true)
            return;

        _rigidbody.AddForce(GameSettings.Player.Speed * _controller.Movement);

        if (!_controller.IsShooting && _rigidbody.Velocity.LengthSquared() > 0.01f)
            _transform.Orientation = _rigidbody.Velocity.ToAngle();
    }
}
