using Microsoft.Xna.Framework;
using GeometryWars.Components;

namespace GeometryWars;

/// <summary>
/// Archetype for Bullets.
/// Composed of movement, collision, and collider components.
/// </summary>
public class Bullet : Entity
{
    public Bullet()
    {
        Image    = Art.Bullet;

        // Assembler: Plug in the specific behaviours of a Bullet
        AddComponent(new BulletMovementBehaviour());
        AddComponent(new BulletCollisionBehaviour());
        AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
    }

    public void Reset(Vector2 position, Vector2 velocity)
    {
        Position   = position;
        Velocity   = velocity;
        Orientation = velocity.ToAngle();
        IsExpired  = false;
    }
}
