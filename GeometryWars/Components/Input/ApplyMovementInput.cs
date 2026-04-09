using GeometryWars.Components.Core;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Input;
using GeometryWars.Utils;

namespace GeometryWars.Components.Input;

// Translates movement input into thrust and idle-facing direction.
public sealed class ApplyMovementInput : Component
{
    private readonly GameController _controller;
    private RespawnState _respawnState;
    private Rigidbody _rigidbody;
    private Transform _transform;

    public ApplyMovementInput(GameController controller)
    {
        _controller = controller;
    }

    public override void OnStart(Entity owner)
    {
        _respawnState = owner.GetComponent<RespawnState>();
        _rigidbody = owner.GetComponent<Rigidbody>();
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
