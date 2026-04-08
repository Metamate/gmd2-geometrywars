using Microsoft.Xna.Framework;
using GeometryWars.Components;

namespace GeometryWars;

public class Bullet : Entity
{
    private RigidbodyComponent _rigidbody;

    public Bullet()
    {
        // Assembler: Plug in components
        AddComponent(new TransformComponent(Vector2.Zero));
        AddComponent(new RigidbodyComponent(damping: 1f));
        AddComponent(new SpriteComponent(Art.Bullet));
        AddComponent(new BulletMovementBehaviour());
        AddComponent(new BulletCollisionBehaviour());
        AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
    }

    public void Reset(Vector2 position, Vector2 velocity)
    {
        IsExpired  = false;
        
        // Lazy find our own component for the Reset helper
        _rigidbody ??= GetComponent<RigidbodyComponent>();

        if (Transform != null)
        {
            Transform.Position = position;
            Transform.Orientation = velocity.ToAngle();
        }
        
        if (_rigidbody != null)
        {
            _rigidbody.Velocity = velocity;
        }
    }
}
