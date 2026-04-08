using Microsoft.Xna.Framework;
using GeometryWars.Components;

namespace GeometryWars;

/// <summary>
/// Archetype for player bullets.
/// </summary>
public class Bullet : Entity
{
    private readonly RigidbodyComponent _rigidbody;

    public Bullet()
    {
        // Rigidbody is cached immediately during construction
        _rigidbody = AddComponent(new RigidbodyComponent(damping: 1f));
        
        AddComponent(new SpriteComponent(Art.Bullet));
        AddComponent(new BulletMovementBehaviour());
        AddComponent(new BulletCollisionBehaviour());
        AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
    }

    /// <summary>
    /// Re-initializes a pooled bullet with a new position and velocity.
    /// </summary>
    public void Reset(Vector2 position, Vector2 velocity)
    {
        IsExpired  = false;
        
        Transform.Position = position;
        Transform.Orientation = velocity.ToAngle();
        _rigidbody.Velocity = velocity;
    }
}
