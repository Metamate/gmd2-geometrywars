using Microsoft.Xna.Framework;
using GeometryWars.Components;

namespace GeometryWars;

/// <summary>
/// Archetype for Bullets.
/// Now just an "Assembler" class that adds the right components.
/// Logic-free: all behaviour is in Components.
/// </summary>
public class Bullet : Entity
{
    public Bullet()
    {
        Image    = Art.Bullet;
        Collider = new CircleCollider(GameSettings.Bullets.ColliderRadius);

        // Assembler: Plug in the specific behaviours of a Bullet
        AddComponent(new BulletMovementBehaviour());
        AddComponent(new BulletCollisionBehaviour());
    }

    public void Reset(Vector2 position, Vector2 velocity)
    {
        Position   = position;
        Velocity   = velocity;
        Orientation = velocity.ToAngle();
        IsExpired  = false;
    }
}
