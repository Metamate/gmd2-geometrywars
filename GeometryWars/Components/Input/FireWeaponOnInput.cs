using System;
using GMDCore.ECS.Components;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Components.Combat;
using GMDCore.ECS;
using GeometryWars.Input;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Input;

// Translates firing input into aiming and weapon trigger requests.
public sealed class FireWeaponOnInput : Component
{
    private readonly GameController _controller;
    private RespawnState _respawnState;
    private Transform _transform;
    private Weapon _weapon;

    public FireWeaponOnInput(GameController controller)
    {
        _controller = controller;
    }

    public override void OnStart(Entity owner)
    {
        _respawnState = owner.RequireComponent<RespawnState>();
        _transform = owner.Transform;
        _weapon = owner.RequireComponent<Weapon>();
    }

    public override void PreUpdate(Entity owner)
    {
        if (_respawnState?.IsRespawning == true)
            return;

        var aim = _controller.AimDirection(_transform.Position);
        if (!_controller.IsShooting || aim == Vector2.Zero)
            return;

        _transform.Orientation = aim.ToAngle();
        _weapon.TryFire(_transform.Position, _transform.Orientation);
    }
}

