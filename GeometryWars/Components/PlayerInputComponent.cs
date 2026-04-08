using System;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class PlayerInputComponent : IComponent
{
    private int _cooldownRemaining = 0;

    public void Update(Entity owner)
    {
        if (owner is not PlayerShip player || player.IsDead)
            return;

        // PERFORMANCE: Using the cached property on Entity instead of GetComponent
        var movement = owner.Movement;
        if (movement == null) return;

        // 1. Movement
        movement.Velocity += GameSettings.Player.Speed * GameController.Movement;

        // 2. Shooting & Orientation
        var aim = GameController.AimDirection(owner.Position);
        
        if (GameController.IsShooting)
        {
            // Face the direction of aim
            movement.Orientation = aim.ToAngle();

            if (_cooldownRemaining <= 0)
            {
                _cooldownRemaining = GameSettings.Bullets.ShotCooldown;
                Shoot(owner, aim, movement.Orientation);
            }
        }
        else if (movement.Velocity.LengthSquared() > 0.01f)
        {
            // Face the direction of travel
            movement.Orientation = movement.Velocity.ToAngle();
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
