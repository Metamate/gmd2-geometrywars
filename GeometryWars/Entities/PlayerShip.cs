using System;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

public class PlayerShip : Entity
{
    private int _framesUntilRespawn = 0;

    public bool IsDead => _framesUntilRespawn > 0;

    public PlayerShip()
    {
        Image    = Art.Player;
        Position = FrameContext.ScreenSize / 2;

        // Damping = 0 clears velocity after each frame; player ship is input-driven,
        // not physics-based — no momentum carries over between frames.
        AddComponent(new VelocityMover(damping: 0f, clampToScreen: true));
        
        // Component-based logic: handling input and visuals separately.
        AddComponent(new PlayerInputComponent());
        AddComponent(new ExhaustFireComponent());
        AddComponent(new GlowOverlay(Art.Glow, Color.White * 0.15f));

        Collider = new CircleCollider(GameSettings.Bullets.ColliderRadius);
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
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsDead)
            base.Draw(spriteBatch);
    }

    public void Kill()
    {
        PlayerStatus.RemoveLife();
        _framesUntilRespawn = PlayerStatus.IsGameOver ? GameSettings.Player.GameOverFrames : GameSettings.Player.RespawnFrames;

        Color yellow = new(0.8f, 0.8f, 0.4f);
        for (int i = 0; i < GameSettings.Visuals.PlayerDeathParticles; i++)
        {
            float speed = GameSettings.Visuals.DeathParticleSpeed * (1f - 1 / Random.Shared.NextFloat(1f, 10f));
            Color color = Color.Lerp(Color.White, yellow, Random.Shared.NextFloat(0, 1));
            GameServices.Particles.CreateParticle(Art.LineParticle, Position, color, GameSettings.Visuals.DeathParticleLife, GameSettings.Visuals.DeathParticleSize,
                new ParticleState { Velocity = Random.Shared.NextVector2(speed, speed), Type = ParticleType.None, LengthMultiplier = 1 });
        }
    }
}
