using GeometryWars.Components.Physics;
using GeometryWars.Components.Visuals;
using GeometryWars.Components.Combat;
using Microsoft.Xna.Framework;

namespace GeometryWars.Entities;

// Archetype for player bullets.
public class Bullet : Entity
{
    public void Reset(Vector2 position, Vector2 velocity)
    {
        IsExpired = false;

        Transform.Position    = position;
        Transform.Orientation = velocity.ToAngle();
        GetComponent<RigidbodyComponent>().Velocity = velocity;
    }
}
