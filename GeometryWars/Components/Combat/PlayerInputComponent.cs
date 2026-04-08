using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Combat;

// Component that translates user input into movement and combat actions.
public sealed class PlayerInputComponent : Component
{
    private int _cooldownRemaining = 0;
    private RigidbodyComponent _rigidbody;
    private TransformComponent _transform;

    public override void OnAdded(Entity owner)
    {
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
        _transform = owner.Transform;
    }

    public override void Update(Entity owner)
    {
        if (owner is not PlayerShip player || player.IsDead)
            return;

        _rigidbody.AddForce(GameSettings.Player.Speed * GameController.Movement);

        var aim = GameController.AimDirection(_transform.Position);
        if (GameController.IsShooting)
        {
            _transform.Orientation = aim.ToAngle();

            if (_cooldownRemaining <= 0)
            {
                _cooldownRemaining = GameSettings.Bullets.ShotCooldown;
                Shoot(owner, aim, _transform.Orientation);
            }
        }
        else if (_rigidbody.Velocity.LengthSquared() > 0.01f)
        {
            _transform.Orientation = _rigidbody.Velocity.ToAngle();
        }

        if (_cooldownRemaining > 0)
            _cooldownRemaining--;
    }

    private void Shoot(Entity owner, Vector2 aim, float aimAngle)
    {
        Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);
        float randomSpread = Random.Shared.NextFloat(-GameSettings.Bullets.Spread, GameSettings.Bullets.Spread)
                           + Random.Shared.NextFloat(-GameSettings.Bullets.Spread, GameSettings.Bullets.Spread);
        
        Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, GameSettings.Bullets.Speed);
        Vector2 offsetA = new(GameSettings.Bullets.OffsetX, -GameSettings.Bullets.OffsetY);
        Vector2 offsetB = new(GameSettings.Bullets.OffsetX,  GameSettings.Bullets.OffsetY);
        
        EntityManager.Add(EntityManager.GetBullet(_transform.Position + Vector2.Transform(offsetA, aimQuat), vel));
        EntityManager.Add(EntityManager.GetBullet(_transform.Position + Vector2.Transform(offsetB, aimQuat), vel));

        GameServices.Audio.Play(Sound.Shot, 0.2f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}
