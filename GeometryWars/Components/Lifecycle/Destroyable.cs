using System;
using GeometryWars.Components.Core;
using GeometryWars.Entities;

namespace GeometryWars.Components.Lifecycle;

// Provides a reusable destruction signal for entities that need "on destroyed" behavior.
public sealed class Destroyable : Component
{
    private Entity _owner;

    public event Action<Entity> Destroyed;

    public override void OnStart(Entity owner)
    {
        _owner = owner;
    }

    public void Destroy()
    {
        if (_owner == null || _owner.IsExpired)
            return;

        Destroyed?.Invoke(_owner);
        _owner.IsExpired = true;
    }
}
