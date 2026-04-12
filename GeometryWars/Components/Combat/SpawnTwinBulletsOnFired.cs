using System;
using GMDCore.ECS.Components;
using GMDCore.ECS;
using GeometryWars.Definitions;
using GeometryWars.Systems;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Combat;

// Spawns the default twin-bullet spread whenever the owner's weapon fires.
public sealed class SpawnTwinBulletsOnFired : Component
{
    private readonly IBulletSpawner _bulletSpawner;
    private readonly TwinBulletPatternDefinition _pattern;
    private Weapon _weapon;

    public SpawnTwinBulletsOnFired(IBulletSpawner bulletSpawner, TwinBulletPatternDefinition pattern)
    {
        _bulletSpawner = bulletSpawner;
        _pattern = pattern;
    }

    public override void OnStart(Entity owner)
    {
        _weapon = owner.RequireComponent<Weapon>();
        _weapon.Fired += HandleFired;
    }

    public override void OnRemoved(Entity owner)
    {
        if (_weapon != null)
            _weapon.Fired -= HandleFired;
    }

    private void HandleFired(WeaponFired shot)
    {
        Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, shot.AimAngle);
        float randomSpread = Random.Shared.NextFloat(-_pattern.Spread, _pattern.Spread)
                           + Random.Shared.NextFloat(-_pattern.Spread, _pattern.Spread);

        Vector2 velocity = MathUtil.FromPolar(shot.AimAngle + randomSpread, _pattern.Speed);
        Vector2 offsetA = new(_pattern.OffsetX, -_pattern.OffsetY);
        Vector2 offsetB = new(_pattern.OffsetX, _pattern.OffsetY);

        _bulletSpawner.SpawnBullet(shot.Origin + Vector2.Transform(offsetA, aimQuat), velocity);
        _bulletSpawner.SpawnBullet(shot.Origin + Vector2.Transform(offsetB, aimQuat), velocity);
    }
}

