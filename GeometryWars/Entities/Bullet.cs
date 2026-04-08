using Microsoft.Xna.Framework;
using GeometryWars.Components;

namespace GeometryWars;

/// <summary>
/// Archetype for Bullets.
/// Composed of movement, collision, and collider components.
/// </summary>
public class Bullet : Entity
{
    private readonly MovementComponent _movement;

    public Bullet()
    {
        Image    = Art.Bullet;

        // Assembler: Plug in the specific behaviours of a Bullet.
        // We use damping 1 (no friction) for bullets.
        _movement = AddComponent(new MovementComponent(damping: 1f, clampToScreen: false));
        AddComponent(new BulletMovementBehaviour());
        AddComponent(new BulletCollisionBehaviour());
        AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
    }

    public void Reset(Vector2 position, Vector2 velocity)
    {
        Position   = position;
        IsExpired  = false;
        
        // Reset the data inside the component
        _movement.Velocity = velocity;
        _movement.Orientation = velocity.ToAngle();
    }
}
