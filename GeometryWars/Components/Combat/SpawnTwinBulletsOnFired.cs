using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;
using GeometryWars.Systems;
using GeometryWars.Utils;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Combat;

// Spawns the default twin-bullet spread whenever the owner's weapon fires.
public sealed class SpawnTwinBulletsOnFired : Component
{
    private readonly IBulletSpawner _bulletSpawner;
    private Weapon _weapon;

    public SpawnTwinBulletsOnFired(IBulletSpawner bulletSpawner)
    {
        _bulletSpawner = bulletSpawner;
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
        float randomSpread = Random.Shared.NextFloat(-GameSettings.Bullets.Spread, GameSettings.Bullets.Spread)
                           + Random.Shared.NextFloat(-GameSettings.Bullets.Spread, GameSettings.Bullets.Spread);

        Vector2 velocity = MathUtil.FromPolar(shot.AimAngle + randomSpread, GameSettings.Bullets.Speed);
        Vector2 offsetA = new(GameSettings.Bullets.OffsetX, -GameSettings.Bullets.OffsetY);
        Vector2 offsetB = new(GameSettings.Bullets.OffsetX, GameSettings.Bullets.OffsetY);

        _bulletSpawner.SpawnBullet(shot.Origin + Vector2.Transform(offsetA, aimQuat), velocity);
        _bulletSpawner.SpawnBullet(shot.Origin + Vector2.Transform(offsetB, aimQuat), velocity);
    }
}
