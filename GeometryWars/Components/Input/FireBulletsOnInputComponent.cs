using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GeometryWars.Components.Input;

// Translates firing input into aim, cooldown handling, bullet spawning, and shot audio.
public sealed class FireBulletsOnInputComponent : Component
{
    private readonly IBulletSpawner _bulletSpawner;
    private readonly GameController _controller;
    private readonly AudioManager _audio;
    private readonly Func<SoundEffect> _getShotSound;
    private int _cooldownRemaining;
    private RespawnStateComponent _respawnState;
    private TransformComponent _transform;

    public FireBulletsOnInputComponent(IBulletSpawner bulletSpawner, GameController controller, AudioManager audio, Func<SoundEffect> getShotSound)
    {
        _bulletSpawner = bulletSpawner;
        _controller = controller;
        _audio = audio;
        _getShotSound = getShotSound;
    }

    public override void OnStart(Entity owner)
    {
        _respawnState = owner.GetComponent<RespawnStateComponent>();
        _transform = owner.Transform;
    }

    public override void PreUpdate(Entity owner)
    {
        if (_respawnState?.IsRespawning == true)
            return;

        var aim = _controller.AimDirection(_transform.Position);
        if (_controller.IsShooting)
        {
            _transform.Orientation = aim.ToAngle();

            if (_cooldownRemaining <= 0)
            {
                _cooldownRemaining = GameSettings.Bullets.ShotCooldown;
                Shoot(aim, _transform.Orientation);
            }
        }

        if (_cooldownRemaining > 0)
            _cooldownRemaining--;
    }

    private void Shoot(Vector2 aim, float aimAngle)
    {
        Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);
        float randomSpread = Random.Shared.NextFloat(-GameSettings.Bullets.Spread, GameSettings.Bullets.Spread)
                           + Random.Shared.NextFloat(-GameSettings.Bullets.Spread, GameSettings.Bullets.Spread);

        Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, GameSettings.Bullets.Speed);
        Vector2 offsetA = new(GameSettings.Bullets.OffsetX, -GameSettings.Bullets.OffsetY);
        Vector2 offsetB = new(GameSettings.Bullets.OffsetX, GameSettings.Bullets.OffsetY);

        _bulletSpawner.SpawnBullet(_transform.Position + Vector2.Transform(offsetA, aimQuat), vel);
        _bulletSpawner.SpawnBullet(_transform.Position + Vector2.Transform(offsetB, aimQuat), vel);

        _audio.Play(_getShotSound(), 0.2f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}
