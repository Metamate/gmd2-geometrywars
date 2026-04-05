using System;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public class PlayerShip : Entity
{
    private static readonly Random rand = new();
    private int framesUntilRespawn = 0;
    private const int CooldownFrames = 6;
    private int cooldownRemaining = 0;

    public bool IsDead => framesUntilRespawn > 0;

    public PlayerShip()
    {
        image = Art.Player;
        Position = GameServices.ScreenSize / 2;

        // Damping = 0 clears velocity after each frame; player ship is input-driven,
        // not physics-based — no momentum carries over between frames.
        AddComponent(new VelocityMover(damping: 0f, clampToScreen: true));

        // Player dies on contact with any active enemy or black hole.
        // All enemies/blackholes are cleared as part of Kill().
        AddComponent(new CircleCollider(10, other =>
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
        }));
    }

    protected override void OnPreUpdate()
    {
        if (IsDead)
        {
            if (--framesUntilRespawn == 0 && !PlayerStatus.IsGameOver)
                GameServices.Grid.ApplyDirectedForce(new Vector3(0, 0, 5000), new Vector3(Position, 0), 50);
            return;
        }

        const float speed = 8;
        Velocity += speed * Input.GetMovementDirection();

        if (Velocity.LengthSquared() > 0)
            Orientation = Velocity.ToAngle();

        MakeExhaustFire();

        var aim = Input.GetAimDirection(Position);
        if (aim.LengthSquared() > 0 && cooldownRemaining <= 0)
        {
            cooldownRemaining = CooldownFrames;
            float aimAngle = aim.ToAngle();
            Quaternion aimQuat = Quaternion.CreateFromYawPitchRoll(0, 0, aimAngle);
            float randomSpread = rand.NextFloat(-0.04f, 0.04f) + rand.NextFloat(-0.04f, 0.04f);
            Vector2 vel = MathUtil.FromPolar(aimAngle + randomSpread, 11f);

            EntityManager.Add(EntityManager.GetBullet(Position + Vector2.Transform(new Vector2(25, -8), aimQuat), vel));
            EntityManager.Add(EntityManager.GetBullet(Position + Vector2.Transform(new Vector2(25, 8), aimQuat), vel));

            Sound.Shot.Play(0.2f, rand.NextFloat(-0.2f, 0.2f), 0);
        }

        if (cooldownRemaining > 0)
            cooldownRemaining--;
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsDead)
            base.Draw(spriteBatch);
    }

    public void Kill()
    {
        PlayerStatus.RemoveLife();
        framesUntilRespawn = PlayerStatus.IsGameOver ? 300 : 120;

        Color yellow = new(0.8f, 0.8f, 0.4f);
        for (int i = 0; i < 1200; i++)
        {
            float speed = 18f * (1f - 1 / rand.NextFloat(1f, 10f));
            Color color = Color.Lerp(Color.White, yellow, rand.NextFloat(0, 1));
            GameServices.Particles.CreateParticle(Art.LineParticle, Position, color, 190, 1.5f,
                new ParticleState { Velocity = rand.NextVector2(speed, speed), Type = ParticleType.None, LengthMultiplier = 1 });
        }
    }

    private void MakeExhaustFire()
    {
        if (Velocity.LengthSquared() <= 0.1f)
            return;

        Orientation = Velocity.ToAngle();
        Quaternion rot = Quaternion.CreateFromYawPitchRoll(0f, 0f, Orientation);
        double t = GameServices.TotalSeconds;

        Vector2 baseVel = Velocity.ScaleTo(-3);
        Vector2 perpVel = new Vector2(baseVel.Y, -baseVel.X) * (0.6f * (float)Math.Sin(t * 10));
        Color sideColor = new Color(200, 38, 9);
        Color midColor = new Color(255, 187, 30);
        Vector2 pos = Position + Vector2.Transform(new Vector2(-25, 0), rot);
        const float alpha = 0.7f;

        Vector2 velMid = baseVel + rand.NextVector2(0, 1);
        GameServices.Particles.CreateParticle(Art.LineParticle, pos, Color.White * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(velMid, ParticleType.Enemy));
        GameServices.Particles.CreateParticle(Art.Glow, pos, midColor * alpha, 60f, new Vector2(0.5f, 1),
            new ParticleState(velMid, ParticleType.Enemy));

        Vector2 vel1 = baseVel + perpVel + rand.NextVector2(0, 0.3f);
        Vector2 vel2 = baseVel - perpVel + rand.NextVector2(0, 0.3f);
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
