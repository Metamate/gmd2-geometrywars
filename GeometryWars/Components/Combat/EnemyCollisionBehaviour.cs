using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components.Combat;

/// <summary>
/// Handles collision response for enemies, including death effects and score.
/// </summary>
public sealed class EnemyCollisionBehaviour : Component, ICollisionComponent
{
    private RigidbodyComponent _rigidbody;
    private TransformComponent _transform;

    public override void OnAdded(Entity owner)
    {
        _rigidbody = owner.GetComponent<RigidbodyComponent>();
        _transform = owner.Transform;
    }

    public override void Update(Entity owner) { }

    public void OnCollision(Entity owner, Entity other)
    {
        if (owner is not Enemy enemy) return;

        if (other is Bullet || (other is BlackHole bh && bh.IsActive))
        {
            WasShot(enemy);
        }
        else if (other is Enemy e)
        {
            var otherTransform = e.Transform;
            var otherRigidbody = e.GetComponent<RigidbodyComponent>();
            if (otherTransform == null || otherRigidbody == null) return;

            var d = _transform.Position - otherTransform.Position;
            _rigidbody.AddForce(10 * d / (d.LengthSquared() + 1));
        }
    }

    private void WasShot(Enemy enemy)
    {
        enemy.IsExpired = true;

        float hue1 = Random.Shared.NextFloat(0, 6);
        float hue2 = (hue1 + Random.Shared.NextFloat(0, 2)) % 6f;
        Color color1 = ColorUtil.HSVToColor(hue1, 0.5f, 1);
        Color color2 = ColorUtil.HSVToColor(hue2, 0.5f, 1);
        for (int i = 0; i < GameSettings.Visuals.EnemyDeathParticles; i++)
        {
            float speed = GameSettings.Visuals.DeathParticleSpeed * (1f - 1 / Random.Shared.NextFloat(1f, 10f));
            var state = new ParticleState
            {
                Velocity = Random.Shared.NextVector2(speed, speed),
                Type = ParticleType.Enemy,
                LengthMultiplier = 1f
            };
            Color color = Color.Lerp(color1, color2, Random.Shared.NextFloat(0, 1));
            GameServices.Particles.CreateParticle(Art.LineParticle, _transform.Position, color, GameSettings.Visuals.DeathParticleLife, GameSettings.Visuals.DeathParticleSize, state);
        }

        PlayerStatus.AddPoints(enemy.PointValue);
        PlayerStatus.IncreaseMultiplier();
        GameServices.Audio.Play(Sound.Explosion, 0.5f, Random.Shared.NextFloat(-0.2f, 0.2f));
    }
}
