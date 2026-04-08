using GeometryWars.Components.Core;
using GeometryWars.Components.Physics;
using GeometryWars.Components.Visuals;
using GeometryWars.Components.Combat;
using Microsoft.Xna.Framework;

namespace GeometryWars.Entities;

/// <summary>
/// Archetype for player bullets.
/// </summary>
public class Bullet : Entity
{
    private RigidbodyComponent _rigidbody;

    public Bullet()
    {
        // Assembler: Plug in components
        _rigidbody = AddComponent(new RigidbodyComponent(damping: 1f));
        AddComponent(new SpriteComponent(Art.Bullet));
        AddComponent(new BulletMovementBehaviour());
        AddComponent(new BulletCollisionBehaviour());
        AddComponent(new CircleColliderComponent(GameSettings.Bullets.ColliderRadius));
    }

    public void Reset(Vector2 position, Vector2 velocity)
    {
        IsExpired  = false;
        
        _rigidbody ??= GetComponent<RigidbodyComponent>();

        Transform.Position = position;
        Transform.Orientation = velocity.ToAngle();
        
        if (_rigidbody != null)
        {
            _rigidbody.Velocity = velocity;
        }
    }
}
