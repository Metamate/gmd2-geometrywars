using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class PlayerInputComponent : IComponent
{
    private int _cooldownRemaining = 0;
    private MovementComponent _movement;

    public void OnAdded(Entity owner)
    {
        // CACHING: Find the sibling component exactly once at birth
        _movement = owner.Movement;
    }

    public void Update(Entity owner)
    {
        if (owner is not PlayerShip player || player.IsDead || _movement == null)
            return;

        // 1. Movement
        _movement.Velocity += GameSettings.Player.Speed * GameController.Movement;

        // 2. Shooting & Orientation
        var aim = GameController.AimDirection(owner.Position);
        
        if (GameController.IsShooting)
        {
            _movement.Orientation = aim.ToAngle();

            if (_cooldownRemaining <= 0)
            {
                _cooldownRemaining = GameSettings.Bullets.ShotCooldown;
                Shoot(owner, aim, _movement.Orientation);
            }
        }
        else if (_movement.Velocity.LengthSquared() > 0.01f)
        {
            _movement.Orientation = _movement.Velocity.ToAngle();
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
        
        EntityManager.Add(EntityManager.GetBullet(owner.Position + Vector2.Transform(offsetA, aimQuat), vel));
        EntityManager.Add(EntityManager.GetBullet(owner.Position + Vector2.Transform(offsetB, aimQuat), vel));

        GameServices.Audio.Play(Sound.Shot, 0.2f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}
