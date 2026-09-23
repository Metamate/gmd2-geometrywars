using System;
using GMDCore.ECS.Components;
using GeometryWars1.Components.Lifecycle;
using GeometryWars1.Components.Combat;
using GMDCore.ECS;
using GeometryWars1.Input;
using GeometryWars1.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars1.Components.Input;

// Translates firing input into aiming and weapon trigger requests.
public sealed class FireWeaponOnInput : Component
{
    private readonly GameController _controller;
    private Transform _transform;
    private Weapon _weapon;

    public FireWeaponOnInput(GameController controller)
    {
        _controller = controller;
    }

    public override void OnStart(Entity owner)
    {
        _transform = owner.Transform;
        _weapon = owner.RequireComponent<Weapon>();
    }

    public override void PreUpdate(Entity owner)
    {
        var aim = _controller.AimDirection(_transform.Position);
        if (!_controller.IsShooting || aim == Vector2.Zero)
            return;

        _transform.Orientation = aim.ToAngle();
        _weapon.TryFire(_transform.Position, _transform.Orientation);
    }
}
