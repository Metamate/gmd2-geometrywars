using GeometryWars.Services;
using Microsoft.Xna.Framework;

namespace GeometryWars.Components;

public sealed class GravityBehaviour : IComponent
{
    private readonly float _range;
    private readonly float _force;

    public GravityBehaviour(float range, float force)
    {
        _range = range;
        _force = force;
    }

    public void Update(Entity owner)
    {
        foreach (var entity in EntityManager.GetNearbyEntities(owner.Position, _range))
        {
            if (entity == owner) continue;

            if (entity is Enemy enemy && !enemy.IsActive)
                continue;

            // Find the movement component of the entity we want to pull
            var movement = entity.GetComponent<MovementComponent>();
            if (movement == null) continue;

            if (entity is Bullet)
            {
                movement.Velocity += (entity.Position - owner.Position).ScaleTo(0.3f);
            }
            else
            {
                var dPos = owner.Position - entity.Position;
                movement.Velocity += dPos.ScaleTo(MathHelper.Lerp(_force, 0, dPos.Length() / _range));
            }
        }
    }
}
