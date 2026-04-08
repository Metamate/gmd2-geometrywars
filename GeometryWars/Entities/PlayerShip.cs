using System;
using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Components.Visuals;
using GeometryWars.Components.Combat;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Services;
using GeometryWars.Systems;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GeometryWars.Entities;

// Archetype for the player-controlled ship.
public class PlayerShip : Entity
{
    private readonly PlayerRespawnBehaviour _respawn;

    public bool IsDead => _respawn.IsDead;

    public PlayerShip()
    {
        Transform.Position = FrameContext.ScreenSize / 2;
        Vector2 size = new(Art.Player.Width, Art.Player.Height);

        AddComponent(new RigidbodyComponent(damping: 0f));
        AddComponent(new ScreenClampBehaviour(size));
        AddComponent(new SpriteComponent(Art.Player));
        AddComponent(new PlayerInputComponent());
        AddComponent(new ExhaustFireComponent());
        AddComponent(new GlowOverlay(Art.Glow, Color.White * 0.15f));
        AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
        AddComponent(new PlayerCollisionBehaviour());

        _respawn = AddComponent(new PlayerRespawnBehaviour());
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

        var pos = Transform.Position;

        Color yellow = new(0.8f, 0.8f, 0.4f);
        for (int i = 0; i < GameSettings.Visuals.PlayerDeathParticles; i++)
        {
            float speed = GameSettings.Visuals.DeathParticleSpeed * (1f - 1 / Random.Shared.NextFloat(1f, 10f));
            Color color = Color.Lerp(Color.White, yellow, Random.Shared.NextFloat(0, 1));
            GameServices.Particles.CreateParticle(Art.LineParticle, pos, color, GameSettings.Visuals.DeathParticleLife, GameSettings.Visuals.DeathParticleSize,
                new ParticleState { Velocity = Random.Shared.NextVector2(speed, speed), Type = ParticleType.None, LengthMultiplier = 1 });
        }
    }
}
