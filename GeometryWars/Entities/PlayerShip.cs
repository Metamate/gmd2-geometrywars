using System;
using GeometryWars.Components;
using GeometryWars.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars;

/// <summary>
/// Archetype for the Player Ship.
/// Composed of movement, input, visuals, and respawn components.
/// </summary>
public class PlayerShip : Entity
{
    private readonly PlayerRespawnBehaviour _respawn;

    public bool IsDead => _respawn.IsDead;

    public PlayerShip()
    {
        Image    = Art.Player;
        Position = FrameContext.ScreenSize / 2;

        // Assembler: Plug in the specific behaviours of the Player
        AddComponent(new VelocityMover(damping: 0f, clampToScreen: true));
        AddComponent(new PlayerInputComponent());
        AddComponent(new ExhaustFireComponent());
        AddComponent(new GlowOverlay(Art.Glow, Color.White * 0.15f));
        AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
        
        // Keep a reference to the respawn component so we can check IsDead
        _respawn = AddComponent(new PlayerRespawnBehaviour());
    }

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

    public override void Draw(SpriteBatch spriteBatch)
    {
        if (!IsDead)
            base.Draw(spriteBatch);
    }

    public void Kill()
    {
        PlayerStatus.RemoveLife();
        
        int frames = PlayerStatus.IsGameOver ? GameSettings.Player.GameOverFrames : GameSettings.Player.RespawnFrames;
        _respawn.Kill(frames);

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
