using Microsoft.Xna.Framework;
using GeometryWars.Components;

namespace GeometryWars;

public class Bullet : Entity
{
    private readonly MovementComponent _movement;

    public Bullet()
    {
        // Assembler: Plug in components
        AddComponent(new TransformComponent(Vector2.Zero));
        _movement = AddComponent(new MovementComponent(damping: 1f));
        AddComponent(new SpriteComponent(Art.Bullet));
        AddComponent(new BulletMovementBehaviour());
        AddComponent(new BulletCollisionBehaviour());
        AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
    }

    public void Reset(Vector2 position, Vector2 velocity)
    {
        IsExpired  = false;
        
        var transform = GetComponent<TransformComponent>();
        if (transform != null)
        {
            transform.Position = position;
            transform.Orientation = velocity.ToAngle();
        }
        
        _movement.Velocity = velocity;
    }
}
