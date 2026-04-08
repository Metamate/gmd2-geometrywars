using GeometryWars.Components.Physics;
using GeometryWars.Components.Visuals;
using GeometryWars.Components.Combat;
using GeometryWars.Components.Lifecycle;
using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Entities;

// Archetype for the player-controlled ship.
// Assembles components; all behaviour lives in those components.
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

        _respawn = AddComponent(new PlayerRespawnBehaviour());
        AddComponent(new PlayerCollisionBehaviour());
    }
}
