using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace GeometryWars.Components.Combat;

// Component that translates user input into movement and combat actions.
public sealed class PlayerInputComponent : Component
{
    private readonly IBulletSpawner _bulletSpawner;
    private readonly GameController _controller;
    private readonly AudioManager _audio;
    private readonly Func<SoundEffect> _getShotSound;
    private int _cooldownRemaining;
    private PlayerShip _player;
    private RigidbodyComponent _rigidbody;
    private TransformComponent _transform;

    public PlayerInputComponent(IBulletSpawner bulletSpawner, GameController controller, AudioManager audio, Func<SoundEffect> getShotSound)
    {
        _bulletSpawner = bulletSpawner;
        _controller = controller;
        _audio = audio;
        _getShotSound = getShotSound;
    }

    public override void OnStart(Entity owner)
    {
        _player    = owner as PlayerShip;
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
        _transform = owner.Transform;
    }

    public override void PreUpdate(Entity owner)
    {
        if (_player == null || _player.IsDead)
            return;

        _rigidbody.AddForce(GameSettings.Player.Speed * _controller.Movement);

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
        else if (_rigidbody.Velocity.LengthSquared() > 0.01f)
        {
            _transform.Orientation = _rigidbody.Velocity.ToAngle();
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
        Vector2 offsetB = new(GameSettings.Bullets.OffsetX,  GameSettings.Bullets.OffsetY);

        _bulletSpawner.SpawnBullet(_transform.Position + Vector2.Transform(offsetA, aimQuat), vel);
        _bulletSpawner.SpawnBullet(_transform.Position + Vector2.Transform(offsetB, aimQuat), vel);

        _audio.Play(_getShotSound(), 0.2f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}
