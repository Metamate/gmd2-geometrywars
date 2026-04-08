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
        _rigidbody = AddComponent(new RigidbodyComponent(damping: 1f));
        
        AddComponent(new SpriteComponent(Art.Bullet));
        AddComponent(new BulletMovementBehaviour());
        AddComponent(new BulletCollisionBehaviour());
        AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
    }

    public void Reset(Vector2 position, Vector2 velocity)
    {
        IsExpired  = false;
        
        Transform.Position = position;
        Transform.Orientation = velocity.ToAngle();
        _rigidbody.Velocity = velocity;
    }
}
