using System.Collections.Generic;
using GeometryWars.Components.Physics;
using GeometryWars.Entities;

namespace GeometryWars.Systems;

// Handles broad ownership of collidable registrations and collision dispatch.
internal sealed class CollisionSystem
{
    private readonly List<(Entity Entity, Collider Collider)> _collidables = [];

    public void Register(Entity entity)
    {
        var collider = entity.GetComponent<Collider>();
        if (collider != null)
            _collidables.Add((entity, collider));
    }

    public void Clear() => _collidables.Clear();

    public void RemoveExpired()
    {
        _collidables.RemoveAll(entry => entry.Entity.IsExpired);
    }

    public void HandleCollisions()
    {
        for (int i = 0; i < _collidables.Count; i++)
        {
            var a = _collidables[i];
            if (a.Entity.IsExpired || !a.Collider.IsActive) continue;

            for (int j = i + 1; j < _collidables.Count; j++)
            {
                var b = _collidables[j];
                if (b.Entity.IsExpired || !b.Collider.IsActive) continue;

                if (CollisionRegistry.Intersects(a.Entity, a.Collider, b.Entity, b.Collider))
                {
                    a.Entity.OnCollision(b.Entity);
                    b.Entity.OnCollision(a.Entity);
                }
            }
        }
    }
}
