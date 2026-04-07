using System;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public class PlayerShip : Entity
{
    private int _framesUntilRespawn = 0;
    private int _cooldownRemaining = 0;

    public bool IsDead => _framesUntilRespawn > 0;

    public PlayerShip()
    {
        Image    = Art.Player;
        Position = FrameContext.ScreenSize / 2;

        // Damping = 0 clears velocity after each frame; player ship is input-driven,
        // not physics-based — no momentum carries over between frames.
        AddComponent(new VelocityMover(damping: 0f, clampToScreen: true));

        Collider = new CircleCollider(GameSettings.PlayerColliderRadius);
    }

    // Collision response: the player dies on contact with any active enemy or black hole.
    // All enemies and black holes are cleared as part of the death sequence so the
    // player does not score points from the explosion of their own death.
    public override void OnCollision(Entity other)
    {
        if (IsDead) return;

        if (other is Enemy e && e.IsActive)
        {
            Kill();
            EntityManager.KillAllEnemies();
            EnemySpawner.Reset();
        }
        else if (other is BlackHole)
        {
            Kill();
            EntityManager.KillAllEnemies();
            EntityManager.KillAllBlackHoles();
            EnemySpawner.Reset();
        }
    }

    protected override void OnUpdate()
    {
        if (IsDead)
        {
            if (--_framesUntilRespawn == 0 && !PlayerStatus.IsGameOver)
                GameServices.Grid.ApplyDirectedForce(new Vector3(0, 0, 5000), new Vector3(Position, 0), 50);
            return;
        }

        Velocity += GameSettings.PlayerSpeed * Input.GetMovementDirection();

        if (Velocity.LengthSquared() > 0)
            Orientation = Velocity.ToAngle();

        MakeExhaustFire();

        var aim = Input.GetAimDirection(Position);
        if (Input.IsShooting() && _cooldownRemaining <= 0)
        {
            _cooldownRemaining = GameSettings.PlayerShotCooldown;
            float aimAngle = aim.ToAngle();
            Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);
            float randomSpread = Random.Shared.NextFloat(-GameSettings.PlayerBulletSpread, GameSettings.PlayerBulletSpread)
                               + Random.Shared.NextFloat(-GameSettings.PlayerBulletSpread, GameSettings.PlayerBulletSpread);
            Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, GameSettings.PlayerBulletSpeed);

            Vector2 offsetA = new(GameSettings.PlayerBulletOffsetX, -GameSettings.PlayerBulletOffsetY);
            Vector2 offsetB = new(GameSettings.PlayerBulletOffsetX,  GameSettings.PlayerBulletOffsetY);
            EntityManager.Add(EntityManager.GetBullet(Position + Vector2.Transform(offsetA, aimQuat), vel));
            EntityManager.Add(EntityManager.GetBullet(Position + Vector2.Transform(offsetB, aimQuat), vel));

            GameServices.Audio.Play(Sound.Shot, 0.2f, Random.Shared.NextFloat(-0.2f, 0.2f));
        }

        if (_cooldownRemaining > 0)
            _cooldownRemaining--;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsDead)
            base.Draw(spriteBatch);
    }

    public void Kill()
    {
        PlayerStatus.RemoveLife();
        _framesUntilRespawn = PlayerStatus.IsGameOver ? GameSettings.PlayerGameOverFrames : GameSettings.PlayerRespawnFrames;

        Color yellow = new(0.8f, 0.8f, 0.4f);
        for (int i = 0; i < GameSettings.PlayerDeathParticles; i++)
        {
            float speed = GameSettings.DeathParticleSpeed * (1f - 1 / Random.Shared.NextFloat(1f, 10f));
            Color color = Color.Lerp(Color.White, yellow, Random.Shared.NextFloat(0, 1));
            GameServices.Particles.CreateParticle(Art.LineParticle, Position, color, GameSettings.DeathParticleLife, GameSettings.DeathParticleSize,
                new ParticleState { Velocity = Random.Shared.NextVector2(speed, speed), Type = ParticleType.None, LengthMultiplier = 1 });
        }
    }

    private void MakeExhaustFire()
    {
        if (Velocity.LengthSquared() <= 0.1f)
            return;

        Orientation = Velocity.ToAngle();
        Quaternion rot = Quaternion.CreateFromYawPitchRoll(0f, 0f, Orientation);
        double t = FrameContext.TotalSeconds;

        Vector2 baseVel = Velocity.ScaleTo(-3);
        Vector2 perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));
        Color sideColor = new Color(200, 38, 9);
        Color midColor = new Color(255, 187, 30);
        Vector2 pos = Position + Vector2.Transform(new Vector2(-25, 0), rot);
        const float alpha = 0.7f;

        Vector2 velMid = baseVel + Random.Shared.NextVector2(0, 1);
        GameServices.Particles.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(velMid, ParticleType.Enemy));
        GameServices.Particles.CreateParticle(Art.Glow, pos, midColor * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(velMid, ParticleType.Enemy));

        Vector2 vel1 = baseVel + perpVel + Random.Shared.NextVector2(0, 0.3f);
        Vector2 vel2 = baseVel - perpVel + Random.Shared.NextVector2(0, 0.3f);
        GameServices.Particles.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel1, ParticleType.Enemy));
        GameServices.Particles.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel2, ParticleType.Enemy));
        GameServices.Particles.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel1, ParticleType.Enemy));
        GameServices.Particles.CreateParticle(Art.Glow, pos, sideColor * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(vel2, ParticleType.Enemy));
    }
}
