using System;
using GeometryWars.Components.Core;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Combat;

public readonly record struct WeaponFired(Vector2 Origin, float AimAngle);

// Owns weapon cadence and raises a local event whenever a shot is fired.
public sealed class Weapon : Component
{
    private readonly int _cooldownFrames;
    private int _cooldownRemaining;

    public event Action<WeaponFired> Fired;

    public Weapon(int cooldownFrames)
    {
        _cooldownFrames = cooldownFrames;
    }

    public bool CanFire => _cooldownRemaining <= 0;

    public void TryFire(Vector2 origin, float aimAngle)
    {
        if (!CanFire)
            return;

        _cooldownRemaining = _cooldownFrames;
        Fired?.Invoke(new WeaponFired(origin, aimAngle));
    }

    public override void PostUpdate(Entities.Entity owner)
    {
        if (_cooldownRemaining > 0)
            _cooldownRemaining--;
    }
}
